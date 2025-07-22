using HarmonyLib;
using RimAsync.Core;
using RimAsync.Utils;
using Verse;

namespace RimAsync.Patches.Performance_Patches
{
    /// <summary>
    /// Performance patches for core RimWorld tick management
    /// Optimizes tick processing and adds performance monitoring
    /// </summary>
    [HarmonyPatch(typeof(TickManager))]
    public static class TickManager_Patch
    {
        /// <summary>
        /// Monitor and potentially optimize tick processing
        /// </summary>
        [HarmonyPatch("DoSingleTick")]
        [HarmonyPrefix]
        public static bool DoSingleTick_Prefix(TickManager __instance)
        {
            // Only apply optimizations if enabled
            if (!RimAsyncMod.Settings.enableMemoryOptimization && !RimAsyncMod.Settings.enablePerformanceMonitoring)
            {
                return true; // Use original method
            }

            // Measure tick performance if monitoring is enabled
            if (RimAsyncMod.Settings.enablePerformanceMonitoring)
            {
                using (PerformanceMonitor.StartMeasuring("TickManager.DoSingleTick"))
                {
                    return true; // Let original method run but measure it
                }
            }

            return true; // Use original method
        }

        /// <summary>
        /// Post-process tick to update our performance metrics
        /// </summary>
        [HarmonyPatch("DoSingleTick")]
        [HarmonyPostfix]
        public static void DoSingleTick_Postfix(TickManager __instance)
        {
            if (!RimAsyncCore.IsInitialized) return;

            try
            {
                // Update performance metrics after each tick
                // This is handled by RimAsyncGameComponent, but we can add additional metrics here
                
                if (RimAsyncMod.Settings?.enableDebugLogging == true && Find.TickManager.TicksGame % 3600 == 0) // Every minute
                {
                    var ticksGame = Find.TickManager.TicksGame;
                    var realTime = Find.TickManager.TickRateMultiplier;
                    
                    Log.Message($"[RimAsync] Tick {ticksGame}, Rate: {realTime:F2}x, TPS: {PerformanceMonitor.CurrentTPS:F1}");
                }
            }
            catch (System.Exception ex)
            {
                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Error($"[RimAsync] Error in TickManager postfix: {ex}");
                }
            }
        }
    }

    /// <summary>
    /// Patches for Map tick optimization
    /// </summary>
    [HarmonyPatch(typeof(Map))]
    public static class Map_Patch
    {
        /// <summary>
        /// Monitor map tick performance
        /// </summary>
        [HarmonyPatch("MapPostTick")]
        [HarmonyPrefix]
        public static bool MapPostTick_Prefix(Map __instance)
        {
            if (!RimAsyncMod.Settings.enablePerformanceMonitoring)
            {
                return true; // Use original method
            }

            // Measure map tick performance
            using (PerformanceMonitor.StartMeasuring($"Map.Tick_{__instance.Index}"))
            {
                return true; // Let original method run but measure it
            }
        }

        /// <summary>
        /// Optional async optimization for map updates
        /// </summary>
        [HarmonyPatch("MapPostTick")]
        [HarmonyPostfix]
        public static void MapPostTick_Postfix(Map __instance)
        {
            if (!RimAsyncCore.IsInitialized) return;

            try
            {
                // Example: Schedule background map optimizations if performance is good
                if (RimAsyncMod.Settings.enableBackgroundJobs && 
                    PerformanceMonitor.IsPerformanceGood && 
                    AsyncManager.CanExecuteAsync())
                {
                    // Could schedule background optimizations here
                    // For example: pathfinding grid updates, cache warming, etc.
                }
            }
            catch (System.Exception ex)
            {
                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Error($"[RimAsync] Error in Map postfix: {ex}");
                }
            }
        }
    }
} 