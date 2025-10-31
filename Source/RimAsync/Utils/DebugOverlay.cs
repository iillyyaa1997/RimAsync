using System;
using System.Text;
using UnityEngine;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Debug overlay for displaying real-time performance metrics
    /// Shows TPS, cache statistics, async operations, and more
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
        /// </summary>
        public static void OnGUI()
        {
            if (!_enabled) return;

            try
            {
                // Update cached text periodically to reduce performance impact
                var currentTime = Time.realtimeSinceStartup;
                if (currentTime - _lastUpdateTime > UPDATE_INTERVAL)
                {
                    _cachedOverlayText = GenerateOverlayText();
                    _lastUpdateTime = currentTime;
                }

                // Draw overlay in top-right corner
                var rect = new Rect(Screen.width - 420, 10, 410, 300);

                // Background
                Widgets.DrawBoxSolid(rect, new Color(0, 0, 0, 0.8f));

                // Border
                Widgets.DrawBox(rect, 2);

                // Text
                var textRect = rect.ContractedBy(10);
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.Label(textRect, _cachedOverlayText);
                Text.Anchor = TextAnchor.UpperLeft;
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error rendering debug overlay: {ex}");
            }
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
                var statusColor = PerformanceMonitor.IsPerformanceGood ? "green" : "red";

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
                var hitRateColor = hitRatePercent > 70 ? "green" : (hitRatePercent > 40 ? "yellow" : "red");

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
                var asyncManager = RimAsync.Threading.AsyncManager.Instance;
                if (asyncManager != null)
                {
                    var activeCount = asyncManager.ActiveTaskCount;
                    var isEnabled = RimAsync.Threading.AsyncManager.CanExecuteAsync();

                    _stringBuilder.AppendLine($"│ Active Tasks: {activeCount}");
                    _stringBuilder.AppendLine($"│ Async Enabled: {(isEnabled ? "YES" : "NO")}");

                    // Execution mode
                    var mode = RimAsync.Core.RimAsyncCore.CurrentExecutionMode;
                    _stringBuilder.AppendLine($"│ Mode: {mode}");
                }
                else
                {
                    _stringBuilder.AppendLine("│ AsyncManager not initialized");
                }
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
                    _stringBuilder.AppendLine($"│ AI: {(settings.enableAsyncAI ? "ON" : "OFF")}");
                    _stringBuilder.AppendLine($"│ Building: {(settings.enableAsyncBuilding ? "ON" : "OFF")}");
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
                var asyncTasks = RimAsync.Threading.AsyncManager.Instance?.ActiveTaskCount ?? 0;

                return $"TPS: {tps:F1} | Cache: {cacheStats.HitRatio:P0} | Async: {asyncTasks}";
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
