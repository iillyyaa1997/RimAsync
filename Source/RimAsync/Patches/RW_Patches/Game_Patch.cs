using HarmonyLib;
using RimAsync.Components;
using RimAsync.Core;
using Verse;

namespace RimAsync.Patches.RW_Patches
{
    /// <summary>
    /// Patches for Game lifecycle to ensure RimAsyncGameComponent is properly initialized
    /// </summary>
    [HarmonyPatch(typeof(Game))]
    public static class Game_Patch
    {
        /// <summary>
        /// Ensure our GameComponent is added when game is loaded/created
        /// </summary>
        [HarmonyPatch("LoadGame")]
        [HarmonyPostfix]
        public static void LoadGame_Postfix(Game __instance)
        {
            EnsureGameComponentExists(__instance);
        }

        /// <summary>
        /// Ensure our GameComponent is added when new game is started
        /// </summary>
        [HarmonyPatch("InitNewGame")]
        [HarmonyPostfix]
        public static void InitNewGame_Postfix(Game __instance)
        {
            EnsureGameComponentExists(__instance);
        }

        /// <summary>
        /// Ensure RimAsyncGameComponent exists in the game
        /// </summary>
        private static void EnsureGameComponentExists(Game game)
        {
            try
            {
                // Check if our component already exists
                var existingComponent = game.GetComponent<RimAsyncGameComponent>();
                if (existingComponent == null)
                {
                    // RimWorld 1.6 API: GameComponent constructor is parameterless
                    var component = new RimAsyncGameComponent();
                    game.components.Add(component);

                    Log.Message("[RimAsync] Added RimAsyncGameComponent to game");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[RimAsync] Error ensuring GameComponent exists: {ex}");
            }
        }

        /// <summary>
        /// Clean up when game ends
        /// </summary>
        [HarmonyPatch("DeinitAndRemoveMap")]
        [HarmonyPostfix]
        public static void DeinitAndRemoveMap_Postfix()
        {
            try
            {
                // Clear smart cache when maps are removed
                Utils.SmartCache.ClearAll();

                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Message("[RimAsync] Cleaned up cache on map removal");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[RimAsync] Error during map cleanup: {ex}");
            }
        }
    }
}
