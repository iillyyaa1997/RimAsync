using System;
using System.Reflection;
using HarmonyLib;
using RimAsync.Core;
using RimAsync.Utils;
using Verse;

namespace RimAsync.Patches.Mod_Patches
{
    /// <summary>
    /// Example compatibility patches for other mods
    /// This serves as a template for creating compatibility with specific mods
    /// </summary>
    public static class ExampleMod_Patch
    {
        private static bool _modDetected = false;
        private static Assembly _modAssembly = null;

        /// <summary>
        /// Initialize mod compatibility patches
        /// Call this during RimAsync initialization
        /// </summary>
        public static void InitializeModPatches()
        {
            try
            {
                // Example: Detect if a specific mod is loaded
                DetectExampleMod();

                if (_modDetected)
                {
                    Log.Message("[RimAsync] Example mod detected, applying compatibility patches...");
                    ApplyExampleModPatches();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error initializing mod compatibility patches: {ex}");
            }
        }

        /// <summary>
        /// Detect if the example mod is loaded
        /// </summary>
        private static void DetectExampleMod()
        {
            foreach (var mod in ModsConfig.ActiveModsInLoadOrder)
            {
                // Example: Check for mod by package ID
                if (mod.PackageId.ToLower().Contains("examplemod"))
                {
                    _modDetected = true;
                    
                    // Try to get the mod's assembly
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (assembly.GetName().Name.Contains("ExampleMod"))
                        {
                            _modAssembly = assembly;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Apply compatibility patches for the example mod
        /// </summary>
        private static void ApplyExampleModPatches()
        {
            if (_modAssembly == null) return;

            try
            {
                // Example: Patch a method from another mod to work with RimAsync
                var targetType = _modAssembly.GetType("ExampleMod.ExampleClass");
                if (targetType != null)
                {
                    var targetMethod = targetType.GetMethod("ExampleMethod");
                    if (targetMethod != null)
                    {
                        var harmony = RimAsyncMod.HarmonyInstance;
                        var prefix = typeof(ExampleMod_Patch).GetMethod(nameof(ExampleMethod_Prefix));
                        
                        harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));
                        Log.Message("[RimAsync] Applied compatibility patch for ExampleMod.ExampleMethod");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error applying example mod patches: {ex}");
            }
        }

        /// <summary>
        /// Example prefix patch for another mod's method
        /// </summary>
        public static bool ExampleMethod_Prefix(/* original method parameters */)
        {
            try
            {
                // Only apply our optimization if it's safe
                if (!RimAsyncCore.CanUseAsync())
                {
                    return true; // Use original method
                }

                // Example: Wrap the operation for multiplayer safety
                MultiplayerCompat_Patch.WrapMultiplayerSafeOperation(() =>
                {
                    // Custom logic that works with RimAsync
                    if (RimAsyncMod.Settings?.enableDebugLogging == true)
                    {
                        Log.Message("[RimAsync] Executed ExampleMod compatibility patch");
                    }
                }, "ExampleModCompat");

                return true; // Continue with original method
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error in ExampleMod compatibility patch: {ex}");
                return true; // Fallback to original
            }
        }
    }

    /// <summary>
    /// Template for creating compatibility with performance-heavy mods
    /// </summary>
    public static class PerformanceModCompat
    {
        /// <summary>
        /// Example of how to coordinate with other performance mods
        /// </summary>
        public static void CoordinateWithPerformanceMod(string modName, Action operation)
        {
            try
            {
                // Check if we're in a performance-critical situation
                if (!PerformanceMonitor.IsPerformanceGood)
                {
                    // If performance is poor, defer to the other mod
                    Log.Message($"[RimAsync] Deferring to {modName} due to poor performance");
                    operation();
                    return;
                }

                // If performance is good, we can safely add our optimizations
                using (PerformanceMonitor.StartMeasuring($"CoordinatedWith_{modName}"))
                {
                    // Apply our optimizations alongside the other mod
                    operation();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error coordinating with {modName}: {ex}");
                // Always fall back to the other mod's behavior
                operation();
            }
        }
    }

    /// <summary>
    /// Utilities for mod detection and compatibility
    /// </summary>
    public static class ModCompatUtils
    {
        /// <summary>
        /// Check if a mod is loaded by package ID
        /// </summary>
        public static bool IsModLoaded(string packageId)
        {
            foreach (var mod in ModsConfig.ActiveModsInLoadOrder)
            {
                if (mod.PackageId.ToLower() == packageId.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Try to get a mod's assembly by name
        /// </summary>
        public static Assembly TryGetModAssembly(string assemblyName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name.Contains(assemblyName))
                {
                    return assembly;
                }
            }
            return null;
        }

        /// <summary>
        /// Safely try to patch a method from another mod
        /// </summary>
        public static bool TryPatchModMethod(Assembly modAssembly, string typeName, string methodName, 
            MethodInfo prefix = null, MethodInfo postfix = null, MethodInfo transpiler = null)
        {
            try
            {
                var targetType = modAssembly.GetType(typeName);
                if (targetType == null) return false;

                var targetMethod = targetType.GetMethod(methodName);
                if (targetMethod == null) return false;

                var harmony = RimAsyncMod.HarmonyInstance;
                var harmonyPrefix = prefix != null ? new HarmonyMethod(prefix) : null;
                var harmonyPostfix = postfix != null ? new HarmonyMethod(postfix) : null;
                var harmonyTranspiler = transpiler != null ? new HarmonyMethod(transpiler) : null;

                harmony.Patch(targetMethod, harmonyPrefix, harmonyPostfix, harmonyTranspiler);
                
                Log.Message($"[RimAsync] Successfully patched {typeName}.{methodName}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Warning($"[RimAsync] Failed to patch {typeName}.{methodName}: {ex.Message}");
                return false;
            }
        }
    }
} 