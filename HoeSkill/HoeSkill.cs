using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Pipakin.SkillInjectorMod;

namespace HoeSkill
{
    [BepInPlugin("tpill90.HoeSkill", "HoeSkill", "0.0.3")]
    [BepInDependency("com.pipakin.SkillInjectorMod")]
    public class HoeSkill : BaseUnityPlugin
    {
        private static readonly int SKILL_ID = 288;
        public static readonly Skills.SkillType HoeSkillType = (Skills.SkillType)SKILL_ID;

        public static ManualLogSource UnityLogger;

        void Awake()
        {
            UnityLogger = Logger;
            UnityLogger.LogInfo("Loading HoeSkill Mod...");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            var skillIconTexture = TextureLoader.LoadCustomTexture("HoeSkill.hoe.png");
            SkillInjector.RegisterNewSkill(SKILL_ID, "Hoe", "Affects cultivator stamina use",
                1.0f, skillIconTexture, Skills.SkillType.Run);
        }
    }

    [HarmonyPatch]
    public static class HoeSkillPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), nameof(Player.ConsumeResources))]
        public static void IncreaseSkillExperience(Player __instance)
        {
            if (__instance.GetRightItem() == null)
            {
                return;
            }

            string usedItemName = __instance.GetRightItem().m_shared.m_name;
            if (usedItemName != "$item_hoe")
            {
                return;
            }

            // Increments total skill XP by 
            __instance.RaiseSkill(HoeSkill.HoeSkillType);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.UseStamina))]
        public static void ReduceCultivatorStaminaCost(ref float v, Player __instance)
        {
            if (__instance.GetRightItem() == null)
            {
                return;
            }

            string usedItemName = __instance.GetRightItem().m_shared.m_name;
            if (usedItemName != "$item_hoe")
            {
                return;
            }

            // Player has likely installed the mod, but has not yet used the hoe, so the skill has not been initialized
            if (!__instance.GetSkills().m_skillData.ContainsKey(HoeSkill.HoeSkillType))
            {
                return;
            }

            var currentSkill = __instance.GetSkills().m_skillData[HoeSkill.HoeSkillType];
            // Reduces stamina by a maximum of 80% at level 100
            v = v * (1 - (currentSkill.m_level * ((4f / 5f) / 100)));
        }
    }
}