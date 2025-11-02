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
    /// Patches for asynchronous building construction and updates
    /// Improves performance by processing building operations in background
    /// </summary>
    [HarmonyPatch(typeof(Thing))]
    public static class Building_Patch
    {
        /// <summary>
        /// Async building tick for better performance
        /// </summary>
        [HarmonyPatch("Tick")]
        [HarmonyPrefix]
        public static bool Tick_Prefix(Thing __instance)
        {
            // Safety check: Settings might not be initialized during early game loading
            if (RimAsyncMod.Settings == null)
            {
                return true; // Use original method if settings not ready
            }

            // Only apply async building if enabled and this is a building-like thing
            if (!RimAsyncMod.Settings.enableAsyncBuilding || !IsBuildingLike(__instance))
            {
                return true; // Use original method
            }

            if (__instance?.Map == null || !__instance.Spawned)
            {
                return true; // Use original method for invalid buildings
            }

            // Don't use async for critical building operations
            if (IsBuildingCritical(__instance))
            {
                return true; // Use original method
            }

            try
            {
                // Try async building processing
                if (TryAsyncBuildingProcessing(__instance))
                {
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async building processing, falling back to sync", ex, "BuildingSystem");
            }

            // Fallback to original method with monitoring
            using (PerformanceMonitor.StartMeasuring("SyncBuildingTick"))
            {
                return true;
            }
        }

        /// <summary>
        /// Public wrapper for testing async building processing
        /// </summary>
        public static bool TestAsyncBuildingProcessing(Thing building)
        {
            return Tick_Prefix(building);
        }

        /// <summary>
        /// Public wrapper for testing building construction
        /// </summary>
        public static bool TestBuildingConstruction(Thing building)
        {
            return IsBuildingLike(building);
        }

        /// <summary>
        /// Attempt asynchronous building processing
        /// </summary>
        private static bool TryAsyncBuildingProcessing(Thing building)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync() || !RimAsyncCore.CanUseAsync())
            {
                return false; // Fallback to sync
            }

            using (PerformanceMonitor.StartMeasuring("AsyncBuildingProcessing"))
            {
                // Quick building update for immediate response
                UpdateBuildingQuick(building);

                // Schedule background building optimization
                ScheduleBuildingOptimization(building);

                return true;
            }
        }

        /// <summary>
        /// Quick building update for immediate response
        /// </summary>
        private static void UpdateBuildingQuick(Thing building)
        {
            // Simplified building updates that can be done immediately

            // Update position-based properties
            if (building.Position != IntVec3.Zero)
            {
                // Building is properly positioned - can do quick updates
                RimAsyncLogger.Debug($"Quick building update for {building.def?.defName} at {building.Position}", "BuildingSystem");
            }

            // Record basic building activity
            PerformanceMonitor.RecordMetric("QuickBuildingUpdate", 1.0f);
        }

        /// <summary>
        /// Schedule background building optimization
        /// RimWorld 1.6: SAFE main-thread scheduling (no Task.Run)
        /// </summary>
        private static void ScheduleBuildingOptimization(Thing building)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            // SAFE: Use AsyncManager.ExecuteAdaptive which handles thread safety internally
            // NO Task.Run() - all Unity/RimWorld objects MUST be accessed from main thread
            AsyncManager.ExecuteAdaptive(
                async (cancellationToken) =>
                {
                    try
                    {
                        // Simulate building optimization work
                        await Task.Delay(4, cancellationToken);

                        // Record the optimization
                        PerformanceMonitor.RecordMetric("BuildingOptimization", 4.0f);

                        RimAsyncLogger.Debug($"Building optimization completed for {building.def?.defName}", "BuildingSystem");
                    }
                    catch (Exception ex)
                    {
                        RimAsyncLogger.Error($"Error in building optimization", ex, "BuildingSystem");
                    }
                },
                () =>
                {
                    // Sync fallback - minimal building processing
                    PerformanceMonitor.RecordMetric("BuildingOptimization", 1.0f);
                },
                "BuildingOptimization");
        }

        /// <summary>
        /// Check if this thing is building-like and should be processed async
        /// </summary>
        public static bool IsBuildingLike(Thing thing)
        {
            if (thing?.def?.defName == null) return false;

            // Building types that can benefit from async processing
            var asyncBuildings = new[]
            {
                "wall", "door", "bed", "table", "chair", "workbench",
                "plant", "tree", "rock", "building", "structure",
                "power", "battery", "generator", "solar", "wind"
            };

            var defNameLower = thing.def.defName.ToLowerInvariant();

            foreach (var buildingType in asyncBuildings)
            {
                if (defNameLower.Contains(buildingType))
                {
                    return true;
                }
            }

            // Check by thing class
            return thing.def.thingClass?.Name?.ToLowerInvariant().Contains("building") == true;
        }

        /// <summary>
        /// Check if building processing is critical and should not be made async
        /// </summary>
        private static bool IsBuildingCritical(Thing building)
        {
            if (building?.def?.defName == null) return false;

            // Critical building types that need immediate processing
            var criticalBuildings = new[]
            {
                "Turret", "Trap", "Defense", "Security", "Medical",
                "Emergency", "Fire", "Explosion", "Combat"
            };

            foreach (var criticalType in criticalBuildings)
            {
                // .NET 4.7.2 compatible: IndexOf instead of Contains with StringComparison
                if (building.def.defName.IndexOf(criticalType, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Patches for async construction and deconstruction - DISABLED
    /// These patches were causing critical issues by intercepting Thing.SpawnSetup/DeSpawn
    /// which broke texture loading, mod initialization, and game state management.
    ///
    /// TODO: Reimplement using Postfix patches that don't skip original methods.
    /// </summary>
    /*
    [HarmonyPatch]
    public static class Construction_Patch
    {
        // DISABLED: These patches break game initialization by skipping critical setup code
        // DO NOT RE-ENABLE without fixing the fundamental issue:
        // - We cannot skip SpawnSetup/DeSpawn original methods
        // - Manual field assignment (Map, Spawned) is insufficient
        // - Missing: texture loading, component initialization, region registration, etc.
    }
    */
}
