#requires -Version 7.0
[CmdletBinding()]
param([string]$ManagedPath, [string]$DependencyPath, [ValidateRange(0,65535)][int]$BuildNumber = 0)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$configPath = Join-Path $repo 'artifacts/.runtime-tools/local.json'
if (Test-Path $configPath) {
    $config = Get-Content $configPath -Raw | ConvertFrom-Json
    if (!$ManagedPath) { $ManagedPath = $config.ManagedPath }
    if (!$DependencyPath) { $DependencyPath = $config.DependencyPath }
}
if (!$ManagedPath -or !$DependencyPath) { throw 'Supply -ManagedPath and -DependencyPath or initialize local tools first.' }
foreach ($name in 'Assembly-CSharp.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.UI.dll','UnityEngine.UIModule.dll','mscorlib.dll') {
    if (!(Test-Path -LiteralPath (Join-Path $ManagedPath $name) -PathType Leaf)) { throw "Missing reference $name in ManagedPath. Select the game Managed directory." }
}
foreach ($name in 'BepInEx.dll','0Harmony.dll') {
    if (!(Test-Path -LiteralPath (Join-Path $DependencyPath $name) -PathType Leaf)) { throw "Missing reference $name in DependencyPath. Select the offline dependency directory." }
}
$ManagedPath = (Resolve-Path -LiteralPath $ManagedPath).Path
$DependencyPath = (Resolve-Path -LiteralPath $DependencyPath).Path
$values = @{}
foreach ($line in [IO.File]::ReadAllLines((Join-Path $repo 'VERSION'))) {
    if ($line -notmatch '^(MAJOR|MINOR)=(0|[1-9][0-9]*)$') { throw "Invalid VERSION line: $line" }
    $key = $Matches[1]
    if ($values.ContainsKey($key) -or [long]$Matches[2] -gt 65535) { throw "Invalid or duplicate VERSION key: $key" }
    $values[$key] = $Matches[2]
}
if ($values.Count -ne 2) { throw 'VERSION must define MAJOR and MINOR.' }
$version = '{0}.{1}.{2}' -f $values.MAJOR, $values.MINOR, $BuildNumber
$generated = Join-Path $repo 'artifacts/generated'
New-Item -ItemType Directory -Force $generated | Out-Null
"namespace DSPSmartQueue { internal static class PluginVersion { internal const string Value = `"$version`"; } }" |
    Set-Content (Join-Path $generated 'PluginVersion.g.cs') -Encoding utf8NoBOM
$oldPackages = $env:NUGET_PACKAGES
try {
    $env:NUGET_PACKAGES = Join-Path $repo 'artifacts/nuget'
    & dotnet build (Join-Path $repo 'src/DSPSmartQueue/DSPSmartQueue.csproj') -c Release --nologo `
        "-p:ManagedPath=$ManagedPath" "-p:DependencyPath=$DependencyPath" "-p:Version=$version" `
        '-p:RestoreSources=https://api.nuget.org/v3/index.json'
    if ($LASTEXITCODE) { throw 'Plugin build failed.' }
    $dll = Join-Path $repo 'artifacts/build/DSPSmartQueue.dll'
    $unexpected = @(Get-ChildItem (Split-Path $dll) -File | Where-Object Name -ne 'DSPSmartQueue.dll')
    if ($unexpected.Count) { throw "Unexpected build output: $($unexpected.Name -join ', ')" }
    & dotnet run --project (Join-Path $repo 'tests/FoundationChecks/FoundationChecks.csproj') -c Release `
        '-p:RestoreSources=https://api.nuget.org/v3/index.json' -- $ManagedPath $DependencyPath $dll
    if ($LASTEXITCODE) { throw 'Foundation verification failed.' }
    Write-Host "Verified foundation DLL: $dll ($version)"
} finally { $env:NUGET_PACKAGES = $oldPackages }
