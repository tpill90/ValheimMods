using BepInEx;
using HarmonyLib;
using Pipakin.SkillInjectorMod;

namespace FarmingSkill
{
    [BepInPlugin("tpill90.FarmingSkill", "FarmingSkill", "0.0.4")]
    [BepInDependency("com.pipakin.SkillInjectorMod")]
    public class FarmingSkill : BaseUnityPlugin
    {
        const string MOD_ID = "tpill90.FarmingSkill";
        private readonly Harmony harmony = new Harmony(MOD_ID);

        // Hopefully this doesn't conflict.
        public const int SKILL_TYPE = 287;

        void Awake()
        {
            Logger.LogInfo("Loading FarmingSkill Mod...");

            harmony.PatchAll(typeof(ReduceCultivatorStaminaCost));
            harmony.PatchAll(typeof(ConsumeResources));

            SkillInjector.RegisterNewSkill(SKILL_TYPE, "Farming", "Affects cultivator stamina use", 1.0f, 
                TextureLoader.LoadCustomTexture("FarmingSkill.cultivator.png"), Skills.SkillType.Run);
        }

        //TODO try to format these harmony patches differently, hard to read like this
        [HarmonyPatch(typeof(Player), nameof(Player.ConsumeResources))]
        public static class ConsumeResources
        {
            [HarmonyPostfix]
            public static void PostFix(Player __instance, Piece.Requirement[] requirements, int qualityLevel)
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
                    __instance.RaiseSkill((Skills.SkillType)SKILL_TYPE, 1.5f);
                }
                else
                {
                    __instance.RaiseSkill((Skills.SkillType)SKILL_TYPE, 1);
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UseStamina))]
        public static class ReduceCultivatorStaminaCost
        {
            [HarmonyPrefix]
            public static void Prefix(ref float v, Player __instance)
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
                if (!__instance.GetSkills().m_skillData.ContainsKey((Skills.SkillType) SKILL_TYPE))
                {
                    return;
                }

                var currentSkill = __instance.GetSkills().m_skillData[(Skills.SkillType)SKILL_TYPE];
                // Reduces stamina by a maximum of 66% at level 100
                v = v * (1 - (currentSkill.m_level * ((2f / 3f) / 100)));
            }
        }
    }
}