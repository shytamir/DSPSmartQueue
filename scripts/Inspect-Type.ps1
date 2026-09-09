#requires -Version 7.0
[CmdletBinding()]
param(
    [Parameter(Mandatory)][ValidatePattern('^[A-Za-z_][A-Za-z0-9_.+`]*$')][string]$Type,
    [ValidateSet('Assembly-CSharp.dll','BepInEx.dll','0Harmony.dll')][string]$Assembly = 'Assembly-CSharp.dll'
)
$ErrorActionPreference = 'Stop'
$root = Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/.runtime-tools'
$config = Get-Content (Join-Path $root 'local.json') -Raw | ConvertFrom-Json
$directory = if ($Assembly -eq 'Assembly-CSharp.dll') { $config.ManagedPath } else { $config.DependencyPath }
$source = Join-Path $directory $Assembly
# Include dependency hashes and decompiler version so reference/tool changes invalidate cache.
$parts = @($config.ILSpyVersion)
foreach ($path in @((Join-Path $config.ManagedPath '*.dll'), (Join-Path $config.DependencyPath '*.dll'))) {
    foreach ($file in Get-ChildItem -Path $path -File | Sort-Object Name) {
        $parts += $file.Name + ':' + (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
    }
}
$key = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes(($parts -join '|')))).ToLowerInvariant()
$cache = Join-Path $root "inspection/$key/$Assembly"
$output = Join-Path $cache "$Type.cs"
if (!(Test-Path -LiteralPath $output)) {
    New-Item -ItemType Directory -Force $cache | Out-Null
    $tool = Join-Path $root 'bin/ilspycmd.exe'
    $text = & $tool --disable-updatecheck -t $Type -r $config.ManagedPath -r $config.DependencyPath $source
    if ($LASTEXITCODE) { throw "Static decompilation failed for $Type." }
    $text | Set-Content -LiteralPath $output -Encoding utf8NoBOM
    Write-Host "Inspected $Type"
} else { Write-Host "Cache hit: $Type" }
$output
