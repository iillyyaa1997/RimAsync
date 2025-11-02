using System;
using HarmonyLib;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimAsync.Components
{
    /// <summary>
    /// Game component responsible for managing RimAsync state across save/load cycles
    /// Provides debug overlay and performance monitoring
    ///
    /// RimWorld 1.6 API: Verse.GameComponent with parameterless constructor
    /// </summary>
    public class RimAsyncGameComponent : Verse.GameComponent
    {
        private int _tickCounter = 0;
        private const int PERFORMANCE_UPDATE_INTERVAL = 60; // Update every 60 ticks (1 second at 60 TPS)

        // RimWorld 1.6 API Change: GameComponent constructor is now parameterless
        // Game instance available via Current.Game
        public RimAsyncGameComponent() : base()
        {
            Log.Message("[RimAsync] GameComponent constructor entered (RimWorld 1.6 API)");
            // Initialize core systems here, AFTER all mods and Defs are fully loaded
            // This is safe because GameComponent constructor runs when creating a new game
            // or loading an existing one, after all initialization is complete
            if (!RimAsyncCore.IsInitialized)
            {
                try
                {
                    Log.Message("[RimAsync] Initializing core systems from GameComponent...");
                    RimAsyncLogger.Info("Initializing RimAsync core systems from GameComponent", "GameComponent");
                    RimAsyncCore.Initialize();
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error("Failed to initialize core systems from GameComponent", ex, "GameComponent");
                }
            }
        }

        public override void GameComponentTick()
        {
            if (!RimAsyncCore.IsInitialized) return;

            try
            {
                _tickCounter++;

                // Update performance monitoring every second
                if (_tickCounter % PERFORMANCE_UPDATE_INTERVAL == 0)
                {
                    PerformanceMonitor.UpdateMetrics();
                }

                // Log status periodically for debugging (if enabled)
                if (RimAsyncMod.Settings?.enableDebugLogging == true && _tickCounter % (PERFORMANCE_UPDATE_INTERVAL * 10) == 0)
                {
                    LogDebugStatus();
                }
            }
            catch (System.Exception ex)
            {
                Log.Error($"[RimAsync] Error in GameComponent tick: {ex}");
            }
        }

        public override void GameComponentOnGUI()
        {
            // Draw debug overlay if enabled
            RimAsync.Utils.DebugOverlay.OnGUI();
        }

        public override void GameComponentUpdate()
        {
            // Handle debug overlay toggle (F11)
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F11))
            {
                RimAsync.Utils.DebugOverlay.Toggle();
            }
        }

        private void LogDebugStatus()
        {
            // Use DebugOverlay for status logging
            RimAsync.Utils.DebugOverlay.LogStatus();
        }
    }
}
