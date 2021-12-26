using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Pipakin.SkillInjectorMod;

namespace HoeSkill
{
    [BepInPlugin("tpill90.HoeSkill", "HoeSkill", "0.0.1")]
    [BepInDependency("com.pipakin.SkillInjectorMod")]
    public class HoeSkill : BaseUnityPlugin
    {
        public static int SKILL_TYPE = 288;

        public static ManualLogSource logger;

        void Awake()
        {
            logger = Logger;
            logger.LogInfo("Loading HoeSkill Mod...");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            SkillInjector.RegisterNewSkill(SKILL_TYPE, "Hoe", "Affects cultivator stamina use", 1.0f, 
                TextureLoader.LoadCustomTexture("HoeSkill.hoe.png"), Skills.SkillType.Run);
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

            var playerIsRested = __instance.m_seman.HaveStatusEffect("Rested");
            if (playerIsRested)
            {
                // Being rested grants a 50% XP bonus
                __instance.RaiseSkill((Skills.SkillType)HoeSkill.SKILL_TYPE, 1.5f);
            }
            else
            {
                __instance.RaiseSkill((Skills.SkillType)HoeSkill.SKILL_TYPE, 1);
            }
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
            if (!__instance.GetSkills().m_skillData.ContainsKey((Skills.SkillType)HoeSkill.SKILL_TYPE))
            {
                return;
            }

            var currentSkill = __instance.GetSkills().m_skillData[(Skills.SkillType)HoeSkill.SKILL_TYPE];
            // Reduces stamina by a maximum of 66% at level 100
            v = v * (1 - (currentSkill.m_level * ((2f / 3f) / 100)));
        }
    }
}