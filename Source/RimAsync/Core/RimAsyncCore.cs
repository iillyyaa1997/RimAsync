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
                RimAsyncLogger.Warning("Already initialized, skipping...", "Core");
                return;
            }

            try
            {
                RimAsyncLogger.InitStep("RimAsyncCore", "Starting initialization");

                // Initialize multiplayer compatibility detection
                RimAsyncLogger.Debug("Initializing multiplayer compatibility detection", "Core");
                MultiplayerCompat.Initialize();

                // Initialize threading manager
                RimAsyncLogger.Debug("Initializing threading manager", "Core");
                AsyncManager.Initialize();

                // Initialize performance monitoring
                RimAsyncLogger.Debug("Initializing performance monitoring", "Core");
                PerformanceMonitor.Initialize();

                IsInitialized = true;
                RimAsyncLogger.InitStep("RimAsyncCore", "Core systems initialization completed", true);
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error("Failed to initialize core systems", ex, "Core");
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
                RimAsyncLogger.Info("Settings changed, updating systems...", "Core");

                // Configure logger with new settings
                var settings = RimAsyncMod.Settings;
                RimAsyncLogger.Configure(settings.enableDebugLogging, (RimAsyncLogger.LogLevel)settings.logLevel);

                // Notify all systems of settings change
                RimAsyncLogger.Debug("Notifying AsyncManager of settings change", "Core");
                AsyncManager.OnSettingsChanged();

                RimAsyncLogger.Debug("Notifying PerformanceMonitor of settings change", "Core");
                PerformanceMonitor.OnSettingsChanged();

                RimAsyncLogger.Info("Systems updated for new settings", "Core");
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error("Error updating systems for new settings", ex, "Core");
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
                RimAsyncLogger.Info("Shutting down systems...", "Core");

                RimAsyncLogger.Debug("Shutting down AsyncManager", "Core");
                AsyncManager.Shutdown();

                RimAsyncLogger.Debug("Shutting down PerformanceMonitor", "Core");
                PerformanceMonitor.Shutdown();

                IsInitialized = false;
                RimAsyncLogger.Info("Systems shut down successfully", "Core");
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error("Error during shutdown", ex, "Core");
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
