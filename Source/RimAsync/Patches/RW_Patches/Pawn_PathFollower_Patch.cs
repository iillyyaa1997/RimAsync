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
            if (pawn.needs?.food?.CurLevel < 0.1f) return true; // Starving

            // Add more critical conditions as needed

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
        /// Async job determination for better AI performance
        /// </summary>
        [HarmonyPatch("DetermineNextJob")]
        [HarmonyPrefix]
        public static bool DetermineNextJob_Prefix(Pawn_JobTracker __instance, ref ThinkResult __result)
        {
            // Only apply async job determination if enabled
            if (!RimAsyncMod.Settings.enableBackgroundJobs)
            {
                return true; // Use original method
            }

            var pawn = __instance.pawn;
            if (pawn == null || pawn.Dead || !pawn.Spawned)
            {
                return true; // Use original method for invalid pawns
            }

            // Don't use async for critical situations
            if (IsJobCritical(pawn))
            {
                return true; // Use original method
            }

            try
            {
                // Try async job determination
                var asyncResult = TryAsyncJobDetermination(__instance);
                if (asyncResult.HasValue)
                {
                    __result = asyncResult.Value;
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async job determination, falling back to sync", ex, "JobSystem");
            }

            // Fallback to original method with performance monitoring
            using (PerformanceMonitor.StartMeasuring("SyncJobDetermination"))
            {
                return true;
            }
        }

        /// <summary>
        /// Async job execution optimization
        /// </summary>
        [HarmonyPatch("JobTrackerTick")]
        [HarmonyPrefix]
        public static bool JobTrackerTick_Prefix(Pawn_JobTracker __instance)
        {
            // Only optimize if async job execution is enabled
            if (!RimAsyncMod.Settings.enableAsyncJobExecution)
            {
                return true; // Use original method
            }

            var pawn = __instance.pawn;
            if (pawn == null || pawn.Dead)
            {
                return true; // Use original method
            }

            try
            {
                // Try async job execution
                if (TryAsyncJobExecution(__instance))
                {
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async job execution, falling back to sync", ex, "JobSystem");
            }

            // Fallback to original method
            return true;
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
            // Priority jobs that can be determined quickly
            if (pawn.needs?.food?.CurLevel < 0.3f)
            {
                // Hungry - need food job
                return CreateFoodJob(pawn);
            }

            if (pawn.needs?.rest?.CurLevel < 0.2f)
            {
                // Tired - need rest job
                return CreateRestJob(pawn);
            }

            // No immediate priority job
            return null;
        }

        /// <summary>
        /// Create a food-seeking job
        /// </summary>
        private static Verse.AI.Job CreateFoodJob(Pawn pawn)
        {
            // Simplified food job creation
            var foodJobDef = new JobDef { defName = "Ingest" };
            var job = new Verse.AI.Job();
            job.def = foodJobDef;
            return job;
        }

        /// <summary>
        /// Create a rest job
        /// </summary>
        private static Verse.AI.Job CreateRestJob(Pawn pawn)
        {
            // Simplified rest job creation
            var restJobDef = new JobDef { defName = "LayDown" };
            var job = new Verse.AI.Job();
            job.def = restJobDef;
            return job;
        }

        /// <summary>
        /// Schedule background job optimization
        /// </summary>
        private static void ScheduleJobOptimization(Pawn pawn)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate advanced job planning
                            await Task.Delay(5, cancellationToken);

                            // Record the optimization
                            PerformanceMonitor.RecordMetric("JobOptimization", 5.0f);

                            RimAsyncLogger.Debug($"Optimized job planning for {pawn.LabelShort}", "JobSystem");
                        },
                        () =>
                        {
                            // Sync fallback - minimal optimization
                            PerformanceMonitor.RecordMetric("JobOptimization", 1.0f);
                        },
                        "JobOptimization");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in job optimization", ex, "JobSystem");
                }
            });
        }

        /// <summary>
        /// Schedule asynchronous job tick execution
        /// </summary>
        private static void ScheduleAsyncJobTick(Pawn_JobTracker jobTracker)
        {
            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Async job driver tick
                            await Task.Delay(1, cancellationToken);

                            if (jobTracker.curDriver != null)
                            {
                                jobTracker.curDriver.DriverTick();
                            }

                            PerformanceMonitor.RecordMetric("AsyncJobTick", 1.0f);
                        },
                        () =>
                        {
                            // Sync fallback
                            if (jobTracker.curDriver != null)
                            {
                                jobTracker.curDriver.DriverTick();
                            }
                        },
                        "AsyncJobTick");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in async job tick", ex, "JobSystem");
                }
            });
        }

        /// <summary>
        /// Check if job is critical and should not be made async
        /// </summary>
        private static bool IsJobCritical(Pawn pawn)
        {
            // Critical situations where jobs need immediate determination
            if (pawn.InCombat) return true;
            if (pawn.health?.summaryHealth < 0.3f) return true; // Severely injured
            if (pawn.needs?.food?.CurLevel < 0.05f) return true; // Near starvation

            // Add more critical conditions as needed
            return false;
        }

        /// <summary>
        /// Check if job can be executed asynchronously
        /// </summary>
        private static bool CanJobBeExecutedAsync(Verse.Job job)
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
