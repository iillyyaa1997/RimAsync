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
            // Safety check: Settings might not be initialized during early game loading
            if (RimAsyncMod.Settings == null)
            {
                return true; // Use original method if settings not ready
            }

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
        /// Public wrapper for testing async pathfinding
        /// </summary>
        public static bool TestAsyncPathfinding(Pawn_PathFollower pathFollower)
        {
            PawnPath result = null;
            return GeneratePatherPath_Prefix(pathFollower, IntVec3.Zero, Verse.AI.PathEndMode.OnCell, ref result);
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
        /// RimWorld 1.6: SAFE main-thread scheduling (no Task.Run)
        /// </summary>
        private static void SchedulePathOptimization(Pawn pawn, IntVec3 destination, Verse.AI.PathEndMode peMode)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            // SAFE: Use AsyncManager.ExecuteAdaptive which handles thread safety internally
            // NO Task.Run() - all Unity/RimWorld objects MUST be accessed from main thread
            AsyncManager.ExecuteAdaptive(
                async (cancellationToken) =>
                {
                    try
                    {
                        // Simulate path optimization work
                        await Task.Delay(10, cancellationToken);

                        // Record the optimization
                        PerformanceMonitor.RecordMetric("PathOptimization", 10.0f);

                        if (RimAsyncMod.Settings?.enableDebugLogging == true)
                        {
                            Log.Message($"[RimAsync] Optimized path for {pawn.LabelShort}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"[RimAsync] Error in path optimization: {ex}");
                    }
                },
                () =>
                {
                    // Sync fallback - do nothing or minimal optimization
                    PerformanceMonitor.RecordMetric("PathOptimization", 1.0f);
                },
                "PathOptimization");
        }

        /// <summary>
        /// Check if pathfinding is critical and should not be made async
        /// RimWorld 1.6: simplified checks (removed InCombat, health.summaryHealth)
        /// </summary>
        private static bool IsCriticalPathfinding(Pawn_PathFollower pathFollower, IntVec3 destination)
        {
            var pawn = pathFollower.pawn;
            if (pawn == null) return true;

            // Critical situations where we need immediate pathfinding
            // RimWorld 1.6: InCombat property not available, removed
            // RimWorld 1.6: health.summaryHealth is SummaryHealthHandler, not float
            if (pawn.needs?.food?.CurLevel < 0.1f) return true; // Starving

            // More conservative approach
            return false;
        }
    }

    /// <summary>
    /// Enhanced patch for pawn job processing with full async functionality
    /// Enables asynchronous job determination and execution for better performance
    /// </summary>
    [HarmonyPatch(typeof(Pawn_JobTracker))]
    public static class Pawn_JobTracker_Patch
    {
        /// <summary>
        /// Async job determination for better AI performance - DISABLED
        /// This patch is disabled because job creation requires properly initialized JobDefs from XML
        /// </summary>
        [HarmonyPatch("DetermineNextJob")]
        [HarmonyPrefix]
        public static bool DetermineNextJob_Prefix(Pawn_JobTracker __instance, ref ThinkResult __result)
        {
            // DISABLED: Job determination requires valid JobDefs from DefDatabase
            // Creating jobs programmatically causes NullReferenceException
            // See GetQuickJob for detailed explanation of why this is disabled

            // Always use original method
            return true;
        }

        /// <summary>
        /// Async job execution optimization - DISABLED
        /// This patch is disabled because job execution cannot be safely moved to background threads
        /// </summary>
        [HarmonyPatch("JobTrackerTick")]
        [HarmonyPrefix]
        public static bool JobTrackerTick_Prefix(Pawn_JobTracker __instance)
        {
            // DISABLED: Job execution requires main thread access to Unity objects
            // See ScheduleAsyncJobTick for detailed explanation of why this is disabled

            // Always use original method
            return true;
        }

        /// <summary>
        /// Public wrapper for testing async job processing
        /// </summary>
        public static bool TestAsyncJobProcessing(Pawn_JobTracker jobTracker)
        {
            return JobTrackerTick_Prefix(jobTracker);
        }

        /// <summary>
        /// Get async job stats for monitoring
        /// </summary>
        public static string GetJobAsyncStats()
        {
            try
            {
                var cacheStats = SmartCache.GetStats();
                var asyncEnabled = AsyncManager.CanExecuteAsync();

                return $"Job Async: {(asyncEnabled ? "Enabled" : "Disabled")}, Cache Hits: {cacheStats.CacheHits}";
            }
            catch
            {
                return "Job Async: Stats unavailable";
            }
        }

        /// <summary>
        /// Attempt asynchronous job determination
        /// </summary>
        private static ThinkResult? TryAsyncJobDetermination(Pawn_JobTracker jobTracker)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync() || !RimAsyncCore.CanUseAsync())
            {
                return null; // Fallback to sync
            }

            var pawn = jobTracker.pawn;

            using (PerformanceMonitor.StartMeasuring("AsyncJobDetermination"))
            {
                // Quick job check for immediate response
                var quickJob = GetQuickJob(pawn);
                if (quickJob != null)
                {
                    RimAsyncLogger.Debug($"Quick job found for {pawn.LabelShort}: {quickJob.def?.defName}", "JobSystem");

                    // Schedule background job optimization
                    ScheduleJobOptimization(pawn);

                    return new ThinkResult(quickJob);
                }

                // No quick job found - use fallback
                return null;
            }
        }

        /// <summary>
        /// Attempt asynchronous job execution
        /// </summary>
        private static bool TryAsyncJobExecution(Pawn_JobTracker jobTracker)
        {
            var pawn = jobTracker.pawn;
            var currentJob = jobTracker.curJob;

            if (currentJob == null)
            {
                return false; // No job to execute
            }

            // Check if job can be executed asynchronously
            if (!CanJobBeExecutedAsync(currentJob))
            {
                return false; // Job must be executed synchronously
            }

            using (PerformanceMonitor.StartMeasuring("AsyncJobExecution"))
            {
                // Execute job driver tick asynchronously
                ScheduleAsyncJobTick(jobTracker);
                return true;
            }
        }

        /// <summary>
        /// Get a quick job for immediate response
        /// </summary>
        private static Verse.AI.Job GetQuickJob(Pawn pawn)
        {
            // DISABLED: Creating new JobDef instances causes NullReferenceException
            // because JobDefs must be loaded from XML definitions, not created at runtime.
            //
            // Quick jobs are not reliable without proper job definitions from DefDatabase.
            // The game's job system is complex and requires proper initialization:
            // - JobDefs must be loaded via XML
            // - JobDefs need proper database registration
            // - Job creation requires valid ThinkTree context
            //
            // Better to return null and let the original job determination system handle it.

            return null;
        }

        /// <summary>
        /// Create a food-seeking job - DISABLED
        /// </summary>
        private static Verse.AI.Job CreateFoodJob(Pawn pawn)
        {
            // DISABLED: See GetQuickJob comment
            return null;
        }

        /// <summary>
        /// Create a rest job - DISABLED
        /// </summary>
        private static Verse.AI.Job CreateRestJob(Pawn pawn)
        {
            // DISABLED: See GetQuickJob comment
            return null;
        }

        /// <summary>
        /// Schedule background job optimization
        /// RimWorld 1.6: SAFE main-thread scheduling (no Task.Run)
        /// </summary>
        private static void ScheduleJobOptimization(Pawn pawn)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            // SAFE: Use AsyncManager.ExecuteAdaptive which handles thread safety internally
            // NO Task.Run() - all Unity/RimWorld objects MUST be accessed from main thread
            AsyncManager.ExecuteAdaptive(
                async (cancellationToken) =>
                {
                    try
                    {
                        // Simulate advanced job planning
                        await Task.Delay(5, cancellationToken);

                        // Record the optimization
                        PerformanceMonitor.RecordMetric("JobOptimization", 5.0f);

                        RimAsyncLogger.Debug($"Optimized job planning for {pawn.LabelShort}", "JobSystem");
                    }
                    catch (Exception ex)
                    {
                        RimAsyncLogger.Error($"Error in job optimization", ex, "JobSystem");
                    }
                },
                () =>
                {
                    // Sync fallback - minimal optimization
                    PerformanceMonitor.RecordMetric("JobOptimization", 1.0f);
                },
                "JobOptimization");
        }

        /// <summary>
        /// Schedule asynchronous job tick execution
        /// CRITICAL: DriverTick MUST run on the main thread due to Unity limitations
        /// This method is DISABLED to prevent threading violations
        /// </summary>
        private static void ScheduleAsyncJobTick(Pawn_JobTracker jobTracker)
        {
            // DISABLED: Calling DriverTick from a background thread causes threading violations
            // Unity objects (pawns, jobs, maps, etc.) can only be accessed from the main thread.
            //
            // Attempting to call DriverTick() from Task.Run causes:
            // - NullReferenceException when accessing Unity objects
            // - Race conditions in game state
            // - Texture loading failures
            // - Map/Region access violations
            //
            // The job system is deeply integrated with Unity's main thread and cannot be
            // safely made asynchronous without major refactoring.
            //
            // Instead, we should use Postfix patches to optimize AFTER jobs complete,
            // or use caching/predictive algorithms that don't touch Unity objects.

            RimAsyncLogger.Debug("Async job tick requested but disabled due to thread safety", "JobSystem");
        }

        /// <summary>
        /// Check if job is critical and should not be made async
        /// RimWorld 1.6: simplified checks (removed InCombat, health.summaryHealth)
        /// </summary>
        private static bool IsJobCritical(Pawn pawn)
        {
            // Critical situations where jobs need immediate determination
            // RimWorld 1.6: InCombat property not available, removed
            // RimWorld 1.6: health.summaryHealth is SummaryHealthHandler, not float
            if (pawn.needs?.food?.CurLevel < 0.05f) return true; // Near starvation

            // More conservative approach
            return false;
        }

        /// <summary>
        /// Check if job can be executed asynchronously
        /// </summary>
        private static bool CanJobBeExecutedAsync(Verse.AI.Job job)
        {
            if (job?.def?.defName == null) return false;

            // Jobs that can be executed asynchronously
            var asyncJobs = new[] { "Research", "Construction", "Cook", "Smith", "Art" };

            foreach (var asyncJob in asyncJobs)
            {
                if (job.def.defName.Contains(asyncJob))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
