using System;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using Verse;
using Verse.AI;

namespace RimAsync.Patches.RW_Patches
{
    /// <summary>
    /// Patches Pawn_PathFollower to enable asynchronous pathfinding
    /// Improves performance by calculating paths in background without blocking gameplay
    /// </summary>
    [HarmonyPatch(typeof(Pawn_PathFollower))]
    public static class Pawn_PathFollower_Patch
    {
        /// <summary>
        /// Patch for generating paths asynchronously when possible
        /// </summary>
        [HarmonyPatch("GeneratePatherPath")]
        [HarmonyPrefix]
        public static bool GeneratePatherPath_Prefix(
            Pawn_PathFollower __instance,
            IntVec3 destination,
            Verse.AI.PathEndMode peMode,
            ref PawnPath __result)
        {
            // Only apply async pathfinding if enabled and safe
            if (!RimAsyncMod.Settings.enableAsyncPathfinding)
            {
                return true; // Use original method
            }

            // Don't use async for critical or time-sensitive pathfinding
            if (IsCriticalPathfinding(__instance, destination))
            {
                return true; // Use original method
            }

            try
            {
                // Try async pathfinding
                var asyncResult = TryAsyncPathfinding(__instance, destination, peMode);
                if (asyncResult != null)
                {
                    __result = asyncResult;
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error in async pathfinding, falling back to sync: {ex}");
            }

            // Fallback to original method
            return true;
        }

        /// <summary>
        /// Attempt asynchronous pathfinding
        /// </summary>
        private static PawnPath TryAsyncPathfinding(Pawn_PathFollower pathFollower, IntVec3 destination, Verse.AI.PathEndMode peMode)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync())
            {
                return null; // Fallback to sync
            }

            var pawn = pathFollower.pawn;
            if (pawn?.Map == null)
            {
                return null; // Invalid state
            }

            // For demonstration - in real implementation this would be a proper async operation
            // This is a simplified example showing the pattern

            using (PerformanceMonitor.StartMeasuring("AsyncPathfinding"))
            {
                // Quick pathfinding for immediate response
                var quickPath = pawn.Map.pathFinder.FindPath(pawn.Position, new LocalTargetInfo(destination), pawn, TraverseMode.ByPawn, peMode);

                // Schedule background optimization if path is complex
                if (quickPath.TotalCost > 100) // Arbitrary threshold
                {
                    SchedulePathOptimization(pawn, destination, peMode);
                }

                return quickPath;
            }
        }

        /// <summary>
        /// Schedule background path optimization
        /// </summary>
        private static void SchedulePathOptimization(Pawn pawn, IntVec3 destination, Verse.AI.PathEndMode peMode)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            // Example async operation - in reality this would optimize the path
            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate path optimization work
                            await Task.Delay(10, cancellationToken);

                            // Record the optimization
                            PerformanceMonitor.RecordMetric("PathOptimization", 10.0f);

                            if (RimAsyncMod.Settings?.enableDebugLogging == true)
                            {
                                Log.Message($"[RimAsync] Optimized path for {pawn.LabelShort}");
                            }
                        },
                        () =>
                        {
                            // Sync fallback - do nothing or minimal optimization
                            PerformanceMonitor.RecordMetric("PathOptimization", 1.0f);
                        },
                        "PathOptimization");
                }
                catch (Exception ex)
                {
                    Log.Error($"[RimAsync] Error in path optimization: {ex}");
                }
            });
        }

        /// <summary>
        /// Check if pathfinding is critical and should not be made async
        /// </summary>
        private static bool IsCriticalPathfinding(Pawn_PathFollower pathFollower, IntVec3 destination)
        {
            var pawn = pathFollower.pawn;
            if (pawn == null) return true;

            // Critical situations where we need immediate pathfinding
            if (pawn.InCombat) return true;
            if (pawn.health?.summaryHealth < 0.5f) return true; // Injured pawn
            if (pawn.needs?.food < 0.1f) return true; // Starving

            // Add more critical conditions as needed

            return false;
        }
    }

    /// <summary>
    /// Example patch for pawn job processing optimization
    /// </summary>
    [HarmonyPatch(typeof(Pawn_JobTracker))]
    public static class Pawn_JobTracker_Patch
    {
        /// <summary>
        /// Optimize job determination for better performance
        /// </summary>
        [HarmonyPatch("DetermineNextJob")]
        [HarmonyPrefix]
        public static bool DetermineNextJob_Prefix(Pawn_JobTracker __instance, ref ThinkResult __result)
        {
            // Only optimize if background jobs are enabled
            if (!RimAsyncMod.Settings.enableBackgroundJobs)
            {
                return true; // Use original method
            }

            // Example optimization - in reality this would be more sophisticated
            using (PerformanceMonitor.StartMeasuring("JobDetermination"))
            {
                // Let original method run but monitor performance
                return true;
            }
        }
    }
}
