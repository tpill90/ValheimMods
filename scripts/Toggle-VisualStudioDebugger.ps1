Push-Location $PSScriptRoot

# Getting Valheim install dir
[xml]$parsedProps = Get-Content ..\Environment.props
$valheimInstallDir = $parsedProps.Project.PropertyGroup.VALHEIM_INSTALL
if(-Not(Test-Path $valheimInstallDir -ErrorAction SilentlyContinue))
{
    Write-Error "Cannot detect Valheim install at $valheimInstallDir.  Stopping..."
    return
}

if(Test-Path "$valheimInstallDir\UnityPlayer.dll")
{
    # Creating backup of original unity player
    Copy-Item "$valheimInstallDir\UnityPlayer.dll" -Destination "$valheimInstallDir\UnityPlayer.dll.bak"

    # Copying over the debug version of the player, in order to allow debugging
    Copy-Item -Path ..\libraries\Debug\UnityPlayer.dll -Destination "$valheimInstallDir\UnityPlayer.dll" -Force
    
    # Allow connecting to the debug version of the game
    Add-Content "$valheimInstallDir\valheim_Data\boot.config" "player-connection-debug=1"
}

Pop-Location