#TODO cleanup
if(Test-Path "C:\Users\Tim\Desktop\Steam\steamapps\common\Valheim\_MonoProfiler64.dll")
{
    Get-ChildItem "C:\Users\Tim\Desktop\Steam\steamapps\common\Valheim\_MonoProfiler64.dll" | Rename-Item -NewName { $_.Name.Replace("_", "") }
}
else
{
    Get-ChildItem "C:\Users\Tim\Desktop\Steam\steamapps\common\Valheim\MonoProfiler64.dll" | Rename-Item -NewName { "_" + $_.Name }
}