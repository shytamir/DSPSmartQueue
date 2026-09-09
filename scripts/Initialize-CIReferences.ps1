#requires -Version 7.0
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$root = Join-Path $repo 'artifacts/ci-references'
$managed = Join-Path $root 'managed'
$dependencies = Join-Path $root 'dependencies'
New-Item -ItemType Directory -Force $managed, $dependencies | Out-Null

function Get-Archive($name, $url, $hash) {
    $archive = Join-Path $root "$name.zip"
    Invoke-WebRequest -Uri $url -OutFile $archive
    if ((Get-FileHash -LiteralPath $archive -Algorithm SHA256).Hash -ne $hash) {
        throw "Download checksum mismatch: $name"
    }
    $destination = Join-Path $root $name
    Expand-Archive -LiteralPath $archive -DestinationPath $destination -Force
    $destination
}

$unity = Get-Archive 'unity' `
    'https://nuget.bepinex.dev/v3/package/unityengine.modules/2022.3.62/unityengine.modules.2022.3.62.nupkg' `
    '58FAFA575544D7216AE60B3D489C82593354D9664AAA39B3BA34A8537B06183A'
Copy-Item -Path "$unity/lib/net45/*.dll" -Destination $managed
$bepinex = Get-Archive 'bepinex' `
    'https://thunderstore.io/package/download/xiaoye97/BepInEx/5.4.17/' `
    'EA24D1C33FA63BE98768D65057A79002A77A5FB20DAB3C7E072CBD3044A1B9FE'
Copy-Item -Path "$bepinex/BepInExPack/BepInEx/core/*.dll" -Destination $dependencies
& dotnet build (Join-Path $repo 'build/References/Game/Game.csproj') -c Release --nologo
if ($LASTEXITCODE) { throw 'Compile-reference build failed.' }
# Metadata inspection needs a framework core library, not an executing game runtime.
$framework = Join-Path $HOME '.nuget/packages/microsoft.netframework.referenceassemblies.net472/1.0.3/build/.NETFramework/v4.7.2'
if ($env:NUGET_PACKAGES) {
    $framework = Join-Path $env:NUGET_PACKAGES 'microsoft.netframework.referenceassemblies.net472/1.0.3/build/.NETFramework/v4.7.2'
}
Copy-Item -Path "$framework/*.dll" -Destination $managed
Copy-Item -Path "$framework/Facades/*.dll" -Destination $managed
Write-Host 'Prepared public dependencies and compile-only game/UI shims.'
