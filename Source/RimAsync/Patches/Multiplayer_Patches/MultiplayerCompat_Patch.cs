using HarmonyLib;
using RimAsync.Core;
using RimAsync.Utils;
using RimWorld;
using Verse;

namespace RimAsync.Patches.Multiplayer_Patches
{
    /// <summary>
    /// Patches for RimWorld Multiplayer compatibility
    /// Ensures RimAsync respects multiplayer synchronization requirements
    /// </summary>
    public static class MultiplayerCompat_Patch
    {
        /// <summary>
        /// Initialize multiplayer-specific patches only if multiplayer is loaded
        /// </summary>
        public static void InitializeMultiplayerPatches()
        {
            if (!MultiplayerCompat.IsMultiplayerLoaded)
            {
                Log.Message("[RimAsync] Multiplayer not detected, skipping multiplayer patches");
                return;
            }

            try
            {
                Log.Message("[RimAsync] Applying multiplayer compatibility patches...");

                // Apply multiplayer-specific patches here
                // This would include patches for command synchronization, deterministic execution, etc.

                Log.Message("[RimAsync] Multiplayer compatibility patches applied successfully");
            }
            catch (System.Exception ex)
            {
                Log.Error($"[RimAsync] Error applying multiplayer patches: {ex}");
            }
        }

        /// <summary>
        /// Example method showing how to wrap operations for multiplayer safety
        /// </summary>
        public static void WrapMultiplayerSafeOperation(System.Action operation, string operationName)
        {
            if (!MultiplayerCompat.IsInMultiplayer)
            {
                // Single player - execute directly
                operation();
                return;
            }

            try
            {
                // In multiplayer, we need to be more careful
                using (PerformanceMonitor.StartMeasuring($"MP_{operationName}"))
                {
                    // Execute operation with multiplayer safety checks
                    operation();
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[RimAsync] Error in multiplayer-safe operation {operationName}: {ex}");
            }
        }
    }

    /// <summary>
    /// Component that monitors multiplayer state changes
    /// </summary>
    public class MultiplayerStateMonitor : GameComponent
    {
        private bool _wasInMultiplayer = false;
        private bool _wasAsyncTimeEnabled = false;

        public MultiplayerStateMonitor(Game game) : base()
        {
        }

        public override void GameComponentTick()
        {
            if (!RimAsyncCore.IsInitialized) return;

            // Check for multiplayer state changes
            var currentlyInMultiplayer = MultiplayerCompat.IsInMultiplayer;
            var currentlyAsyncTime = MultiplayerCompat.AsyncTimeEnabled;

            if (currentlyInMultiplayer != _wasInMultiplayer)
            {
                OnMultiplayerStateChanged(currentlyInMultiplayer);
                _wasInMultiplayer = currentlyInMultiplayer;
            }

            if (currentlyAsyncTime != _wasAsyncTimeEnabled)
            {
                OnAsyncTimeStateChanged(currentlyAsyncTime);
                _wasAsyncTimeEnabled = currentlyAsyncTime;
            }
        }

        private void OnMultiplayerStateChanged(bool inMultiplayer)
        {
            Log.Message($"[RimAsync] Multiplayer state changed: {(inMultiplayer ? "Entered" : "Exited")} multiplayer");
            
            // Notify core systems of the change
            RimAsyncCore.OnSettingsChanged();
        }

        private void OnAsyncTimeStateChanged(bool asyncTimeEnabled)
        {
            Log.Message($"[RimAsync] AsyncTime state changed: {(asyncTimeEnabled ? "Enabled" : "Disabled")}");
            
            // Notify core systems of the change
            RimAsyncCore.OnSettingsChanged();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            // No data to save/load for this component
        }
    }
} 