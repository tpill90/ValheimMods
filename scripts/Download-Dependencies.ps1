function Ensure-BepinExIsInstalled
{
    if(Test-Path "$valheimInstallDir\BepInEx")
    {
        Write-Host "BepInEx " -ForegroundColor Cyan -NoNewline
        Write-Host "already installed.  Skipping..."
        return
    }

    # Downloading BepinEx
    $package = $allThunderstorePackages | Where-Object { $_.Name -eq "BepInExPack_Valheim" } | Select -First 1
    $version = $package.Versions | Select -First 1

    Write-Host "Downloading " -NoNewline; Write-Host "BepInEx" -ForegroundColor Cyan -NoNewline; Write-Host "..."

    # Download
    Invoke-WebRequest -Uri $version.download_url -OutFile "$Env:TEMP\$($version.name).zip"
    # Extract zip
    Expand-Archive -LiteralPath "$Env:TEMP\$($version.name).zip" -DestinationPath $Env:TEMP -Force

    # Copy to install dir
    Copy-Item "$Env:TEMP\$($version.name)\*" -Destination $valheimInstallDir -Recurse -Force
}

function Ensure-DependencyIsInstalled($modName)
{
    $package = $allThunderstorePackages | Where-Object { $_.full_name -eq $modName } | Select -First 1
    $version = $package.Versions | Select -First 1

    if(Test-Path "$valheimInstallDir\BepInEx\plugins\$($version.name)")
    {
        Write-Host "$modName " -ForegroundColor Cyan -NoNewline
        Write-Host "already installed.  Skipping..."
        return
    }

    Write-Host "Downloading " -NoNewline; Write-Host $modName -ForegroundColor Cyan -NoNewline; Write-Host "..."
    Invoke-WebRequest -Uri $version.download_url -OutFile "$Env:TEMP\$($version.name).zip"
    Expand-Archive -LiteralPath "$Env:TEMP\$($version.name).zip" -DestinationPath "$Env:TEMP\$($version.name)" -Force
    New-Item "$valheimInstallDir\BepInEx\plugins\$($version.name)\" -ItemType Directory
    Copy-Item "$Env:TEMP\$($version.name)\*" -Destination "$valheimInstallDir\BepInEx\plugins\$($version.name)\" -Recurse
}


Push-Location $PSScriptRoot

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.IO.Compression.FileSystem

# Getting Valheim install dir
[xml]$parsedProps = Get-Content ..\Environment.props
$valheimInstallDir = $parsedProps.Project.PropertyGroup.VALHEIM_INSTALL
if(-Not(Test-Path $valheimInstallDir))
{
    Write-Error "Cannot detect Valheim install at $valheimInstallDir.  Stopping..."
    return
}

Write-Host "Querying Thunderstore for package list..."
$allThunderstorePackages = Invoke-RestMethod "https://valheim.thunderstore.io/api/v1/package/"
$allThunderstorePackages = $allThunderstorePackages | Where-Object { $_.is_deprecated -eq $false }
Write-Host "Found " -NoNewline; Write-Host $allThunderstorePackages.Count -ForegroundColor Yellow -NoNewline; Write-Host " available packages"

Ensure-BepinExIsInstalled

# Installing mod dependencies
Ensure-DependencyIsInstalled "pipakin-SkillInjector";

Pop-Location