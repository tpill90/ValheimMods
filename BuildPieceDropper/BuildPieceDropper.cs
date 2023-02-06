using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BuildPieceDropper
{
    [BepInPlugin("tpill90.BuildPieceDropper", "BuildPieceDropper", "0.0.0")]
    public class BuildPieceDropper : BaseUnityPlugin
    {
        public static KeyboardShortcut pipetteShortcut = new KeyboardShortcut(KeyCode.Q);

        public static ManualLogSource UnityLogger;

        void Awake()
        {
            UnityLogger = Logger;
            UnityLogger.LogInfo("Loading BuildPieceDropper Mod...");

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(Hud), "UpdateCrosshair")]
        static class UpdateCrosshair_Patch
        {
            static void Postfix(Hud __instance, Player player)
            {
                if (player.GetRightItem() == null)
                {
                    return;
                }

                string usedItemName = player.GetRightItem().m_shared.m_name;
                if (usedItemName != "$item_hammer")
                {
                    return;
                }

                if (pipetteShortcut.IsDown())
                {
                    Piece hoveringPiece = player.GetHoveringPiece();
                    if (hoveringPiece)
                    {
                        UnityLogger.LogDebug(hoveringPiece.name);

                        WearNTear wnt = hoveringPiece.GetComponent<WearNTear>();
                        ZNetView znv = hoveringPiece.GetComponent<ZNetView>();
                        
                    }

                }
            }
        }
    }
}