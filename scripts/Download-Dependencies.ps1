function Ensure-BepinExIsInstalled
{
    if(Test-Path "$valheimInstallDir\BepInEx")
    {
        Write-Host "BepInEx already installed.  Skipping"
    }
    # Downloading BepinEx
    $package = $packageList | Where-Object { $_.Name -eq "BepInExPack_Valheim" } | Select -First 1
    $version = $package.Versions | Select -First 1

    Write-Host "Downloading BepInEx..."
    Invoke-WebRequest -Uri $version.download_url -OutFile "$Env:TEMP\$($version.name).zip"
    Write-Host "Extracting..."
    Expand-Archive -LiteralPath "$Env:TEMP\$($version.name).zip" -DestinationPath $Env:TEMP -Force
    Write-Host "Copying"
    Copy-Item "$Env:TEMP\$($version.name)\*" -Destination $valheimInstallDir -Recurse -Force
}


Push-Location $PSScriptRoot

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.IO.Compression.FileSystem

# Getting Valheim install dir
[xml]$parsedProps = Get-Content .\Environment.props
$valheimInstallDir = $parsedProps.Project.PropertyGroup.VALHEIM_INSTALL
if(-Not(Test-Path $valheimInstallDir))
{
    Write-Error "Cannot detect Valheim install at $valheimInstallDir.  Stopping..."
}

Write-Host "Querying Thunderstore for package list..."
$packageList = Invoke-RestMethod "https://valheim.thunderstore.io/api/v1/package/"
$packageList = $packageList | Where-Object { $_.is_deprecated -eq $false }

Ensure-BepinExIsInstalled

# Installing mod dependencies
$package = $packageList | Where-Object { $_.full_name -eq "pipakin-SkillInjector" } | Select -First 1
$version = $package.Versions | Select -First 1

Write-Host "Downloading..."
Invoke-WebRequest -Uri $version.download_url -OutFile "$Env:TEMP\$($version.name).zip"
Write-Host "Extracting..."
Expand-Archive -LiteralPath "$Env:TEMP\$($version.name).zip" -DestinationPath "$Env:TEMP\$($version.name)" -Force
Write-Host "Copying"
New-Item "$valheimInstallDir\BepInEx\plugins\$($version.name)\" -ItemType Directory
Copy-Item "$Env:TEMP\$($version.name)\*" -Destination "$valheimInstallDir\BepInEx\plugins\$($version.name)\" -Recurse