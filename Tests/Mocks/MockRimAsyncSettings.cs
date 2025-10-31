using RimAsync.Core;

namespace RimAsync.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of RimAsyncSettings for testing
    /// Provides default settings without RimWorld dependencies
    /// </summary>
    public class MockRimAsyncSettings : RimAsyncSettings
    {
        /// <summary>
        /// Create mock settings with default values
        /// </summary>
        public MockRimAsyncSettings()
        {
            // Performance Settings (enabled by default for testing)
            enableAsyncPathfinding = true;
            enableBackgroundJobs = true;
            enableAsyncJobExecution = true;
            enableAsyncAI = true;
            enableAsyncBuilding = true;
            enableSmartCaching = true;
            enableMemoryOptimization = true;

            // Safety Settings (enabled for comprehensive testing)
            enableFallbackMechanisms = true;
            enablePerformanceMonitoring = true;

            // Advanced Settings (conservative defaults for testing)
            maxAsyncThreads = 2;
            asyncTimeoutSeconds = 5.0f;
            enableDebugLogging = false; // Disable to reduce test noise

            // Logging Settings
            logLevel = 1; // Info level

            // Multiplayer Compatibility
            respectAsyncTimeSetting = true;
            enableMultiplayerOptimizations = true;
        }

        /// <summary>
        /// Create mock settings optimized for performance testing
        /// </summary>
        public static MockRimAsyncSettings CreateForPerformanceTests()
        {
            return new MockRimAsyncSettings
            {
                // Enable all performance features
                enableAsyncPathfinding = true,
                enableBackgroundJobs = true,
                enableAsyncJobExecution = true,
                enableAsyncAI = true,
                enableAsyncBuilding = true,
                enableSmartCaching = true,
                enableMemoryOptimization = true,
                enablePerformanceMonitoring = true,

                // Increase thread pool for parallel tests
                maxAsyncThreads = 4,
                asyncTimeoutSeconds = 10.0f,

                // Disable debug logging for cleaner test output
                enableDebugLogging = false,
                logLevel = 2 // Warning level
            };
        }

        /// <summary>
        /// Create mock settings with all optimizations disabled
        /// </summary>
        public static MockRimAsyncSettings CreateWithOptimizationsDisabled()
        {
            return new MockRimAsyncSettings
            {
                // Disable all async features
                enableAsyncPathfinding = false,
                enableBackgroundJobs = false,
                enableAsyncJobExecution = false,
                enableAsyncAI = false,
                enableAsyncBuilding = false,
                enableSmartCaching = false,
                enableMemoryOptimization = false,

                // Keep monitoring enabled to track performance
                enablePerformanceMonitoring = true,
                enableFallbackMechanisms = true,

                maxAsyncThreads = 1,
                asyncTimeoutSeconds = 1.0f,
                enableDebugLogging = false
            };
        }

        /// <summary>
        /// Create mock settings for multiplayer testing
        /// </summary>
        public static MockRimAsyncSettings CreateForMultiplayerTests()
        {
            return new MockRimAsyncSettings
            {
                // Enable multiplayer-safe features only
                enableAsyncPathfinding = true,
                enableBackgroundJobs = false, // Can cause desyncs
                enableAsyncJobExecution = true,
                enableAsyncAI = true,
                enableAsyncBuilding = true,
                enableSmartCaching = true,
                enableMemoryOptimization = true,

                // Multiplayer safety
                respectAsyncTimeSetting = true,
                enableMultiplayerOptimizations = true,
                enableFallbackMechanisms = true,
                enablePerformanceMonitoring = true,

                maxAsyncThreads = 2,
                asyncTimeoutSeconds = 3.0f,
                enableDebugLogging = false
            };
        }

        /// <summary>
        /// Reset to default test values
        /// </summary>
        public void ResetToDefaults()
        {
            var defaults = new MockRimAsyncSettings();
            
            enableAsyncPathfinding = defaults.enableAsyncPathfinding;
            enableBackgroundJobs = defaults.enableBackgroundJobs;
            enableAsyncJobExecution = defaults.enableAsyncJobExecution;
            enableAsyncAI = defaults.enableAsyncAI;
            enableAsyncBuilding = defaults.enableAsyncBuilding;
            enableSmartCaching = defaults.enableSmartCaching;
            enableMemoryOptimization = defaults.enableMemoryOptimization;
            enableFallbackMechanisms = defaults.enableFallbackMechanisms;
            enablePerformanceMonitoring = defaults.enablePerformanceMonitoring;
            maxAsyncThreads = defaults.maxAsyncThreads;
            asyncTimeoutSeconds = defaults.asyncTimeoutSeconds;
            enableDebugLogging = defaults.enableDebugLogging;
            logLevel = defaults.logLevel;
            respectAsyncTimeSetting = defaults.respectAsyncTimeSetting;
            enableMultiplayerOptimizations = defaults.enableMultiplayerOptimizations;
        }
    }
}

