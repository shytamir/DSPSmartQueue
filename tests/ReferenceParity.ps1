#requires -Version 7.0
[CmdletBinding()]
param([string]$BuildNumber = '0')
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
& "$repo/scripts/Build-Local.ps1" -Hosted -BuildNumber $BuildNumber
$hosted = (Get-FileHash "$repo/artifacts/build/DSPSmartQueue.dll" -Algorithm SHA256).Hash
& "$repo/scripts/Build-Local.ps1" -BuildNumber $BuildNumber
$real = (Get-FileHash "$repo/artifacts/build/DSPSmartQueue.dll" -Algorithm SHA256).Hash
if ($hosted -ne $real) { throw 'Hosted and real-reference DLLs differ; review the reference declarations before release.' }
Write-Host "PASS: hosted and real-reference builds produce the same DLL (SHA256 $real)."
