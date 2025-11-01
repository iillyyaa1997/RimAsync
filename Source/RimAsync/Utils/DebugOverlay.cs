using System;
using System.Text;
using RimAsync.Threading;
using RimAsync.Core;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Debug overlay for displaying real-time performance metrics
    /// Shows TPS, cache statistics, async operations, and more
    /// Note: UI rendering is disabled in Docker builds (requires real RimWorld UI assemblies)
    /// </summary>
    public static class DebugOverlay
    {
        private static bool _enabled = false;
        private static readonly StringBuilder _stringBuilder = new StringBuilder();
        private static float _lastUpdateTime = 0f;
        private static string _cachedOverlayText = "";

        private const float UPDATE_INTERVAL = 0.5f; // Update every 0.5 seconds

        /// <summary>
        /// Enable or disable debug overlay
        /// </summary>
        public static bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        /// Toggle debug overlay on/off
        /// </summary>
        public static void Toggle()
        {
            _enabled = !_enabled;
            if (_enabled)
            {
                Log.Message("[RimAsync] Debug overlay enabled");
            }
            else
            {
                Log.Message("[RimAsync] Debug overlay disabled");
            }
        }

        /// <summary>
        /// Render debug overlay on screen
        /// Called from OnGUI
        /// Note: This method is a no-op in Docker builds without RimWorld UI assemblies
        /// </summary>
        public static void OnGUI()
        {
            if (!_enabled) return;

            try
            {
                // Update cached text periodically to reduce performance impact
                float currentTime = UnityEngine.Time.realtimeSinceStartup;
                if (currentTime - _lastUpdateTime > UPDATE_INTERVAL)
                {
                    _cachedOverlayText = GenerateOverlayText();
                    _lastUpdateTime = currentTime;
                }

                // Note: Actual UI rendering requires RimWorld UI assemblies (Widgets, Text, etc.)
                // In real RimWorld environment, this would render the overlay
                // In Docker/test environment, this is disabled
#if !DEBUG
                // Only attempt UI rendering in non-debug builds (real RimWorld environment)
                RenderOverlayUI(_cachedOverlayText);
#endif
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error rendering debug overlay: {ex}");
            }
        }

        /// <summary>
        /// Render the actual UI (only works with real RimWorld UI assemblies)
        /// </summary>
        private static void RenderOverlayUI(string text)
        {
            // This method would use RimWorld's Widgets and Text classes
            // which are not available in Docker builds
            // Implementation is intentionally left empty for Docker compatibility
        }

        /// <summary>
        /// Generate overlay text with all metrics
        /// </summary>
        private static string GenerateOverlayText()
        {
            _stringBuilder.Clear();

            // Header
            _stringBuilder.AppendLine("═══ RimAsync Debug Info ═══");
            _stringBuilder.AppendLine();

            // Performance metrics
            AddPerformanceMetrics();
            _stringBuilder.AppendLine();

            // Cache statistics
            AddCacheStatistics();
            _stringBuilder.AppendLine();

            // Async operations info
            AddAsyncOperationsInfo();
            _stringBuilder.AppendLine();

            // Settings info
            AddSettingsInfo();

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Add performance metrics section
        /// </summary>
        private static void AddPerformanceMetrics()
        {
            _stringBuilder.AppendLine("┌─ Performance ─────────────┐");

            try
            {
                var currentTPS = PerformanceMonitor.CurrentTPS;
                var avgTPS = PerformanceMonitor.AverageTPS;
                var status = PerformanceMonitor.IsPerformanceGood ? "GOOD" : "POOR";

                _stringBuilder.AppendLine($"│ TPS: {currentTPS:F1} (avg: {avgTPS:F1})");
                _stringBuilder.AppendLine($"│ Status: {status}");

                // Show top 3 performance metrics
                var metrics = PerformanceMonitor.GetAllMetrics();
                int count = 0;
                foreach (var kvp in metrics)
                {
                    if (count >= 3) break;
                    var metric = kvp.Value;
                    _stringBuilder.AppendLine($"│ {kvp.Key}: {metric.Average:F2}{metric.Unit}");
                    count++;
                }
            }
            catch
            {
                _stringBuilder.AppendLine("│ Error reading metrics");
            }

            _stringBuilder.AppendLine("└───────────────────────────┘");
        }

        /// <summary>
        /// Add cache statistics section
        /// </summary>
        private static void AddCacheStatistics()
        {
            _stringBuilder.AppendLine("┌─ SmartCache ──────────────┐");

            try
            {
                var stats = SmartCache.GetStats();
                var hitRatePercent = stats.HitRatio * 100;

                _stringBuilder.AppendLine($"│ Entries: {stats.ValidEntries}/{stats.TotalEntries}");
                _stringBuilder.AppendLine($"│ Hit Rate: {hitRatePercent:F1}%");
                _stringBuilder.AppendLine($"│ Hits: {stats.CacheHits} | Misses: {stats.CacheMisses}");
                _stringBuilder.AppendLine($"│ Evictions: {stats.Evictions}");
            }
            catch
            {
                _stringBuilder.AppendLine("│ Cache stats unavailable");
            }

            _stringBuilder.AppendLine("└───────────────────────────┘");
        }

        /// <summary>
        /// Add async operations info section
        /// </summary>
        private static void AddAsyncOperationsInfo()
        {
            _stringBuilder.AppendLine("┌─ Async Operations ────────┐");

            try
            {
                // AsyncManager is static, no Instance property needed
                var isEnabled = AsyncManager.CanExecuteAsync();

                _stringBuilder.AppendLine($"│ Async Enabled: {(isEnabled ? "YES" : "NO")}");

                // Check multiplayer status
                var isMultiplayer = MultiplayerCompat.IsInMultiplayer;
                var asyncTimeEnabled = MultiplayerCompat.AsyncTimeEnabled;

                _stringBuilder.AppendLine($"│ Multiplayer: {(isMultiplayer ? "YES" : "NO")}");
                _stringBuilder.AppendLine($"│ AsyncTime: {(asyncTimeEnabled ? "YES" : "NO")}");
            }
            catch
            {
                _stringBuilder.AppendLine("│ Async info unavailable");
            }

            _stringBuilder.AppendLine("└───────────────────────────┘");
        }

        /// <summary>
        /// Add settings info section
        /// </summary>
        private static void AddSettingsInfo()
        {
            _stringBuilder.AppendLine("┌─ Settings ────────────────┐");

            try
            {
                var settings = RimAsyncMod.Settings;
                if (settings != null)
                {
                    _stringBuilder.AppendLine($"│ Pathfinding: {(settings.enableAsyncPathfinding ? "ON" : "OFF")}");
                    _stringBuilder.AppendLine($"│ Background Jobs: {(settings.enableBackgroundJobs ? "ON" : "OFF")}");
                    _stringBuilder.AppendLine($"│ Smart Cache: {(settings.enableSmartCaching ? "ON" : "OFF")}");
                    _stringBuilder.AppendLine($"│ Max Threads: {settings.maxAsyncThreads}");
                }
                else
                {
                    _stringBuilder.AppendLine("│ Settings not available");
                }
            }
            catch
            {
                _stringBuilder.AppendLine("│ Settings error");
            }

            _stringBuilder.AppendLine("└───────────────────────────┘");
            _stringBuilder.AppendLine();
            _stringBuilder.AppendLine("Press F11 to toggle overlay");
        }

        /// <summary>
        /// Get quick status string for logging
        /// </summary>
        public static string GetQuickStatus()
        {
            try
            {
                var tps = PerformanceMonitor.CurrentTPS;
                var cacheStats = SmartCache.GetStats();
                var asyncEnabled = AsyncManager.CanExecuteAsync();

                return $"TPS: {tps:F1} | Cache: {cacheStats.HitRatio:P0} | Async: {(asyncEnabled ? "ON" : "OFF")}";
            }
            catch
            {
                return "Status unavailable";
            }
        }

        /// <summary>
        /// Log current status to console
        /// </summary>
        public static void LogStatus()
        {
            Log.Message($"[RimAsync] {GetQuickStatus()}");
        }

        /// <summary>
        /// Get detailed report as string
        /// </summary>
        public static string GetDetailedReport()
        {
            return GenerateOverlayText();
        }
    }
}
