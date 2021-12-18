using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Pipakin.SkillInjectorMod;
using UnityEngine;

namespace FarmingSkill
{
    [BepInPlugin("tpill90.FarmingSkill", "FarmingSkill", "0.0.1")]
    [BepInDependency("com.pipakin.SkillInjectorMod")]
    public class FarmingSkill : BaseUnityPlugin
    {
        const string MOD_ID = "tpill90.FarmingSkill";
        private readonly Harmony harmony = new Harmony(MOD_ID);

        // Hopefully this doesn't conflict.
        public const int SKILL_TYPE = 287;

        void Awake()
        {
            Debug.Log("Loading Farming Skill Mod...");

            harmony.PatchAll(typeof(ReduceCultivatorStaminaCost));
            harmony.PatchAll(typeof(PlacePiece));

            SkillInjector.RegisterNewSkill(SKILL_TYPE, "Farming", "Affects cultivator stamina use", 1.0f, 
                TextureLoader.LoadCustomTexture("FarmingSkill.cultivator.png"), Skills.SkillType.Run);
        }

        

        [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
        public static class PlacePiece
        {
            [HarmonyPostfix]
            public static void PostFix(Player __instance, ref bool __result, Piece piece)
            {
                var successful = __result;
                if (!successful)
                {
                    return;
                }

                var plant = piece.gameObject.GetComponent<Plant>();
                if (!plant)
                {
                    return;
                }

                __instance.RaiseSkill((Skills.SkillType)SKILL_TYPE, 1);
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