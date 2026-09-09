#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Path,
    [string]$BuildDirectory = (Join-Path $PSScriptRoot '../artifacts/build')
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$repo = Split-Path $PSScriptRoot -Parent
$build = Get-Content -LiteralPath (Join-Path $BuildDirectory 'build.json') -Raw | ConvertFrom-Json
if ($build.source.dirty -or $build.source.changes.Count -ne 0) { throw 'Handoff requires a clean source build.' }
$files = [ordered]@{
    'README.md' = 'packaging/README.md'
    'OWNER-PROCEDURE.md' = 'docs/OWNER-PROCEDURE.md'
    'LICENSE' = 'LICENSE'
    'icon.png' = 'packaging/icon.png'
}
$expected = @($files.Keys) + @('manifest.json', 'build.json', 'BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll')
$zip = [IO.Compression.ZipFile]::OpenRead((Resolve-Path -LiteralPath $Path).Path)
try {
    if ($zip.Entries.Count -ne $expected.Count -or
        (Compare-Object -ReferenceObject $expected -DifferenceObject @($zip.Entries.FullName) -CaseSensitive)) {
        throw 'Package entries differ from the handoff allowlist.'
    }
    $hashes = [ordered]@{}
    foreach ($entry in $zip.Entries) {
        $stream = $entry.Open()
        try { $hashes[$entry.FullName] = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($stream)) }
        finally { $stream.Dispose() }
    }
    foreach ($name in $files.Keys) {
        $source = Join-Path $repo $files[$name]
        $recorded = @($build.source.files | Where-Object path -CEQ $files[$name])
        if ($recorded.Count -ne 1 -or $hashes[$name] -ne $recorded[0].sha256 -or
            $hashes[$name] -ne (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash) {
            throw "Package/source identity mismatch: $name"
        }
    }
    if ($hashes['build.json'] -ne (Get-FileHash -LiteralPath (Join-Path $BuildDirectory 'build.json')).Hash -or
        $hashes['BepInEx/plugins/DSPSmartQueue/DSPSmartQueue.dll'] -ne $build.output.sha256 -or
        $build.output.sha256 -ne (Get-FileHash -LiteralPath (Join-Path $BuildDirectory 'DSPSmartQueue.dll')).Hash) {
        throw 'Package DLL or build evidence differs from the verified build.'
    }
    $stream = $zip.GetEntry('manifest.json').Open()
    $reader = [IO.StreamReader]::new($stream, [Text.UTF8Encoding]::new($false, $true))
    try { $manifest = $reader.ReadToEnd() | ConvertFrom-Json -AsHashtable }
    finally { $reader.Dispose() }
    $templatePath = Join-Path $repo 'packaging/manifest.json'
    $recorded = @($build.source.files | Where-Object path -CEQ 'packaging/manifest.json')
    if ($recorded.Count -ne 1 -or $recorded[0].sha256 -ne (Get-FileHash $templatePath).Hash) { throw 'Manifest template changed since build.' }
    $template = Get-Content -LiteralPath $templatePath -Raw | ConvertFrom-Json -AsHashtable
    $template.version_number = $build.version
    if ($manifest.Count -ne $template.Count -or
        (Compare-Object @($template.Keys) @($manifest.Keys) -CaseSensitive)) { throw 'Unexpected manifest fields.' }
    foreach ($key in @('name', 'description', 'website_url', 'version_number')) {
        if ($manifest[$key] -cne $template[$key]) { throw "Manifest mismatch: $key" }
    }
    if ($manifest.dependencies.Count -ne $template.dependencies.Count -or
        (Compare-Object @($template.dependencies) @($manifest.dependencies) -CaseSensitive -SyncWindow 0) -or
        $manifest.name -notmatch '^[A-Za-z0-9_]+$' -or $manifest.description.Length -gt 250 -or
        $manifest.version_number -notmatch '^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$') {
        throw 'Invalid or inconsistent package metadata.'
    }
    $stream = $zip.GetEntry('icon.png').Open()
    $memory = [IO.MemoryStream]::new()
    try {
        $stream.CopyTo($memory)
        $memory.Position = 0
        $icon = [Drawing.Image]::FromStream($memory)
        try {
            if ($icon.Width -ne 256 -or $icon.Height -ne 256 -or $icon.RawFormat.Guid -ne [Drawing.Imaging.ImageFormat]::Png.Guid) {
                throw 'Package icon must be a 256x256 PNG.'
            }
        } finally { $icon.Dispose() }
    } finally { $memory.Dispose(); $stream.Dispose() }
    [ordered]@{
        package = [IO.Path]::GetFileName($Path)
        sha256 = (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash
        version = $build.version
        revision = $build.source.revision
        dllSha256 = $build.output.sha256
        entries = $hashes
    }
} finally { $zip.Dispose() }
