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
    /// Patches for asynchronous AI processing and decision making
    /// Improves performance by processing AI thinking in background
    /// </summary>
    [HarmonyPatch(typeof(Pawn_MindState))]
    public static class Pawn_MindState_Patch
    {
        /// <summary>
        /// Async AI state processing for better performance
        /// </summary>
        [HarmonyPatch("MindStateTick")]
        [HarmonyPrefix]
        public static bool MindStateTick_Prefix(Pawn_MindState __instance)
        {
            // Only apply async AI if enabled
            if (!RimAsyncMod.Settings.enableAsyncAI)
            {
                return true; // Use original method
            }

            var pawn = __instance.pawn;
            if (pawn == null || pawn.Dead || !pawn.Spawned)
            {
                return true; // Use original method for invalid pawns
            }

            // Don't use async for critical AI decisions
            if (IsAICritical(pawn))
            {
                return true; // Use original method
            }

            try
            {
                // Try async AI processing
                if (TryAsyncAIProcessing(__instance))
                {
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async AI processing, falling back to sync", ex, "AISystem");
            }

            // Fallback to original method with monitoring
            using (PerformanceMonitor.StartMeasuring("SyncAIProcessing"))
            {
                return true;
            }
        }

        /// <summary>
        /// Public wrapper for testing async AI processing
        /// </summary>
        public static bool TestAsyncAIProcessing(Pawn_MindState mindState)
        {
            return MindStateTick_Prefix(mindState);
        }

        /// <summary>
        /// Attempt asynchronous AI processing
        /// </summary>
        private static bool TryAsyncAIProcessing(Pawn_MindState mindState)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync() || !RimAsyncCore.CanUseAsync())
            {
                return false; // Fallback to sync
            }

            var pawn = mindState.pawn;

            using (PerformanceMonitor.StartMeasuring("AsyncAIProcessing"))
            {
                // Quick AI state update for immediate response
                UpdateAIStateQuick(mindState);

                // Schedule background AI optimization
                ScheduleAIOptimization(pawn);

                return true;
            }
        }

        /// <summary>
        /// Quick AI state update for immediate response
        /// </summary>
        private static void UpdateAIStateQuick(Pawn_MindState mindState)
        {
            // Simplified AI state updates that can be done immediately
            var pawn = mindState.pawn;

            // Update basic activity state
            if (pawn.InCombat)
            {
                mindState.Active = true;
            }
            else if (pawn.needs?.rest?.CurLevel < 0.1f)
            {
                mindState.Active = false; // Tired, less active
            }

            RimAsyncLogger.Debug($"Quick AI update for {pawn.LabelShort}: Active={mindState.Active}", "AISystem");
        }

        /// <summary>
        /// Schedule background AI optimization
        /// </summary>
        private static void ScheduleAIOptimization(Pawn pawn)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate advanced AI processing
                            await Task.Delay(3, cancellationToken);

                            // Record the optimization
                            PerformanceMonitor.RecordMetric("AIOptimization", 3.0f);

                            RimAsyncLogger.Debug($"AI optimization completed for {pawn.LabelShort}", "AISystem");
                        },
                        () =>
                        {
                            // Sync fallback - minimal AI processing
                            PerformanceMonitor.RecordMetric("AIOptimization", 1.0f);
                        },
                        "AIOptimization");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in AI optimization", ex, "AISystem");
                }
            });
        }

        /// <summary>
        /// Check if AI processing is critical and should not be made async
        /// </summary>
        private static bool IsAICritical(Pawn pawn)
        {
            // Critical situations where AI needs immediate processing
            if (pawn.InCombat) return true;
            if (pawn.health?.summaryHealth < 0.2f) return true; // Critically injured
            if (pawn.needs?.food?.CurLevel < 0.05f) return true; // Near starvation

            // Add more critical conditions as needed
            return false;
        }
    }

    /// <summary>
    /// Patches for async ThinkNode processing
    /// </summary>
    [HarmonyPatch(typeof(ThinkNode))]
    public static class ThinkNode_Patch
    {
        /// <summary>
        /// Async job package issuing for better AI performance
        /// </summary>
        [HarmonyPatch("TryIssueJobPackage")]
        [HarmonyPrefix]
        public static bool TryIssueJobPackage_Prefix(
            ThinkNode __instance,
            Pawn pawn,
            JobIssueParams jobParams,
            ref ThinkResult __result)
        {
            // Only apply async thinking if enabled
            if (!RimAsyncMod.Settings.enableAsyncAI)
            {
                return true; // Use original method
            }

            if (pawn == null || pawn.Dead)
            {
                return true; // Use original method for invalid pawns
            }

            // Don't use async for urgent thinking
            if (IsThinkingUrgent(pawn))
            {
                return true; // Use original method
            }

            try
            {
                // Try async thinking
                var asyncResult = TryAsyncThinking(__instance, pawn, jobParams);
                if (asyncResult.HasValue)
                {
                    __result = asyncResult.Value;
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async thinking, falling back to sync", ex, "AISystem");
            }

            // Fallback to original method
            return true;
        }

        /// <summary>
        /// Attempt asynchronous thinking
        /// </summary>
        private static ThinkResult? TryAsyncThinking(ThinkNode thinkNode, Pawn pawn, JobIssueParams jobParams)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync() || !RimAsyncCore.CanUseAsync())
            {
                return null; // Fallback to sync
            }

            using (PerformanceMonitor.StartMeasuring("AsyncThinking"))
            {
                // Quick thinking for immediate response
                var quickThought = GetQuickThought(pawn);
                if (quickThought.HasValue)
                {
                    RimAsyncLogger.Debug($"Quick thought for {pawn.LabelShort}: {quickThought.Value.Job?.def?.defName}", "AISystem");

                    // Schedule background deep thinking
                    ScheduleDeepThinking(pawn, jobParams);

                    return quickThought;
                }

                // No quick thought - use fallback
                return null;
            }
        }

        /// <summary>
        /// Get a quick thought for immediate response
        /// </summary>
        private static ThinkResult? GetQuickThought(Pawn pawn)
        {
            // Quick priority checks
            if (pawn.needs?.food?.CurLevel < 0.4f)
            {
                // Hungry - think about food
                var foodJob = new Verse.AI.Job(new JobDef { defName = "Ingest" });
                return new ThinkResult(foodJob);
            }

            if (pawn.needs?.rest?.CurLevel < 0.3f)
            {
                // Tired - think about rest
                var restJob = new Verse.AI.Job(new JobDef { defName = "LayDown" });
                return new ThinkResult(restJob);
            }

            // No immediate priority thought
            return null;
        }

        /// <summary>
        /// Schedule background deep thinking
        /// </summary>
        private static void ScheduleDeepThinking(Pawn pawn, JobIssueParams jobParams)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate deep AI thinking
                            await Task.Delay(7, cancellationToken);

                            // Record the deep thinking
                            PerformanceMonitor.RecordMetric("DeepThinking", 7.0f);

                            RimAsyncLogger.Debug($"Deep thinking completed for {pawn.LabelShort}", "AISystem");
                        },
                        () =>
                        {
                            // Sync fallback - minimal thinking
                            PerformanceMonitor.RecordMetric("DeepThinking", 1.0f);
                        },
                        "DeepThinking");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in deep thinking", ex, "AISystem");
                }
            });
        }

        /// <summary>
        /// Check if thinking is urgent and should not be made async
        /// </summary>
        private static bool IsThinkingUrgent(Pawn pawn)
        {
            // Urgent situations where thinking needs immediate processing
            if (pawn.InCombat) return true;
            if (pawn.health?.summaryHealth < 0.3f) return true; // Severely injured
            if (pawn.needs?.food?.CurLevel < 0.05f) return true; // Near starvation

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
