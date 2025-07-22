using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using RimWorld;
using Verse;

namespace RimAsync.Components
{
    /// <summary>
    /// Main game component for RimAsync
    /// Handles per-game lifecycle and coordinates systems during gameplay
    /// </summary>
    public class RimAsyncGameComponent : GameComponent
    {
        private int _tickCounter = 0;
        private const int PERFORMANCE_UPDATE_INTERVAL = 60; // Update every 60 ticks (1 second at 60 TPS)

        public RimAsyncGameComponent(Game game) : base()
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
            // Optional: Draw performance overlay if debug is enabled
            if (RimAsyncMod.Settings?.enableDebugLogging == true && RimAsyncMod.Settings?.enablePerformanceMonitoring == true)
            {
                DrawDebugOverlay();
            }
        }

        private void LogDebugStatus()
        {
            var performanceStatus = PerformanceMonitor.GetPerformanceSummary();
            var asyncStatus = AsyncManager.GetStatus();
            var multiplayerStatus = MultiplayerCompat.GetMultiplayerStatus();

            Log.Message($"[RimAsync] Status - {performanceStatus} | {asyncStatus} | {multiplayerStatus}");
        }

        private void DrawDebugOverlay()
        {
            // Simple performance display in top-left corner
            var rect = new UnityEngine.Rect(10, 10, 300, 100);
            
            UnityEngine.GUI.Box(rect, "");
            UnityEngine.GUILayout.BeginArea(rect);
            
            UnityEngine.GUILayout.Label($"RimAsync Debug");
            UnityEngine.GUILayout.Label($"TPS: {PerformanceMonitor.CurrentTPS:F1}");
            UnityEngine.GUILayout.Label($"Mode: {RimAsyncCore.GetExecutionMode()}");
            UnityEngine.GUILayout.Label($"Async Available: {AsyncManager.CanExecuteAsync()}");
            
            UnityEngine.GUILayout.EndArea();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            // No persistent data to save/load for this component
        }
    }
} 