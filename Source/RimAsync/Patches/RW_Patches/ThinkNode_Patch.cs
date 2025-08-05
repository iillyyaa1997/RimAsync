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
    /// Patches for asynchronous thinking node processing
    /// Improves performance by processing AI thinking nodes in background
    /// </summary>
    [HarmonyPatch(typeof(ThinkNode))]
    public static class ThinkNode_Patch
    {
        /// <summary>
        /// Async thinking node processing
        /// </summary>
        [HarmonyPatch("TryIssueJobPackage")]
        [HarmonyPrefix]
        public static bool TryIssueJobPackage_Prefix(ThinkNode __instance, Pawn pawn, JobIssueParams jobParams, ref ThinkResult __result)
        {
            // Only apply async thinking if enabled
            if (!RimAsyncMod.Settings.enableAsyncAI)
            {
                return true; // Use original method
            }

            // Check if we can use async thinking for this pawn
            if (!CanUseAsyncThinking(pawn))
            {
                return true; // Use synchronous thinking
            }

            try
            {
                // Schedule async thinking
                ScheduleAsyncThinking(__instance, pawn, jobParams, result =>
                {
                    __result = result;
                });

                // Return a temporary thinking result
                __result = ThinkResult.NoJob;
                return false; // Skip original method
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Failed to process async thinking for pawn {pawn?.LabelShort}", ex, "ThinkNode");
                return true; // Fall back to original method
            }
        }

        /// <summary>
        /// Check if pawn can use async thinking
        /// </summary>
        private static bool CanUseAsyncThinking(Pawn pawn)
        {
            if (pawn == null) return false;
            if (pawn.InCombat) return false; // Combat requires immediate response
            if (pawn.health?.summaryHealth < 0.3f) return false; // Injured pawns need immediate attention

            return true;
        }

        /// <summary>
        /// Schedule async thinking processing
        /// </summary>
        private static void ScheduleAsyncThinking(ThinkNode thinkNode, Pawn pawn, JobIssueParams jobParams, Action<ThinkResult> callback)
        {
            AsyncManager.ScheduleWork(async () =>
            {
                try
                {
                    // Simulate async thinking process
                    await Task.Delay(10); // Small delay to simulate thinking time

                    // For now, return NoJob - this would be expanded with actual thinking logic
                    var result = ThinkResult.NoJob;

                    // Execute callback on main thread if needed
                    callback?.Invoke(result);

                    RimAsyncLogger.Debug($"Completed async thinking for pawn {pawn?.LabelShort}", "ThinkNode");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Async thinking failed for pawn {pawn?.LabelShort}", ex, "ThinkNode");
                    callback?.Invoke(ThinkResult.NoJob);
                }
            });
        }

        /// <summary>
        /// Public wrapper for testing async thinking
        /// </summary>
        public static bool TestAsyncThinking(ThinkNode thinkNode, Pawn pawn, JobIssueParams jobParams)
        {
            var result = ThinkResult.NoJob;
            return TryIssueJobPackage_Prefix(thinkNode, pawn, jobParams, ref result);
        }

        /// <summary>
        /// Public wrapper for testing thinking eligibility
        /// </summary>
        public static bool TestThinkingEligibility(Pawn pawn)
        {
            return CanUseAsyncThinking(pawn);
        }
    }
}
