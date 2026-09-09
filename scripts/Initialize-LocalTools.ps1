#requires -Version 7.0
[CmdletBinding()]
param(
    [string]$ManagedPath,
    [string]$DependencyPath,
    [switch]$CheckOnly
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$root = Join-Path $repo 'artifacts/.runtime-tools'
$configPath = Join-Path $root 'local.json'
$version = '11.0.0.9375'
if (Test-Path $configPath) {
    $saved = Get-Content $configPath -Raw | ConvertFrom-Json
    if (!$ManagedPath) { $ManagedPath = $saved.ManagedPath }
    if (!$DependencyPath) { $DependencyPath = $saved.DependencyPath }
}
if (!$ManagedPath -or !$DependencyPath) {
    throw 'First run requires -ManagedPath (game Managed directory) and -DependencyPath (offline BepInEx/Harmony DLL directory).'
}
$ManagedPath = (Resolve-Path -LiteralPath $ManagedPath).Path
$DependencyPath = (Resolve-Path -LiteralPath $DependencyPath).Path
$references = @(
    foreach ($name in 'Assembly-CSharp.dll','UnityEngine.dll','UnityEngine.CoreModule.dll','UnityEngine.UI.dll','UnityEngine.UIModule.dll','mscorlib.dll') {
        Join-Path $ManagedPath $name
    }
    foreach ($name in 'BepInEx.dll','0Harmony.dll') { Join-Path $DependencyPath $name }
)
$inventory = @(foreach ($path in $references) {
    if (!(Test-Path -LiteralPath $path -PathType Leaf)) { throw "Missing reference: $path" }
    # GetAssemblyName reads the PE identity without executing the target assembly.
    [ordered]@{ Name = [IO.Path]::GetFileName($path); Path = $path
        AssemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($path).Version.ToString()
        SHA256 = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() }
})
foreach ($name in 'dotnet','git','rg') { Get-Command $name -ErrorAction Stop | Out-Null }
$sdk = (& dotnet --version | Out-String).Trim()
if ($LASTEXITCODE) { throw 'dotnet SDK check failed.' }
if ([version]$sdk -lt [version]'10.0.100') { throw 'Pinned ILSpy requires .NET 10; select an installed .NET 10 SDK/runtime.' }
$toolPath = Join-Path $root 'bin'
$tool = Join-Path $toolPath 'ilspycmd.exe'
if (!(Test-Path $tool)) {
    if ($CheckOnly) { throw 'ILSpy is missing. Run initialization without -CheckOnly.' }
    New-Item -ItemType Directory -Force $root | Out-Null
    # Explicit source and local cache; no global tool installation or product manifest.
    $nugetConfig = Join-Path $root 'NuGet.Config'
    '<configuration><packageSources><clear/><add key="nuget.org" value="https://api.nuget.org/v3/index.json"/></packageSources></configuration>' | Set-Content $nugetConfig
    $oldPackages = $env:NUGET_PACKAGES
    try {
        $env:NUGET_PACKAGES = Join-Path $root 'packages'
        & dotnet tool install ilspycmd --version $version --tool-path $toolPath --configfile $nugetConfig
        if ($LASTEXITCODE) { throw 'ILSpy installation failed. No automatic retry.' }
    } finally { $env:NUGET_PACKAGES = $oldPackages }
}
$actual = (& $tool --version | Out-String).Trim()
if ($LASTEXITCODE -or $actual -notmatch [regex]::Escape($version)) { throw "Unexpected ILSpy version: $actual" }
if (!$CheckOnly) {
    [ordered]@{ ManagedPath = $ManagedPath; DependencyPath = $DependencyPath; ILSpyVersion = $version } |
        ConvertTo-Json | Set-Content $configPath -Encoding utf8NoBOM
    [ordered]@{ CapturedUtc = [DateTime]::UtcNow.ToString('o'); SDK = $sdk
        PowerShell = $PSVersionTable.PSVersion.ToString(); ILSpy = $actual; References = $inventory } |
        ConvertTo-Json -Depth 5 | Set-Content (Join-Path $root 'baseline.json') -Encoding utf8NoBOM
} elseif (Test-Path (Join-Path $root 'baseline.json')) {
    $baseline = Get-Content (Join-Path $root 'baseline.json') -Raw | ConvertFrom-Json
    foreach ($item in $inventory) {
        $previous = @($baseline.References | Where-Object Name -eq $item.Name)
        if ($previous.Count -ne 1 -or $previous[0].SHA256 -ne $item.SHA256) { throw "Reference drift: $($item.Name). Review before refreshing baseline." }
    }
} else { throw 'Reference baseline is missing. Run initialization before activation.' }
Write-Host "Ready: PowerShell $($PSVersionTable.PSVersion), .NET SDK $sdk, ILSpy $version; $($inventory.Count) references checked."
