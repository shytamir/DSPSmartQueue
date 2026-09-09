#requires -Version 7.0
[CmdletBinding()]
param([string]$ManagedPath, [string]$DependencyPath, [string]$BuildNumber = '0', [switch]$Hosted)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$output = Join-Path $repo 'artifacts/package'
New-Item -ItemType Directory -Force $output | Out-Null
$temp = Join-Path $output 'package.pending.zip'
try {
    if (Test-Path -LiteralPath $temp) { Remove-Item -LiteralPath $temp }
    & "$PSScriptRoot/Build-Local.ps1" -ManagedPath $ManagedPath -DependencyPath $DependencyPath -BuildNumber $BuildNumber -Hosted:$Hosted
    $buildDirectory = Join-Path $repo 'artifacts/build'
    $build = Get-Content -LiteralPath (Join-Path $buildDirectory 'build.json') -Raw | ConvertFrom-Json
    if ($build.source.dirty) { throw 'Commit source and documentation before producing a release package.' }
    $manifest = Get-Content (Join-Path $repo 'packaging/manifest.json') -Raw | ConvertFrom-Json
    $manifest.version_number = $build.version
    $zip = [IO.Compression.ZipFile]::Open($temp, [IO.Compression.ZipArchiveMode]::Create)
    try {
        $files = [ordered]@{
            'README.md' = 'packaging/README.md'
            'LICENSE' = 'LICENSE'
            'icon.png' = 'packaging/icon.png'
            'BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll' = 'artifacts/build/DSPSmartQueue.dll'
        }
        foreach ($name in $files.Keys) {
            [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, (Join-Path $repo $files[$name]), $name) | Out-Null
        }
        $writer = [IO.StreamWriter]::new($zip.CreateEntry('manifest.json').Open(), [Text.UTF8Encoding]::new($false))
        try { $writer.Write(($manifest | ConvertTo-Json -Depth 3)) } finally { $writer.Dispose() }
    } finally { $zip.Dispose() }
    $inspection = & "$PSScriptRoot/Test-Package.ps1" -Path $temp -BuildDirectory $buildDirectory
    $name = "DSPSmartQueue-$($build.version)-$($build.source.revision.Substring(0,12))-$($inspection.sha256.Substring(0,12)).zip"
    $destination = Join-Path $output $name
    $inspection.package = $name
    $inspection | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath "$temp.json" -Encoding utf8NoBOM
    Copy-Item -LiteralPath (Join-Path $buildDirectory 'build.json') -Destination "$destination.build.json" -Force
    Move-Item -LiteralPath "$temp.json" -Destination "$destination.inspection.json" -Force
    Move-Item -LiteralPath $temp -Destination $destination -Force
    Write-Host "Inspected release package: $destination"
    $destination
} finally {
    foreach ($file in @($temp, "$temp.json")) {
        if (Test-Path -LiteralPath $file) { Remove-Item -LiteralPath $file }
    }
}
