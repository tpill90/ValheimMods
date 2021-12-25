using System.Reflection;
using BepInEx;
using HarmonyLib;
using Pipakin.SkillInjectorMod;

namespace FarmingSkill
{
    [BepInPlugin("tpill90.FarmingSkill", "FarmingSkill", "0.0.5")]
    [BepInDependency("com.pipakin.SkillInjectorMod")]
    public class FarmingSkill : BaseUnityPlugin
    {
        public static int SKILL_TYPE = 287;

        void Awake()
        {
            Logger.LogInfo("Loading FarmingSkill Mod...");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

            SkillInjector.RegisterNewSkill(SKILL_TYPE, "Farming", "Affects cultivator stamina use", 1.0f, 
                TextureLoader.LoadCustomTexture("FarmingSkill.cultivator.png"), Skills.SkillType.Run);
        }
    }

    [HarmonyPatch]
    public static class FarmingSkillPatches
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
            if (usedItemName != "$item_cultivator")
            {
                return;
            }

            var playerIsRested = __instance.m_seman.HaveStatusEffect("Rested");
            if (playerIsRested)
            {
                // Being rested grants a 50% XP bonus
                __instance.RaiseSkill((Skills.SkillType)FarmingSkill.SKILL_TYPE, 1.5f);
            }
            else
            {
                __instance.RaiseSkill((Skills.SkillType)FarmingSkill.SKILL_TYPE, 1);
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
            if (usedItemName != "$item_cultivator")
            {
                return;
            }

            // Player has likely installed the mod, but not placed any plants yet, so the skill has not been initialized
            if (!__instance.GetSkills().m_skillData.ContainsKey((Skills.SkillType)FarmingSkill.SKILL_TYPE))
            {
                return;
            }

            var currentSkill = __instance.GetSkills().m_skillData[(Skills.SkillType)FarmingSkill.SKILL_TYPE];
            // Reduces stamina by a maximum of 66% at level 100
            v = v * (1 - (currentSkill.m_level * ((2f / 3f) / 100)));
        }
    }
}