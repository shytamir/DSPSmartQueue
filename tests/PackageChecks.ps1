#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    [string]$BuildDirectory = (Join-Path $PSScriptRoot '../artifacts/build')
)
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$inspect = Join-Path $repo 'scripts/Test-Package.ps1'
& $inspect -Path $Path -BuildDirectory $BuildDirectory | Out-Null
$expected = @('manifest.json', 'README.md', 'icon.png', 'LICENSE', 'BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll')
$zip = [IO.Compression.ZipFile]::OpenRead((Resolve-Path -LiteralPath $Path).Path)
try {
    if ($zip.Entries.Count -ne 5 -or (Compare-Object $expected @($zip.Entries.FullName) -CaseSensitive)) {
        throw 'The public download must contain only metadata, README, icon, license, and plugin.'
    }
} finally { $zip.Dispose() }

$temp = Join-Path $repo "artifacts/package-check-$([Guid]::NewGuid()).zip"
function Reject($label, $change) {
    Copy-Item -LiteralPath $Path -Destination $temp -Force
    $archive = [IO.Compression.ZipFile]::Open($temp, [IO.Compression.ZipArchiveMode]::Update)
    try { & $change $archive } finally { $archive.Dispose() }
    $rejected = $false
    try { & $inspect -Path $temp -BuildDirectory $BuildDirectory | Out-Null }
    catch { $rejected = $true }
    if (!$rejected) { throw "Inspector accepted $label" }
    Write-Host "PASS: rejected $label"
}
try {
    foreach ($extra in @('OWNER-PROCEDURE.md', 'build.json', 'Assembly-CSharp.dll', 'docs/PROJECT.md')) {
        Reject $extra { param($archive) $archive.CreateEntry($extra) | Out-Null }
    }
    Reject 'missing license' { param($archive) $archive.GetEntry('LICENSE').Delete() }
    Reject 'duplicate README' { param($archive) $archive.CreateEntry('README.md') | Out-Null }
    Reject 'altered DLL' {
        param($archive)
        $stream = $archive.GetEntry('BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll').Open()
        try { $stream.SetLength(0) } finally { $stream.Dispose() }
    }
    Reject 'altered icon' {
        param($archive)
        $stream = $archive.GetEntry('icon.png').Open()
        try { $stream.SetLength(0) } finally { $stream.Dispose() }
    }
    Reject 'inconsistent manifest version' {
        param($archive)
        $entry = $archive.GetEntry('manifest.json')
        $reader = [IO.StreamReader]::new($entry.Open())
        try { $manifest = $reader.ReadToEnd() | ConvertFrom-Json } finally { $reader.Dispose() }
        $manifest.version_number = '99.0.0'
        $entry.Delete()
        $writer = [IO.StreamWriter]::new($archive.CreateEntry('manifest.json').Open())
        try { $writer.Write(($manifest | ConvertTo-Json)) } finally { $writer.Dispose() }
    }
    Write-Host 'PASS: public package contents and inspector regression checks'
} finally {
    if (Test-Path -LiteralPath $temp) { Remove-Item -LiteralPath $temp }
}
