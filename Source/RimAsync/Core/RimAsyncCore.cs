using System;
using RimAsync.Threading;
using RimAsync.Utils;
using Verse;

namespace RimAsync.Core
{
    /// <summary>
    /// Core coordinator for all RimAsync systems
    /// Manages initialization, lifecycle, and coordination between components
    /// </summary>
    public static class RimAsyncCore
    {
        public static bool IsInitialized { get; private set; }
        public static bool IsMultiplayerActive => MultiplayerCompat.IsInMultiplayer;
        public static bool AsyncTimeEnabled => MultiplayerCompat.AsyncTimeEnabled;

        /// <summary>
        /// Initialize all RimAsync systems
        /// Called during mod startup
        /// </summary>
        public static void Initialize()
        {
            if (IsInitialized)
            {
                Log.Warning("[RimAsync] Already initialized, skipping...");
                return;
            }

            try
            {
                Log.Message("[RimAsync] Initializing core systems...");

                // Initialize multiplayer compatibility detection
                MultiplayerCompat.Initialize();

                // Initialize threading manager
                AsyncManager.Initialize();

                // Initialize performance monitoring
                PerformanceMonitor.Initialize();

                IsInitialized = true;
                Log.Message("[RimAsync] Core systems initialized successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Failed to initialize core systems: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Called when settings are changed
        /// </summary>
        public static void OnSettingsChanged()
        {
            if (!IsInitialized) return;

            try
            {
                Log.Message("[RimAsync] Settings changed, updating systems...");
                
                // Notify all systems of settings change
                AsyncManager.OnSettingsChanged();
                PerformanceMonitor.OnSettingsChanged();
                
                Log.Message("[RimAsync] Systems updated for new settings");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error updating systems for new settings: {ex}");
            }
        }

        /// <summary>
        /// Cleanup resources when mod is unloaded
        /// </summary>
        public static void Shutdown()
        {
            if (!IsInitialized) return;

            try
            {
                Log.Message("[RimAsync] Shutting down systems...");

                AsyncManager.Shutdown();
                PerformanceMonitor.Shutdown();

                IsInitialized = false;
                Log.Message("[RimAsync] Systems shut down successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error during shutdown: {ex}");
            }
        }

        /// <summary>
        /// Check if async operations are safe to use
        /// Considers multiplayer status and AsyncTime setting
        /// </summary>
        public static bool CanUseAsync()
        {
            // Always allow async in single player
            if (!IsMultiplayerActive) return true;

            // In multiplayer, only allow if AsyncTime is enabled
            return AsyncTimeEnabled;
        }

        /// <summary>
        /// Get current execution mode based on multiplayer status
        /// </summary>
        public static ExecutionMode GetExecutionMode()
        {
            if (!IsMultiplayerActive)
                return ExecutionMode.FullAsync;

            return AsyncTimeEnabled ? ExecutionMode.AsyncTimeEnabled : ExecutionMode.FullSync;
        }
    }

    /// <summary>
    /// Execution modes for RimAsync operations
    /// </summary>
    public enum ExecutionMode
    {
        /// <summary>
        /// Single player - full async performance
        /// </summary>
        FullAsync,
        
        /// <summary>
        /// Multiplayer with AsyncTime - limited async operations
        /// </summary>
        AsyncTimeEnabled,
        
        /// <summary>
        /// Multiplayer without AsyncTime - full synchronous execution
        /// </summary>
        FullSync
    }
} 