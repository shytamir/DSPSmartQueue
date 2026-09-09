#requires -Version 7.0
[CmdletBinding()]
param([string]$ManagedPath, [string]$DependencyPath, [string]$BuildNumber = '0')
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$output = Join-Path $repo 'artifacts/build'
$staging = Join-Path $repo 'artifacts/build-staging'
$dll = Join-Path $output 'DSPSmartQueue.dll'
$evidence = Join-Path $output 'build.json'
$oldPackages = $env:NUGET_PACKAGES

function Read-SourceIdentity {
    $revision = & git -c "safe.directory=$repo" -C $repo rev-parse HEAD
    if ($LASTEXITCODE) { throw 'Cannot identify source revision.' }
    $status = @(& git -c "safe.directory=$repo" -C $repo status --porcelain=v1 --untracked-files=all)
    if ($LASTEXITCODE) { throw 'Cannot identify working-tree changes.' }
    $paths = @(& git -c "safe.directory=$repo" -C $repo ls-files --cached --others --exclude-standard)
    if ($LASTEXITCODE) { throw 'Cannot enumerate source files.' }
    $files = @(foreach ($path in ($paths | Sort-Object -Unique)) {
        $file = Join-Path $repo $path
        if (Test-Path -LiteralPath $file -PathType Leaf) {
            [ordered]@{ path = $path; sha256 = (Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash }
        }
    })
    [ordered]@{ revision = $revision; dirty = $status.Count -gt 0; changes = $status; files = $files }
}

function Read-ReferenceIdentity {
    @(foreach ($directory in @($ManagedPath, $DependencyPath)) {
        foreach ($file in (Get-ChildItem -LiteralPath $directory -Filter '*.dll' -File | Sort-Object Name)) {
            [ordered]@{
                group = $(if ($directory -eq $ManagedPath) { 'game' } else { 'dependencies' })
                name = $file.Name
                sha256 = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
            }
        }
    })
}

try {
    # Invalidate the previous deliverable before validating this invocation's inputs.
    foreach ($file in @($dll, $evidence, (Join-Path $staging 'DSPSmartQueue.dll'))) {
        if (Test-Path -LiteralPath $file) { Remove-Item -LiteralPath $file -Force }
    }
    if (Test-Path -LiteralPath $output) {
        $unexpected = @(Get-ChildItem -LiteralPath $output -Force)
        if ($unexpected.Count) { throw "Unexpected deliverable content: $($unexpected.Name -join ', ')" }
    }
    $configPath = Join-Path $repo 'artifacts/.runtime-tools/local.json'
    if (Test-Path -LiteralPath $configPath) {
        $config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
        if (!$ManagedPath) { $ManagedPath = $config.ManagedPath }
        if (!$DependencyPath) { $DependencyPath = $config.DependencyPath }
    }
    if (!$ManagedPath -or !$DependencyPath) { throw 'Supply -ManagedPath and -DependencyPath or initialize local tools first.' }
    foreach ($name in 'Assembly-CSharp.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.UI.dll','UnityEngine.UIModule.dll','mscorlib.dll') {
        if (!(Test-Path -LiteralPath (Join-Path $ManagedPath $name) -PathType Leaf)) { throw "Missing reference $name in ManagedPath." }
    }
    foreach ($name in 'BepInEx.dll','0Harmony.dll') {
        if (!(Test-Path -LiteralPath (Join-Path $DependencyPath $name) -PathType Leaf)) { throw "Missing reference $name in DependencyPath." }
    }
    $ManagedPath = (Resolve-Path -LiteralPath $ManagedPath).Path
    $DependencyPath = (Resolve-Path -LiteralPath $DependencyPath).Path
    $version = & "$PSScriptRoot/Get-BuildVersion.ps1" -BuildNumber $BuildNumber
    $source = Read-SourceIdentity
    $references = Read-ReferenceIdentity
    $generated = Join-Path $repo 'artifacts/generated'
    New-Item -ItemType Directory -Force $generated, $output, $staging | Out-Null
    "namespace DSPSmartQueue { internal static class PluginVersion { internal const string Value = `"$version`"; } }" |
        Set-Content (Join-Path $generated 'PluginVersion.g.cs') -Encoding utf8NoBOM
    $env:NUGET_PACKAGES = Join-Path $repo 'artifacts/nuget'
    & dotnet build (Join-Path $repo 'src/DSPSmartQueue/DSPSmartQueue.csproj') -c Release --nologo `
        "-p:ManagedPath=$ManagedPath" "-p:DependencyPath=$DependencyPath" "-p:Version=$version" `
        '-p:RestoreSources=https://api.nuget.org/v3/index.json'
    if ($LASTEXITCODE) { throw 'Plugin build failed.' }
    $candidate = Join-Path $staging 'DSPSmartQueue.dll'
    $unexpected = @(Get-ChildItem -LiteralPath $staging -File | Where-Object Name -ne 'DSPSmartQueue.dll')
    if ($unexpected.Count) { throw "Unexpected build output: $($unexpected.Name -join ', ')" }
    & dotnet run --project (Join-Path $repo 'tests/FoundationChecks/FoundationChecks.csproj') -c Release `
        '-p:RestoreSources=https://api.nuget.org/v3/index.json' -- $ManagedPath $DependencyPath $candidate
    if ($LASTEXITCODE) { throw 'Offline checks failed.' }
    if (($source | ConvertTo-Json -Depth 5 -Compress) -ne ((Read-SourceIdentity) | ConvertTo-Json -Depth 5 -Compress) -or
        ($references | ConvertTo-Json -Depth 5 -Compress) -ne ((Read-ReferenceIdentity) | ConvertTo-Json -Depth 5 -Compress)) {
        throw 'Source or references changed during verification; rebuild from stable inputs.'
    }
    $record = [ordered]@{
        version = $version
        source = $source
        references = $references
        sdk = (& dotnet --version)
        checks = @('real-reference compilation', 'offline logic', 'real metadata and hook signatures')
        runtimeValidated = $false
        output = [ordered]@{ name = 'DSPSmartQueue.dll'; sha256 = (Get-FileHash -LiteralPath $candidate -Algorithm SHA256).Hash }
    }
    if ($LASTEXITCODE) { throw 'Cannot identify the .NET SDK.' }
    Copy-Item -LiteralPath $candidate -Destination $dll
    $record | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $evidence -Encoding utf8NoBOM
    Write-Host "Verified plugin DLL: $dll ($version); identity: $evidence"
} catch {
    foreach ($file in @($dll, $evidence)) {
        if (Test-Path -LiteralPath $file) { Remove-Item -LiteralPath $file -Force }
    }
    throw
} finally {
    $env:NUGET_PACKAGES = $oldPackages
}
