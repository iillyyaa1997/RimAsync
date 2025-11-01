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
    /// </summary>
    public class RimAsyncGameComponent : RimWorld.GameComponent
    {
        private int _tickCounter = 0;
        private const int PERFORMANCE_UPDATE_INTERVAL = 60; // Update every 60 ticks (1 second at 60 TPS)

        public RimAsyncGameComponent(Game game) : base(game)
        {
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
