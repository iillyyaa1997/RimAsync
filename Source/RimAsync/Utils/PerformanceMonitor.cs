using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimAsync.Core;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Monitors performance metrics and automatically adjusts optimizations
    /// Provides TPS tracking and performance statistics
    /// </summary>
    public static class PerformanceMonitor
    {
        private static bool _initialized = false;
        private static readonly Queue<float> _tpsHistory = new Queue<float>();
        private static readonly Stopwatch _frameTimer = new Stopwatch();
        private static readonly Dictionary<string, PerformanceMetric> _metrics = new Dictionary<string, PerformanceMetric>();
        
        private static float _lastTps = 60.0f;
        private static int _frameCount = 0;
        private static float _totalFrameTime = 0.0f;
        
        private const int TPS_HISTORY_SIZE = 300; // 5 seconds at 60 TPS
        private const float TPS_UPDATE_INTERVAL = 1.0f; // Update TPS every second

        /// <summary>
        /// Current TPS (Ticks Per Second)
        /// </summary>
        public static float CurrentTPS => _lastTps;

        /// <summary>
        /// Average TPS over the last few seconds
        /// </summary>
        public static float AverageTPS
        {
            get
            {
                if (_tpsHistory.Count == 0) return 60.0f;
                
                float sum = 0;
                foreach (var tps in _tpsHistory)
                {
                    sum += tps;
                }
                return sum / _tpsHistory.Count;
            }
        }

        /// <summary>
        /// True if performance is currently good (high TPS)
        /// </summary>
        public static bool IsPerformanceGood => CurrentTPS > 50.0f;

        /// <summary>
        /// Initialize performance monitoring
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                Log.Message("[RimAsync] Initializing performance monitor...");

                _frameTimer.Start();
                _initialized = true;

                Log.Message("[RimAsync] Performance monitor initialized");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Failed to initialize performance monitor: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Called when settings change
        /// </summary>
        public static void OnSettingsChanged()
        {
            if (!_initialized) return;

            // Reset metrics when settings change
            _metrics.Clear();
            _tpsHistory.Clear();
        }

        /// <summary>
        /// Shutdown performance monitoring
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized) return;

            try
            {
                Log.Message("[RimAsync] Shutting down performance monitor...");

                _frameTimer.Stop();
                _metrics.Clear();
                _tpsHistory.Clear();

                _initialized = false;
                Log.Message("[RimAsync] Performance monitor shut down");
            }
            catch (Exception ex)
            {
                Log.Error($"[RimAsync] Error during performance monitor shutdown: {ex}");
            }
        }

        /// <summary>
        /// Update performance metrics (called each game tick)
        /// </summary>
        public static void UpdateMetrics()
        {
            if (!_initialized) return;

            try
            {
                _frameCount++;
                var elapsed = (float)_frameTimer.Elapsed.TotalSeconds;
                _totalFrameTime += elapsed;
                _frameTimer.Restart();

                // Update TPS every second
                if (_totalFrameTime >= TPS_UPDATE_INTERVAL)
                {
                    _lastTps = _frameCount / _totalFrameTime;
                    
                    // Add to history
                    _tpsHistory.Enqueue(_lastTps);
                    while (_tpsHistory.Count > TPS_HISTORY_SIZE)
                    {
                        _tpsHistory.Dequeue();
                    }

                    // Reset counters
                    _frameCount = 0;
                    _totalFrameTime = 0.0f;

                    // Auto-adjust optimizations based on performance
                    AutoAdjustOptimizations();
                }
            }
            catch (Exception ex)
            {
                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Error($"[RimAsync] Error updating performance metrics: {ex}");
                }
            }
        }

        /// <summary>
        /// Start measuring performance for a specific operation
        /// </summary>
        public static IDisposable StartMeasuring(string operationName)
        {
            if (!_initialized) return null;

            return new PerformanceMeasurement(operationName);
        }

        /// <summary>
        /// Record a performance metric
        /// </summary>
        public static void RecordMetric(string name, float value, string unit = "ms")
        {
            if (!_initialized) return;

            if (!_metrics.ContainsKey(name))
            {
                _metrics[name] = new PerformanceMetric(name, unit);
            }

            _metrics[name].AddValue(value);
        }

        /// <summary>
        /// Get a performance metric by name
        /// </summary>
        public static PerformanceMetric GetMetric(string name)
        {
            _metrics.TryGetValue(name, out var metric);
            return metric;
        }

        /// <summary>
        /// Get all recorded metrics
        /// </summary>
        public static Dictionary<string, PerformanceMetric> GetAllMetrics()
        {
            return new Dictionary<string, PerformanceMetric>(_metrics);
        }

        /// <summary>
        /// Automatically adjust optimizations based on current performance
        /// </summary>
        private static void AutoAdjustOptimizations()
        {
            if (!RimAsyncMod.Settings.enablePerformanceMonitoring) return;

            var settings = RimAsyncMod.Settings;
            var avgTps = AverageTPS;

            // If performance is poor, we might want to disable some optimizations
            // to reduce overhead until performance improves
            if (avgTps < 30.0f)
            {
                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Warning($"[RimAsync] Low TPS detected ({avgTps:F1}), performance monitoring active");
                }
            }
            else if (avgTps > 55.0f)
            {
                if (RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Message($"[RimAsync] Good TPS performance ({avgTps:F1})");
                }
            }
        }

        /// <summary>
        /// Get a summary of current performance status
        /// </summary>
        public static string GetPerformanceSummary()
        {
            if (!_initialized) return "Performance monitor not initialized";

            return $"TPS: {CurrentTPS:F1} (avg: {AverageTPS:F1}), Status: {(IsPerformanceGood ? "Good" : "Poor")}";
        }
    }

    /// <summary>
    /// Represents a performance metric with statistics
    /// </summary>
    public class PerformanceMetric
    {
        public string Name { get; }
        public string Unit { get; }
        
        private readonly Queue<float> _values = new Queue<float>();
        private const int MAX_VALUES = 100;

        public PerformanceMetric(string name, string unit)
        {
            Name = name;
            Unit = unit;
        }

        public void AddValue(float value)
        {
            _values.Enqueue(value);
            while (_values.Count > MAX_VALUES)
            {
                _values.Dequeue();
            }
        }

        public float Average
        {
            get
            {
                if (_values.Count == 0) return 0;
                float sum = 0;
                foreach (var value in _values)
                {
                    sum += value;
                }
                return sum / _values.Count;
            }
        }

        public float Minimum
        {
            get
            {
                if (_values.Count == 0) return 0;
                float min = float.MaxValue;
                foreach (var value in _values)
                {
                    if (value < min) min = value;
                }
                return min;
            }
        }

        public float Maximum
        {
            get
            {
                if (_values.Count == 0) return 0;
                float max = float.MinValue;
                foreach (var value in _values)
                {
                    if (value > max) max = value;
                }
                return max;
            }
        }

        public int SampleCount => _values.Count;

        public override string ToString()
        {
            return $"{Name}: {Average:F2}{Unit} (min: {Minimum:F2}, max: {Maximum:F2}, samples: {SampleCount})";
        }
    }

    /// <summary>
    /// Disposable performance measurement helper
    /// </summary>
    public class PerformanceMeasurement : IDisposable
    {
        private readonly string _operationName;
        private readonly Stopwatch _stopwatch;

        public PerformanceMeasurement(string operationName)
        {
            _operationName = operationName;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            PerformanceMonitor.RecordMetric(_operationName, (float)_stopwatch.Elapsed.TotalMilliseconds);
        }
    }
} 