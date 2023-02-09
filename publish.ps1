#TODO compare this to the latest JotunnModStub

param(
    [Parameter(Mandatory)]
    [ValidateSet('Debug','Release')]
    [System.String]$Target,
    
    [Parameter(Mandatory)]
    [System.String]$TargetPath,
    
    [Parameter(Mandatory)]
    [System.String]$TargetAssembly,

    [Parameter(Mandatory)]
    [System.String]$ValheimPath,

    [Parameter(Mandatory)]
    [System.String]$ProjectPath
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "Target : $Target"
Write-Host "TargetPath : $TargetPath"
Write-Host "TargetAssembly : $TargetAssembly"
Write-Host "ValheimPath : $ValheimPath"
Write-Host "ProjectPath : $ProjectPath"
Write-Host ""

# Make sure Get-Location is the script path
Push-Location -Path (Split-Path -Parent $MyInvocation.MyCommand.Path)

# Test some preliminaries
("$TargetPath",
 "$ValheimPath",
 "$(Get-Location)\libraries"
) | % {
    if (!(Test-Path "$_")) {Write-Error -ErrorAction Stop -Message "$_ folder is missing"}
}

# Plugin name without ".dll"
$name = "$TargetAssembly" -Replace('.dll')

Write-Host "--- Publishing $Target ---"

# Create the mdb file, for debugging with Mono
if (Test-Path -Path "$TargetPath\$name.pdb") 
{
    Write-Host "Create mdb file for plugin $name"
    Invoke-Expression "& `"$(Get-Location)\libraries\Debug\pdb2mdb.exe`" `"$TargetPath\$TargetAssembly`""
}

# Main Script

# TODO should probably implement a "clean project" target
if ($Target.Equals("Debug")) 
{
    Write-Host "Updating local installation in $ValheimPath"
    
    $plug = New-Item -Type Directory -Path "$ValheimPath\BepInEx\plugins\$name" -Force
    Write-Host "Copy $TargetAssembly to $plug"
    Copy-Item -Path "$TargetPath\$name.dll" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.pdb" -Destination "$plug" -Force
    Copy-Item -Path "$TargetPath\$name.dll.mdb" -Destination "$plug" -Force
    
    # Sets up debugger requirements
    $mono = "$ValheimPath\MonoBleedingEdge\EmbedRuntime";
    if (!(Test-Path -Path "$mono\mono-2.0-bdwgc.dll.orig")) 
    {
        Write-Host "Copy mono-2.0-bdwgc.dll to $mono"
        Copy-Item -Path "$mono\mono-2.0-bdwgc.dll" -Destination "$mono\mono-2.0-bdwgc.dll.orig" -Force

        $monoFileHash = Get-FileHash -Path "$mono\mono-2.0-bdwgc.dll" -Algorithm MD5
        if($monoFileHash -ne "C9C45BB4BEB556CB5AE25EBDE489416A")
        {
            Copy-Item -Path "$(Get-Location)\libraries\Debug\mono-2.0-bdwgc.dll" -Destination "$mono" -Force
        }
    }
}

if($Target.Equals("Release")) 
{
    Write-Host "Packaging for ThunderStore..."
    $Package="Package"
    $PackagePath="$ProjectPath\$Package"

    
    if(-Not(Test-Path "$PackagePath\plugins"))
    {
        New-Item -ItemType Directory "$PackagePath\plugins"
    }

    Copy-Item -Path "$TargetPath\$TargetAssembly" -Destination "$PackagePath\plugins\$TargetAssembly" -Force
    Copy-Item -Path "$ProjectPath\README.md" -Destination "$PackagePath\README.md" -Force

    # Saves the published package in the solution root
    $rootDirectory = (get-item $ProjectPath ).parent.FullName
    if(-Not(Test-Path "$rootDirectory\publish"))
    {
        New-Item -ItemType Directory "$rootDirectory\publish"
    }
    Compress-Archive -Path "$PackagePath\*" -DestinationPath "$rootDirectory\publish\$name.zip" -Force
    Write-Host "Done..."
}

# Pop Location
Pop-Location