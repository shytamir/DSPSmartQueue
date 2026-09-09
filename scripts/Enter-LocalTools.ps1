#requires -Version 7.0
# Dot-source this file: . ./scripts/Enter-LocalTools.ps1
$ErrorActionPreference = 'Stop'
& (Join-Path $PSScriptRoot 'Initialize-LocalTools.ps1') -CheckOnly
$localToolBin = Join-Path (Split-Path $PSScriptRoot -Parent) 'artifacts/.runtime-tools/bin'
if ($localToolBin -notin ($env:PATH -split [IO.Path]::PathSeparator)) {
    $env:PATH = $localToolBin + [IO.Path]::PathSeparator + $env:PATH
}
