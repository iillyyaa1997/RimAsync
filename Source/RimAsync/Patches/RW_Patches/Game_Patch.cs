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
            if (!RimAsyncCore.IsInitialized) return;

            try
            {
                // Check if our component already exists
                var existingComponent = game.GetComponent<RimAsyncGameComponent>();
                if (existingComponent == null)
                {
                    // Add our component
                    var component = new RimAsyncGameComponent(game);
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

    /// <summary>
    /// Patches for World lifecycle
    /// </summary>
    [HarmonyPatch(typeof(World))]
    public static class World_Patch
    {
        /// <summary>
        /// Clear cache when world is finalized
        /// </summary>
        [HarmonyPatch("FinalizeInit")]
        [HarmonyPostfix]
        public static void FinalizeInit_Postfix()
        {
            try
            {
                // Clear any stale cache entries when world initializes
                Utils.SmartCache.ClearAll();

                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Message("[RimAsync] Cleared cache on world init");
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[RimAsync] Error during world init cleanup: {ex}");
            }
        }
    }
}
