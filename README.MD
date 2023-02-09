# Valheim Mods

Several mods for the game Valheim, by me.

Mods can be found on Valheim [ThunderStore](https://valheim.thunderstore.io/package/tpill90/)

## Development Environment Setup

How to setup the development enviroment for this project:

1. Install [Visual Studio 2022](https://visualstudio.microsoft.com) and add the C# workload.
2. Update `Environment.props` in the projects base path, and update the property `VALHEIM_INSTALL` to point to the path of your installed copy of Valheim.
3. Install the required dependencies by running the script `./scripts/Download-Dependencies.ps1` in Powershell
4. Open `ValheimMods.sln` with Visual Studio, and `build` the project at least once.
5. Close Visual Studio, and reopen `ValheimMods.sln`.  For whatever reason Visual Studio will not detect the publicized assemblies unless this is done.

## Post Build automations

Included in this repo is a PowerShell script `publish.ps1`. The script is referenced in the project file as a post-build event. Depending on the chosen configuration in Visual Studio the script executes the following actions.

## Building Debug

* The compiled dll file for this project is copied to `<ValheimDir>\BepInEx\plugins`.
* A .mdb file is generated for the compiled project dll and copied to `<ValheimDir>\BepInEx\plugins`.
* `<JotunnModStub>\libraries\Debug\mono-2.0-bdwgc.dll` is copied to `<ValheimDir>\MonoBleedingEdge\EmbedRuntime` replacing the original file (a backup is created before).

## Debugging with Visual Studio

Your own code can be debugged in source with Visual Studio itself. You cannot debug game disassemblies as with dnSpy, though.

1. Install Visual Studio Tools for Unity (can be done in Visual Studio installer via `Individual Components` -> `Visual Studio Tools for Unity`)
3. Build the project with target `Debug`. The publish.ps1 PowerShell script from this repo...
   * copies the generated mod `.dll` and `.pdb` to \<ValheimDir>\BepInEx\plugins after a successful build
   * automatically generates a `.dll.mdb` file, which is needed for Unity/mono debugging. It should be in \<ValheimDir>\BepInEx\plugins, too.
   * copies the patched mono-2.0-bdwgc.dll from .\libraries\Debug to \<ValheimDir>\MonoBleedingEdge\EmbedRuntime and makes a backup copy of the original.
4. Start Valheim (either directly from Steam or hit F5 in Visual Studio when Steam is running)
5. Go to `Debug` -> `Attach Unity debugger`
6. Since the patched mono dll does not open the same port as Unity Dev builds, you have to click on `Input IP`. It should fill in your local IP automatically. you just have to change the port to `55555`and the debugger should connect.

## Building Release

* A compressed file with the binaries is created in `<JotunnModStub>\Packages`ready for upload to ThunderStore. Dont forget to include your information in the manifest.json and to change the project's readme file.

## Unity Editor Setup

1. [Download](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe) UnityHub directly from Unity or install it with the Visual Studio Installer via `Individual Components` -> `Visual Studio Tools for Unity`.
2. You will need an Unity account to register your PC and get a free licence. Create the account, login with it in Unity Hub and get your licence via `Settings` -> `Licence Management`.
3. Install Unity Editor version v2020.3.44f1.
4. Copy all `assembly_*.dll` from `<ValheimDir>\valheim_Data\Managed` into `<JotunnModStub>\JotunnModUnity\Assets\Assemblies`. **Do this directly in the filesystem - don't open Unity first or import the dlls directly in Unity**.
5. **Warning:** These assembly files are copyrighted material and you can theoretically get into trouble when you distribute them in your github repository. To avoid that there is a .gitignore file in the Unity project folder. Keep that when you clone or copy this repository.

## Debugging with dnSpy

Thanks to mono and unity-mono being open source, we patched and compiled our own mono runtime to enable actual live debugging of the game and the mod itself with dnSpy.

1. Download [dnSpy-net-win64](https://github.com/dnSpy/dnSpy/releases) and extract the exe.
2. Load all assemblies from `<ValheimDir>\unstripped_corlib` into dnSpy (just drag&drop the folder onto it).
3. Load all `assembly_*` from `<ValheimDir>\valheim_Data\Managed` into dnSpy (*do not load the publicized ones, they will not be loaded into the process and therefore can not be debugged*).
4. Load `JotunnModStub.dll` from `<ValheimDir>\BepInEx\plugins` into dnSpy.
5. Copy `<JotunnModStub>\libraries\Debug\mono-2.0-bdwgc.dll` into `<ValheimDir>\MonoBleedingEdge\EmbedRuntime` and overwrite the existing file.
6. Now go to `Debug` -> `Start Debugging` and select Unity debug engine. Select your valheim.exe as the executable and hit OK.
7. If you did set some breakpoints, the game will halt when it hits the breakpoint in memory and dnSpy will show you the objects in memory and much more useful stuff.



## Mod Development Resources

| Description | Link |
| --- | --- |
| Template used to create this project| https://github.com/Valheim-Modding/JotunnModStub |
| Jötunn's in depth setup guide | https://valheim-modding.github.io/Jotunn/guides/overview.html |
| Jötunn Mod Library Tutorials | https://valheim-modding.github.io/Jotunn/tutorials/overview.html |
| Logging with BepinEx | https://docs.bepinex.dev/v5.4.11/articles/dev_guide/plugin_tutorial/4_logging.html |
| Helpful development mod, launches game automatically and picks world | https://valheim.thunderstore.io/package/abearcodes/Simple_Auto_Game_Start/ |
