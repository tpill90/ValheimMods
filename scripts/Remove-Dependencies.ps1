Push-Location $PSScriptRoot

# Getting Valheim install dir
[xml]$parsedProps = Get-Content ..\Environment.props
$valheimInstallDir = $parsedProps.Project.PropertyGroup.VALHEIM_INSTALL
if(-Not(Test-Path $valheimInstallDir -ErrorAction SilentlyContinue))
{
    Write-Error "Cannot detect Valheim install at $valheimInstallDir.  Stopping..."
    return
}

# Removing all installed mods so that there are no conflicts with R2ModMan
Remove-Item "$valheimInstallDir\BepInEx" -Recurse -Force

Pop-Location