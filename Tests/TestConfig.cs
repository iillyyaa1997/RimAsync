using System;

namespace RimAsync.Tests
{
    /// <summary>
    /// Global configuration for RimAsync test suite
    /// </summary>
    public static class TestConfig
    {
        // Timeouts
        public const int DefaultTimeoutMs = 5000;
        public const int PerformanceTimeoutMs = 30000;
        public const int IntegrationTimeoutMs = 15000;
        public const int MultiplayerTimeoutMs = 60000;
        
        // Performance targets
        public const float SmallColonyTPSImprovement = 0.15f; // +15%
        public const float MediumColonyTPSImprovement = 0.20f; // +20%
        public const float LargeColonyTPSImprovement = 0.25f; // +25%
        public const float MaxMemoryIncrease = 0.10f; // +10%
        
        // Thread limits
        public const int MaxTestThreads = 4;
        public const int DefaultThreadPoolSize = 2;
        
        // File paths
        public const string MpDesyncPath = "/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs";
        public const string TestDataPath = "Tests/Data";
        public const string TestLogsPath = "Tests/Logs";
        
        // Test categories
        public const string UnitTestCategory = "Unit";
        public const string IntegrationTestCategory = "Integration";
        public const string PerformanceTestCategory = "Performance";
        public const string MultiplayerTestCategory = "Multiplayer";
        
        // Test priorities
        public const string CriticalPriority = "Critical";
        public const string HighPriority = "High";
        public const string MediumPriority = "Medium";
        public const string LowPriority = "Low";
        
        // Mock settings
        public const bool EnableAsyncTimeMock = true;
        public const bool EnableRimWorldMocks = true;
        public const bool EnableHarmonyMocks = true;
        
        /// <summary>
        /// Get timeout based on test category
        /// </summary>
        public static int GetTimeoutForCategory(string category)
        {
            return category switch
            {
                PerformanceTestCategory => PerformanceTimeoutMs,
                IntegrationTestCategory => IntegrationTimeoutMs,
                MultiplayerTestCategory => MultiplayerTimeoutMs,
                _ => DefaultTimeoutMs
            };
        }
        
        /// <summary>
        /// Check if performance target is met
        /// </summary>
        public static bool IsPerformanceTargetMet(string colonySize, float improvement)
        {
            return colonySize.ToLower() switch
            {
                "small" => improvement >= SmallColonyTPSImprovement,
                "medium" => improvement >= MediumColonyTPSImprovement,
                "large" => improvement >= LargeColonyTPSImprovement,
                _ => improvement > 0
            };
        }
        
        /// <summary>
        /// Get test priority numeric value for sorting
        /// </summary>
        public static int GetPriorityValue(string priority)
        {
            return priority switch
            {
                CriticalPriority => 4,
                HighPriority => 3,
                MediumPriority => 2,
                LowPriority => 1,
                _ => 0
            };
        }
    }
} 