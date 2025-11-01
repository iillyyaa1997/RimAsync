using RimWorld;
using UnityEngine;
using Verse;

namespace RimAsync.Core
{
    /// <summary>
    /// Settings for RimAsync mod
    /// Controls performance optimizations and behavior
    /// </summary>
    public class RimAsyncSettings : ModSettings
    {
        // Performance Settings
        public bool enableAsyncPathfinding = true;
        public bool enableBackgroundJobs = true;
        public bool enableAsyncJobExecution = true;
        public bool enableAsyncAI = true;
        public bool enableAsyncBuilding = true;
        public bool enableSmartCaching = true;
        public bool enableMemoryOptimization = true;

        // Safety Settings
        public bool enableFallbackMechanisms = true;
        public bool enablePerformanceMonitoring = true;

        // Advanced Settings
        public int maxAsyncThreads = 2;
        public bool enableAutoThreadLimits = true; // New: auto calculate thread limits
        public float asyncTimeoutSeconds = 5.0f;
        public bool enableDebugLogging = false;

        // Logging Settings
        public int logLevel = 1; // 0=Debug, 1=Info, 2=Warning, 3=Error

        // Multiplayer Compatibility
        public bool respectAsyncTimeSetting = true;
        public bool enableMultiplayerOptimizations = true;

        public override void ExposeData()
        {
            // Performance Settings
            Scribe_Values.Look(ref enableAsyncPathfinding, "enableAsyncPathfinding", true);
            Scribe_Values.Look(ref enableBackgroundJobs, "enableBackgroundJobs", true);
            Scribe_Values.Look(ref enableAsyncJobExecution, "enableAsyncJobExecution", true);
            Scribe_Values.Look(ref enableAsyncAI, "enableAsyncAI", true);
            Scribe_Values.Look(ref enableAsyncBuilding, "enableAsyncBuilding", true);
            Scribe_Values.Look(ref enableSmartCaching, "enableSmartCaching", true);
            Scribe_Values.Look(ref enableMemoryOptimization, "enableMemoryOptimization", true);

            // Safety Settings
            Scribe_Values.Look(ref enableFallbackMechanisms, "enableFallbackMechanisms", true);
            Scribe_Values.Look(ref enablePerformanceMonitoring, "enablePerformanceMonitoring", true);

            // Advanced Settings
            Scribe_Values.Look(ref maxAsyncThreads, "maxAsyncThreads", 2);
            Scribe_Values.Look(ref enableAutoThreadLimits, "enableAutoThreadLimits", true);
            Scribe_Values.Look(ref asyncTimeoutSeconds, "asyncTimeoutSeconds", 5.0f);
            Scribe_Values.Look(ref enableDebugLogging, "enableDebugLogging", false);

            // Logging Settings
            Scribe_Values.Look(ref logLevel, "logLevel", 1);

            // Multiplayer Compatibility
            Scribe_Values.Look(ref respectAsyncTimeSetting, "respectAsyncTimeSetting", true);
            Scribe_Values.Look(ref enableMultiplayerOptimizations, "enableMultiplayerOptimizations", true);

            base.ExposeData();
        }

        public void DoWindowContents(Rect inRect)
        {
            var listing = new Listing_Standard();
            listing.Begin(inRect);

            // Header
            listing.Label("RimAsync Settings");
            listing.Gap();

            // Performance Section
            listing.Label("Performance Optimizations:");
            listing.CheckboxLabeled("Enable Async Pathfinding", ref enableAsyncPathfinding,
                "Allows pawns to find paths in background without blocking gameplay");
            listing.CheckboxLabeled("Enable Background Jobs", ref enableBackgroundJobs,
                "Processes work tasks in background for smoother performance");
            listing.CheckboxLabeled("Enable Async Job Execution", ref enableAsyncJobExecution,
                "Executes pawn jobs asynchronously for better responsiveness");
            listing.CheckboxLabeled("Enable Async AI Processing", ref enableAsyncAI,
                "Processes AI thinking and decisions in background");
            listing.CheckboxLabeled("Enable Async Building Updates", ref enableAsyncBuilding,
                "Handles building construction and updates asynchronously");
            listing.CheckboxLabeled("Enable Smart Caching", ref enableSmartCaching,
                "Intelligently caches frequent calculations");
            listing.CheckboxLabeled("Enable Memory Optimization", ref enableMemoryOptimization,
                "Reduces memory usage for better performance");

            listing.Gap();

            // Safety Section
            listing.Label("Safety & Reliability:");
            listing.CheckboxLabeled("Enable Fallback Mechanisms", ref enableFallbackMechanisms,
                "Automatically falls back to original code if issues occur");
            listing.CheckboxLabeled("Enable Performance Monitoring", ref enablePerformanceMonitoring,
                "Monitors performance and adjusts optimizations automatically");

            listing.Gap();

            // Multiplayer Section
            listing.Label("Multiplayer Compatibility:");
            listing.CheckboxLabeled("Respect AsyncTime Setting", ref respectAsyncTimeSetting,
                "Follows RimWorld Multiplayer AsyncTime setting for safety");
            listing.CheckboxLabeled("Enable Multiplayer Optimizations", ref enableMultiplayerOptimizations,
                "Additional optimizations safe for multiplayer use");

            listing.Gap();

            // Advanced Section
            listing.Label("Advanced Settings:");

            // Auto thread limits checkbox
            listing.CheckboxLabeled("Auto Thread Limits", ref enableAutoThreadLimits,
                "Automatically determine optimal thread count based on system capabilities");

            // Show thread limit slider (disabled if auto is on)
            if (enableAutoThreadLimits)
            {
                var optimal = RimAsync.Utils.ThreadLimitCalculator.CalculateOptimalThreadLimit();
                listing.Label($"Auto Thread Count: {optimal} (based on {RimAsync.Utils.ThreadLimitCalculator.ProcessorCount} CPU cores)");
                listing.Label(RimAsync.Utils.ThreadLimitCalculator.GetRecommendationText());
            }
            else
            {
                listing.Label($"Manual Thread Count: {maxAsyncThreads}");
                maxAsyncThreads = (int)listing.Slider(maxAsyncThreads, 1, 8);
            }

            listing.Label($"Async Timeout: {asyncTimeoutSeconds:F1}s");
            asyncTimeoutSeconds = listing.Slider(asyncTimeoutSeconds, 1.0f, 30.0f);

            listing.CheckboxLabeled("Enable Debug Logging", ref enableDebugLogging,
                "Enables detailed logging for troubleshooting (may impact performance)");

            // Logging Level Setting
            var logLevelLabels = new[] { "Debug", "Info", "Warning", "Error" };
            listing.Label($"Log Level: {logLevelLabels[logLevel]}");
            logLevel = (int)listing.Slider(logLevel, 0, 3);

            listing.Gap();

            // Status Information
            var core = RimAsync.Core.RimAsyncCore.GetExecutionMode();
            listing.Label($"Current Mode: {core}");

            if (RimAsync.Core.RimAsyncCore.IsMultiplayerActive)
            {
                listing.Label($"AsyncTime Enabled: {RimAsync.Core.RimAsyncCore.AsyncTimeEnabled}");
            }

            listing.End();
        }
    }
}
