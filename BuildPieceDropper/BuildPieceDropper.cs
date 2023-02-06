using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Tools;

namespace BuildPieceDropper
{
    [BepInPlugin("tpill90.BuildPieceDropper", "BuildPieceDropper", "0.0.0")]
    public class HoeSkill : BaseUnityPlugin
    {
        public static ManualLogSource UnityLogger;

        void Awake()
        {
            UnityLogger = Logger;
            UnityLogger.LogInfo("Loading BuildPieceDropper Mod...");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            
        }
    }

    [HarmonyPatch]
    public static class BuildPieceDropperPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), nameof(Player.ConsumeResources))]
        public static void IncreaseSkillExperience(Player __instance)
        {
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.UseStamina))]
        public static void ReduceCultivatorStaminaCost(ref float v, Player __instance)
        {
          
        }
    }
}