#requires -Version 7.0
[CmdletBinding()]
param([ValidateRange(0,65535)][int]$BuildNumber = 0)
$ErrorActionPreference = 'Stop'
$values = @{}
foreach ($line in [IO.File]::ReadAllLines((Join-Path $PSScriptRoot '../VERSION'))) {
    if ($line -notmatch '^(MAJOR|MINOR)=(0|[1-9][0-9]*)$') { throw "Invalid VERSION line: $line" }
    $key = $Matches[1]
    if ($values.ContainsKey($key) -or [long]$Matches[2] -gt 65535) { throw "Invalid or duplicate VERSION key: $key" }
    $values[$key] = $Matches[2]
}
if ($values.Count -ne 2) { throw 'VERSION must define MAJOR and MINOR.' }
'{0}.{1}.{2}' -f $values.MAJOR, $values.MINOR, $BuildNumber
