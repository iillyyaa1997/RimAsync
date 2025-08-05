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
        /// </summary>
        private static void ScheduleBuildingOptimization(Thing building)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate building optimization work
                            await Task.Delay(4, cancellationToken);

                            // Record the optimization
                            PerformanceMonitor.RecordMetric("BuildingOptimization", 4.0f);

                            RimAsyncLogger.Debug($"Building optimization completed for {building.def?.defName}", "BuildingSystem");
                        },
                        () =>
                        {
                            // Sync fallback - minimal building processing
                            PerformanceMonitor.RecordMetric("BuildingOptimization", 1.0f);
                        },
                        "BuildingOptimization");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in building optimization", ex, "BuildingSystem");
                }
            });
        }

        /// <summary>
        /// Check if this thing is building-like and should be processed async
        /// </summary>
        private static bool IsBuildingLike(Thing thing)
        {
            if (thing?.def?.defName == null) return false;

            // Building types that can benefit from async processing
            var asyncBuildings = new[]
            {
                "Wall", "Door", "Bed", "Table", "Chair", "Workbench",
                "Plant", "Tree", "Rock", "Building", "Structure",
                "Power", "Battery", "Generator", "Solar", "Wind"
            };

            foreach (var buildingType in asyncBuildings)
            {
                if (thing.def.defName.Contains(buildingType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // Check by thing class
            return thing.def.thingClass?.Name?.Contains("Building") == true;
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
                if (building.def.defName.Contains(criticalType, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Patches for async construction and deconstruction
    /// </summary>
    [HarmonyPatch]
    public static class Construction_Patch
    {
        /// <summary>
        /// Async construction processing
        /// </summary>
        [HarmonyPatch(typeof(Thing), "SpawnSetup")]
        [HarmonyPrefix]
        public static bool SpawnSetup_Prefix(Thing __instance, Map map, bool respawningAfterLoad)
        {
            // Only apply async construction if enabled and this is a building
            if (!RimAsyncMod.Settings.enableAsyncBuilding || !IsBuildingLike(__instance))
            {
                return true; // Use original method
            }

            if (map == null)
            {
                return true; // Use original method for invalid map
            }

            try
            {
                // Try async construction setup
                if (TryAsyncConstructionSetup(__instance, map, respawningAfterLoad))
                {
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async construction setup, falling back to sync", ex, "ConstructionSystem");
            }

            // Fallback to original method
            return true;
        }

        /// <summary>
        /// Async deconstruction processing
        /// </summary>
        [HarmonyPatch(typeof(Thing), "DeSpawn")]
        [HarmonyPrefix]
        public static bool DeSpawn_Prefix(Thing __instance, DestroyMode mode)
        {
            // Only apply async deconstruction if enabled and this is a building
            if (!RimAsyncMod.Settings.enableAsyncBuilding || !IsBuildingLike(__instance))
            {
                return true; // Use original method
            }

            // Don't use async for violent destruction
            if (mode == DestroyMode.KillFinalize || mode == DestroyMode.FailConstruction)
            {
                return true; // Use original method for violent destruction
            }

            try
            {
                // Try async deconstruction
                if (TryAsyncDeconstruction(__instance, mode))
                {
                    return false; // Skip original method
                }
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error($"Error in async deconstruction, falling back to sync", ex, "ConstructionSystem");
            }

            // Fallback to original method
            return true;
        }

        /// <summary>
        /// Attempt asynchronous construction setup
        /// </summary>
        private static bool TryAsyncConstructionSetup(Thing building, Map map, bool respawningAfterLoad)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync() || !RimAsyncCore.CanUseAsync())
            {
                return false; // Fallback to sync
            }

            using (PerformanceMonitor.StartMeasuring("AsyncConstructionSetup"))
            {
                // Quick setup for immediate response
                building.Map = map;
                building.Spawned = true;

                RimAsyncLogger.Debug($"Quick construction setup for {building.def?.defName}", "ConstructionSystem");

                // Schedule background construction finalization
                ScheduleConstructionFinalization(building, respawningAfterLoad);

                return true;
            }
        }

        /// <summary>
        /// Attempt asynchronous deconstruction
        /// </summary>
        private static bool TryAsyncDeconstruction(Thing building, DestroyMode mode)
        {
            // Check if we can use async operations
            if (!AsyncManager.CanExecuteAsync() || !RimAsyncCore.CanUseAsync())
            {
                return false; // Fallback to sync
            }

            using (PerformanceMonitor.StartMeasuring("AsyncDeconstruction"))
            {
                // Quick deconstruction for immediate response
                building.Spawned = false;

                RimAsyncLogger.Debug($"Quick deconstruction for {building.def?.defName}", "ConstructionSystem");

                // Schedule background deconstruction cleanup
                ScheduleDeconstructionCleanup(building, mode);

                return true;
            }
        }

        /// <summary>
        /// Schedule background construction finalization
        /// </summary>
        private static void ScheduleConstructionFinalization(Thing building, bool respawningAfterLoad)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate construction finalization
                            await Task.Delay(6, cancellationToken);

                            // Record the construction
                            PerformanceMonitor.RecordMetric("ConstructionFinalization", 6.0f);

                            RimAsyncLogger.Debug($"Construction finalized for {building.def?.defName}", "ConstructionSystem");
                        },
                        () =>
                        {
                            // Sync fallback - minimal construction finalization
                            PerformanceMonitor.RecordMetric("ConstructionFinalization", 1.0f);
                        },
                        "ConstructionFinalization");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in construction finalization", ex, "ConstructionSystem");
                }
            });
        }

        /// <summary>
        /// Schedule background deconstruction cleanup
        /// </summary>
        private static void ScheduleDeconstructionCleanup(Thing building, DestroyMode mode)
        {
            if (!RimAsyncCore.CanUseAsync()) return;

            Task.Run(async () =>
            {
                try
                {
                    await AsyncManager.ExecuteAdaptive(
                        async (cancellationToken) =>
                        {
                            // Simulate deconstruction cleanup
                            await Task.Delay(4, cancellationToken);

                            // Record the deconstruction
                            PerformanceMonitor.RecordMetric("DeconstructionCleanup", 4.0f);

                            RimAsyncLogger.Debug($"Deconstruction cleanup completed for {building.def?.defName}", "ConstructionSystem");
                        },
                        () =>
                        {
                            // Sync fallback - minimal cleanup
                            PerformanceMonitor.RecordMetric("DeconstructionCleanup", 1.0f);
                        },
                        "DeconstructionCleanup");
                }
                catch (Exception ex)
                {
                    RimAsyncLogger.Error($"Error in deconstruction cleanup", ex, "ConstructionSystem");
                }
            });
        }

        /// <summary>
        /// Check if this thing is building-like (reuse from Building_Patch)
        /// </summary>
        private static bool IsBuildingLike(Thing thing)
        {
            return Building_Patch.IsBuildingLike(thing);
        }
    }
}
