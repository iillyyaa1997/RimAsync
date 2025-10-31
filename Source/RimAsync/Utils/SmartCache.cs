using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using RimAsync.Core;
using Verse;

namespace RimAsync.Utils
{
    /// <summary>
    /// Smart caching system for frequently computed values
    /// Thread-safe and multiplayer-compatible
    /// </summary>
    public static class SmartCache
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry>();
        private static readonly object _cleanupLock = new object();
        private static int _lastCleanupTick = 0;

        private const int CLEANUP_INTERVAL = 3600; // Clean up every minute (60 ticks/second * 60 seconds)
        private const int DEFAULT_TTL_TICKS = 600; // Default 10 second TTL

        /// <summary>
        /// Get or compute a cached value
        /// </summary>
        public static T GetOrCompute<T>(string key, Func<T> computeFunc, int ttlTicks = DEFAULT_TTL_TICKS)
        {
            // Check if Settings is available (may be null in tests)
            if (RimAsyncMod.Settings != null && !RimAsyncMod.Settings.enableSmartCaching)
            {
                // Caching disabled, compute directly
                return computeFunc();
            }

            var currentTick = Find.TickManager?.TicksGame ?? 0;

            // Try to get from cache
            if (_cache.TryGetValue(key, out var entry))
            {
                if (currentTick - entry.CreatedTick < ttlTicks)
                {
                    // Cache hit and still valid
                    try
                    {
                        return (T)entry.Value;
                    }
                    catch (InvalidCastException ex)
                    {
                        Log.Warning($"[RimAsync] Cache type mismatch for key {key}: {ex.Message}");
                        _cache.TryRemove(key, out _);
                    }
                }
                else
                {
                    // Cache expired
                    _cache.TryRemove(key, out _);
                }
            }

            // Cache miss or expired, compute new value
            using (PerformanceMonitor.StartMeasuring($"Cache.Compute.{key}"))
            {
                var value = computeFunc();

                // Store in cache
                var newEntry = new CacheEntry
                {
                    Value = value,
                    CreatedTick = currentTick
                };

                _cache.TryAdd(key, newEntry);

                // Periodic cleanup
                if (currentTick - _lastCleanupTick > CLEANUP_INTERVAL)
                {
                    CleanupExpiredEntries(currentTick);
                }

                return value;
            }
        }

        /// <summary>
        /// Invalidate a specific cache entry
        /// </summary>
        public static void Invalidate(string key)
        {
            _cache.TryRemove(key, out _);
        }

        /// <summary>
        /// Invalidate all cache entries matching a pattern
        /// </summary>
        public static void InvalidatePattern(string pattern)
        {
            var keysToRemove = new List<string>();

            foreach (var kvp in _cache)
            {
                if (kvp.Key.Contains(pattern))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _cache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Clear all cache entries
        /// </summary>
        public static void ClearAll()
        {
            _cache.Clear();
            Log.Message("[RimAsync] Smart cache cleared");
        }

        /// <summary>
        /// Get cache statistics
        /// </summary>
        public static CacheStats GetStats()
        {
            var currentTick = Find.TickManager?.TicksGame ?? 0;
            int validEntries = 0;
            int expiredEntries = 0;

            foreach (var entry in _cache.Values)
            {
                if (currentTick - entry.CreatedTick < DEFAULT_TTL_TICKS)
                {
                    validEntries++;
                }
                else
                {
                    expiredEntries++;
                }
            }

            return new CacheStats
            {
                TotalEntries = _cache.Count,
                ValidEntries = validEntries,
                ExpiredEntries = expiredEntries
            };
        }

        /// <summary>
        /// Clean up expired cache entries
        /// </summary>
        private static void CleanupExpiredEntries(int currentTick)
        {
            if (!Monitor.TryEnter(_cleanupLock, 0))
            {
                return; // Another thread is already cleaning up
            }

            try
            {
                var keysToRemove = new List<string>();

                foreach (var kvp in _cache)
                {
                    if (currentTick - kvp.Value.CreatedTick > DEFAULT_TTL_TICKS * 2) // Extra buffer
                    {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                int removedCount = 0;
                foreach (var key in keysToRemove)
                {
                    if (_cache.TryRemove(key, out _))
                    {
                        removedCount++;
                    }
                }

                _lastCleanupTick = currentTick;

                if (removedCount > 0 && RimAsyncMod.Settings?.enableDebugLogging == true)
                {
                    Log.Message($"[RimAsync] Cleaned up {removedCount} expired cache entries");
                }
            }
            finally
            {
                Monitor.Exit(_cleanupLock);
            }
        }

        /// <summary>
        /// Cache entry with creation time
        /// </summary>
        private class CacheEntry
        {
            public object Value { get; set; }
            public int CreatedTick { get; set; }
        }
    }

    /// <summary>
    /// Cache statistics
    /// </summary>
    public class CacheStats
    {
        public int TotalEntries { get; set; }
        public int ValidEntries { get; set; }
        public int ExpiredEntries { get; set; }

        public float HitRatio => TotalEntries > 0 ? (float)ValidEntries / TotalEntries : 0f;

        public override string ToString()
        {
            return $"Cache: {ValidEntries}/{TotalEntries} valid ({HitRatio:P1}), {ExpiredEntries} expired";
        }
    }

    /// <summary>
    /// Cache utilities for common RimWorld operations
    /// </summary>
    public static class CacheUtils
    {
        /// <summary>
        /// Create a cache key for pawn-related operations
        /// </summary>
        public static string PawnKey(Verse.Pawn pawn, string operation)
        {
            return $"Pawn_{pawn.ThingID}_{operation}";
        }

        /// <summary>
        /// Create a cache key for map-related operations
        /// </summary>
        public static string MapKey(Verse.Map map, string operation)
        {
            return $"Map_{map.Index}_{operation}";
        }

        /// <summary>
        /// Create a cache key for thing-related operations
        /// </summary>
        public static string ThingKey(Verse.Thing thing, string operation)
        {
            return $"Thing_{thing.ThingID}_{operation}";
        }
    }
}
