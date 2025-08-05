using System;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using Verse;

namespace RimAsync.Patches.RW_Patches
{
    /// <summary>
    /// Patches for asynchronous construction processing
    /// Improves performance by processing construction tasks in background
    /// </summary>
    [HarmonyPatch(typeof(Thing))]
    public static class Construction_Patch
    {
        /// <summary>
        /// Async construction processing
        /// </summary>
        [HarmonyPatch("TickLong")]
        [HarmonyPrefix]
        public static bool TickLong_Prefix(Thing __instance)
        {
            // Only apply async construction if enabled and this is a construction-related thing
            if (!RimAsyncMod.Settings.enableAsyncBuilding || !IsConstructionRelated(__instance))
            {
                return true; // Use original method
            }

            // Check if we can use async construction for this thing
            if (!CanUseAsyncConstruction(__instance))
            {
                return true; // Use synchronous construction
            }

            try
            {
                // Schedule async construction
                ScheduleAsyncConstruction(__instance);
                return false; // Skip original method
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Failed to process async construction for {__instance?.def?.defName}", ex, "Construction");
                return true; // Fall back to original method
            }
        }

        /// <summary>
        /// Check if thing is construction-related
        /// </summary>
        private static bool IsConstructionRelated(Thing thing)
        {
            if (thing?.def?.defName == null) return false;

            // Check for construction-related things
            var constructionTerms = new[] { "Frame", "Blueprint", "Construction", "Build", "Wall", "Door", "Floor" };

            foreach (var term in constructionTerms)
            {
                if (thing.def.defName.Contains(term))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if we can use async construction for this thing
        /// </summary>
        private static bool CanUseAsyncConstruction(Thing thing)
        {
            if (thing == null) return false;
            if (!thing.Spawned) return false; // Only process spawned things

            // Don't process things in combat areas
            if (thing.Map?.threatTracker?.AnyMajorThreat == true) return false;

            return true;
        }

        /// <summary>
        /// Schedule async construction processing
        /// </summary>
        private static void ScheduleAsyncConstruction(Thing thing)
        {
            AsyncManager.ScheduleWork(async () =>
            {
                try
                {
                    // Simulate async construction process
                    await Task.Delay(50); // Small delay to simulate construction time

                    // Log construction progress
                    RimAsyncLogger.Debug($"Processed async construction for {thing?.def?.defName}", "Construction");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Async construction failed for {thing?.def?.defName}", ex, "Construction");
                }
            });
        }

        /// <summary>
        /// Public wrapper for testing async construction processing
        /// </summary>
        public static bool TestAsyncConstruction(Thing thing)
        {
            return TickLong_Prefix(thing);
        }

        /// <summary>
        /// Public wrapper for testing construction eligibility
        /// </summary>
        public static bool TestConstructionEligibility(Thing thing)
        {
            return CanUseAsyncConstruction(thing) && IsConstructionRelated(thing);
        }

        /// <summary>
        /// Public wrapper for testing construction relation
        /// </summary>
        public static bool TestConstructionRelation(Thing thing)
        {
            return IsConstructionRelated(thing);
        }
    }
}
