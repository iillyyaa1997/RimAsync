using System;
using System.Threading;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Automatically calculates optimal thread limits based on system capabilities
    /// Provides intelligent defaults for different CPU configurations
    /// </summary>
    public static class ThreadLimitCalculator
    {
        private static int? _cachedOptimalThreads = null;
        private static int? _cachedProcessorCount = null;

        /// <summary>
        /// Get the number of processor cores available to the system
        /// </summary>
        public static int ProcessorCount
        {
            get
            {
                if (_cachedProcessorCount == null)
                {
                    _cachedProcessorCount = Environment.ProcessorCount;
                }
                return _cachedProcessorCount.Value;
            }
        }

        /// <summary>
        /// Calculate optimal thread limit based on system capabilities
        /// Uses conservative approach to avoid overwhelming the system
        /// </summary>
        public static int CalculateOptimalThreadLimit()
        {
            if (_cachedOptimalThreads != null)
            {
                return _cachedOptimalThreads.Value;
            }

            try
            {
                int processorCount = ProcessorCount;
                int optimalThreads = CalculateThreadsForProcessorCount(processorCount);

                _cachedOptimalThreads = optimalThreads;

                RimAsyncLogger.Info(
                    $"Calculated optimal thread limit: {optimalThreads} (CPU cores: {processorCount})",
                    "ThreadCalculator"
                );

                return optimalThreads;
            }
            catch (Exception ex)
            {
                RimAsyncLogger.Error("Failed to calculate optimal thread limit, using default", ex, "ThreadCalculator");
                return 2; // Safe default
            }
        }

        /// <summary>
        /// Calculate thread count based on processor count
        /// Conservative formula to avoid system overload
        /// </summary>
        private static int CalculateThreadsForProcessorCount(int processorCount)
        {
            // Formula: use 25-50% of available cores, minimum 1, maximum 8
            // This leaves resources for RimWorld's main thread and other processes

            int calculatedThreads;

            if (processorCount <= 2)
            {
                // Low-end systems: use 1 thread to minimize overhead
                calculatedThreads = 1;
            }
            else if (processorCount <= 4)
            {
                // Mid-range systems (2-4 cores): use 50% of cores
                calculatedThreads = Math.Max(1, processorCount / 2);
            }
            else if (processorCount <= 8)
            {
                // High-end systems (4-8 cores): use 37.5% of cores
                calculatedThreads = Math.Max(2, (processorCount * 3) / 8);
            }
            else
            {
                // Very high-end systems (8+ cores): use 25-33% of cores
                calculatedThreads = Math.Max(2, processorCount / 4);
            }

            // Clamp to reasonable range: 1-8 threads (.NET 4.7.2 compatible)
            return Math.Max(1, Math.Min(calculatedThreads, 8));
        }

        /// <summary>
        /// Get recommended thread limit with optional override
        /// </summary>
        /// <param name="userPreference">User's preferred thread count (ignored if auto enabled)</param>
        /// <param name="autoEnabled">Whether auto calculation is enabled</param>
        /// <returns>Recommended thread count</returns>
        public static int GetRecommendedThreadLimit(int userPreference, bool autoEnabled)
        {
            // If auto enabled, always use optimal calculation
            if (autoEnabled)
            {
                return CalculateOptimalThreadLimit();
            }

            // Manual mode: use user preference, clamped to valid range (.NET 4.7.2 compatible)
            return Math.Max(1, Math.Min(userPreference, 8));
        }

        /// <summary>
        /// Get detailed system info for debugging
        /// </summary>
        public static string GetSystemInfo()
        {
            try
            {
                int processorCount = ProcessorCount;
                int optimalThreads = CalculateOptimalThreadLimit();
                int maxWorkerThreads, maxCompletionPortThreads;
                ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);

                return $"CPU Cores: {processorCount}, " +
                       $"Optimal Threads: {optimalThreads}, " +
                       $"ThreadPool Max Workers: {maxWorkerThreads}, " +
                       $"ThreadPool Max IO: {maxCompletionPortThreads}";
            }
            catch (Exception ex)
            {
                return $"System info unavailable: {ex.Message}";
            }
        }

        /// <summary>
        /// Clear cached values (for testing)
        /// </summary>
        public static void ClearCache()
        {
            _cachedOptimalThreads = null;
            _cachedProcessorCount = null;
        }

        /// <summary>
        /// Get performance category for current system
        /// </summary>
        public static PerformanceCategory GetPerformanceCategory()
        {
            int processorCount = ProcessorCount;

            if (processorCount <= 2)
                return PerformanceCategory.Low;
            else if (processorCount <= 4)
                return PerformanceCategory.Medium;
            else if (processorCount <= 8)
                return PerformanceCategory.High;
            else
                return PerformanceCategory.VeryHigh;
        }

        /// <summary>
        /// Get descriptive recommendation for user
        /// </summary>
        public static string GetRecommendationText()
        {
            var category = GetPerformanceCategory();
            int optimal = CalculateOptimalThreadLimit();

            switch (category)
            {
                case PerformanceCategory.Low:
                    return $"Low-end system detected. Recommended: {optimal} thread (minimizes overhead)";
                case PerformanceCategory.Medium:
                    return $"Mid-range system detected. Recommended: {optimal} threads (balanced performance)";
                case PerformanceCategory.High:
                    return $"High-end system detected. Recommended: {optimal} threads (good multitasking)";
                case PerformanceCategory.VeryHigh:
                    return $"Very high-end system detected. Recommended: {optimal} threads (optimal async)";
                default:
                    return $"Recommended: {optimal} threads";
            }
        }
    }

    /// <summary>
    /// System performance category
    /// </summary>
    public enum PerformanceCategory
    {
        Low,        // 1-2 cores
        Medium,     // 3-4 cores
        High,       // 5-8 cores
        VeryHigh    // 9+ cores
    }
}
