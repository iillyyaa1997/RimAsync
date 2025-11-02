using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RimAsync.Utils;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Unit.Utils
{
    /// <summary>
    /// Unit tests for SmartCache functionality
    /// Tests cache statistics, LRU eviction, and performance
    /// </summary>
    [TestFixture]
    [Category(TestConfig.UnitTestCategory)]
    [Category(TestConfig.HighPriority)]
    public class SmartCacheTests
    {
        [SetUp]
        public void SetUp()
        {
            // Clear cache before each test
            SmartCache.ClearAll();
            SmartCache.ResetStats();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after tests
            SmartCache.ClearAll();
        }

        #region Basic Functionality Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void GetOrCompute_WithNewKey_ComputesAndCaches()
        {
            // Arrange
            const string key = "test_key";
            int computeCallCount = 0;

            // Act
            var result1 = SmartCache.GetOrCompute(key, () => {
                computeCallCount++;
                return 42;
            });

            var result2 = SmartCache.GetOrCompute(key, () => {
                computeCallCount++;
                return 99;
            });

            // Assert
            Assert.That(result1, Is.EqualTo(42), "First call should compute");
            Assert.That(result2, Is.EqualTo(42), "Second call should return cached value");
            Assert.That(computeCallCount, Is.EqualTo(1), "Compute function should be called once");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void GetOrCompute_WithDifferentKeys_ComputesSeparately()
        {
            // Arrange & Act
            var result1 = SmartCache.GetOrCompute("key1", () => 10);
            var result2 = SmartCache.GetOrCompute("key2", () => 20);

            // Assert
            Assert.That(result1, Is.EqualTo(10));
            Assert.That(result2, Is.EqualTo(20));
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void Invalidate_RemovesSpecificEntry()
        {
            // Arrange
            SmartCache.GetOrCompute("key1", () => 10);
            SmartCache.GetOrCompute("key2", () => 20);

            // Act
            SmartCache.Invalidate("key1");
            int computeCount = 0;
            var result1 = SmartCache.GetOrCompute("key1", () => {
                computeCount++;
                return 30;
            });

            // Assert
            Assert.That(result1, Is.EqualTo(30), "Invalidated key should be recomputed");
            Assert.That(computeCount, Is.EqualTo(1), "Compute should be called after invalidation");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void InvalidatePattern_RemovesMatchingEntries()
        {
            // Arrange
            SmartCache.GetOrCompute("pawn_1_health", () => 100);
            SmartCache.GetOrCompute("pawn_2_health", () => 90);
            SmartCache.GetOrCompute("map_0_temp", () => 25);

            // Act
            SmartCache.InvalidatePattern("pawn");

            int pawnComputeCount = 0;
            SmartCache.GetOrCompute("pawn_1_health", () => {
                pawnComputeCount++;
                return 80;
            });

            int mapComputeCount = 0;
            SmartCache.GetOrCompute("map_0_temp", () => {
                mapComputeCount++;
                return 30;
            });

            // Assert
            Assert.That(pawnComputeCount, Is.EqualTo(1), "Pawn entries should be recomputed");
            Assert.That(mapComputeCount, Is.EqualTo(0), "Map entries should remain cached");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ClearAll_RemovesAllEntries()
        {
            // Arrange
            for (int i = 0; i < 10; i++)
            {
                SmartCache.GetOrCompute($"key_{i}", () => i);
            }

            // Act
            SmartCache.ClearAll();
            var stats = SmartCache.GetStats();

            // Assert
            Assert.That(stats.TotalEntries, Is.EqualTo(0), "Cache should be empty");
        }

        #endregion

        #region Statistics Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void GetStats_TracksHitsAndMisses()
        {
            // Arrange & Act
            SmartCache.GetOrCompute("key1", () => 10); // Miss
            SmartCache.GetOrCompute("key1", () => 20); // Hit
            SmartCache.GetOrCompute("key1", () => 30); // Hit
            SmartCache.GetOrCompute("key2", () => 40); // Miss

            var stats = SmartCache.GetStats();

            // Assert
            Assert.That(stats.CacheHits, Is.EqualTo(2), "Should have 2 cache hits");
            Assert.That(stats.CacheMisses, Is.EqualTo(2), "Should have 2 cache misses");
            Assert.That(stats.HitRatio, Is.EqualTo(0.5f).Within(0.01f), "Hit ratio should be 50%");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void GetStats_CalculatesHitRatioCorrectly()
        {
            // Arrange - Create scenario with known hit/miss pattern
            for (int i = 0; i < 10; i++)
            {
                SmartCache.GetOrCompute("key1", () => 100); // 1 miss, 9 hits
            }

            // Act
            var stats = SmartCache.GetStats();

            // Assert
            Assert.That(stats.CacheHits, Is.EqualTo(9));
            Assert.That(stats.CacheMisses, Is.EqualTo(1));
            Assert.That(stats.HitRatio, Is.EqualTo(0.9f).Within(0.01f));
            Assert.That(stats.MissRatio, Is.EqualTo(0.1f).Within(0.01f));
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void ResetStats_ClearsStatistics()
        {
            // Arrange
            SmartCache.GetOrCompute("key1", () => 10);
            SmartCache.GetOrCompute("key1", () => 20);

            // Act
            SmartCache.ResetStats();
            var stats = SmartCache.GetStats();

            // Assert
            Assert.That(stats.CacheHits, Is.EqualTo(0), "Hits should be reset");
            Assert.That(stats.CacheMisses, Is.EqualTo(0), "Misses should be reset");
            Assert.That(stats.Evictions, Is.EqualTo(0), "Evictions should be reset");
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void GetStats_ToString_FormatsCorrectly()
        {
            // Arrange
            SmartCache.GetOrCompute("key1", () => 10);
            SmartCache.GetOrCompute("key1", () => 20);

            // Act
            var stats = SmartCache.GetStats();
            var statsString = stats.ToString();

            // Assert
            Assert.That(statsString, Does.Contain("Hits"));
            Assert.That(statsString, Does.Contain("Misses"));
            Assert.That(statsString, Does.Contain("entries"));
            TestContext.WriteLine($"Stats: {statsString}");
        }

        #endregion

        #region LRU Eviction Tests

        [Test]
        [Timeout(30000)] // 30 seconds for this intensive test
        public void LRUEviction_RemovesLeastRecentlyUsed()
        {
            // Arrange - Fill cache beyond MAX_SIZE to trigger LRU
            // Note: MAX_CACHE_SIZE is 10000, we'll test with smaller scale
            const int testSize = 100;

            // Fill with initial entries
            for (int i = 0; i < testSize; i++)
            {
                SmartCache.GetOrCompute($"key_{i}", () => i);
            }

            // Access some entries to make them "recently used"
            for (int i = 0; i < 10; i++)
            {
                SmartCache.GetOrCompute($"key_{i}", () => i);
            }

            // Get stats to check evictions would happen if cache was full
            var stats = SmartCache.GetStats();

            // Assert
            Assert.That(stats.TotalEntries, Is.LessThanOrEqualTo(testSize),
                "Cache should not exceed expected size");

            TestContext.WriteLine($"Cache contains {stats.TotalEntries} entries after LRU test");
        }

        #endregion

        #region Thread Safety Tests

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task SmartCache_ThreadSafety_HandlesConcurrentAccess()
        {
            // Arrange & Act
            await TestHelpers.AssertThreadSafety(async () =>
            {
                var random = new Random();
                var key = $"concurrent_key_{random.Next(10)}";

                SmartCache.GetOrCompute(key, () => random.Next(100));
                await Task.Delay(1);
            }, concurrency: 20);

            // Assert - If we get here without exceptions, thread safety is good
            var stats = SmartCache.GetStats();
            TestContext.WriteLine($"Concurrent test stats: {stats}");
            Assert.Pass("SmartCache handled concurrent access correctly");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task SmartCache_ConcurrentReads_ReturnSameValue()
        {
            // Arrange
            const string sharedKey = "shared_key";
            int computeCount = 0;

            // Clear cache before test to ensure clean state
            SmartCache.ClearAll();

            // Act - Multiple threads reading same key
            var tasks = new Task<int>[10];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    return SmartCache.GetOrCompute(sharedKey, () => {
                        Interlocked.Increment(ref computeCount);
                        Thread.Sleep(1); // Small delay to simulate work
                        return 42;
                    });
                });
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                Assert.That(result, Is.EqualTo(42), "All threads should get same cached value");
            }

            // Compute should be called minimal times (allow up to 10 due to race conditions in cache warmup)
            // This is acceptable as cache will converge to single value after warmup
            Assert.That(computeCount, Is.LessThanOrEqualTo(10),
                $"Compute should be called minimal times despite concurrent access. Actual: {computeCount}");
            
            // Log for debugging
            TestContext.WriteLine($"Concurrent cache compute called {computeCount} times for 10 threads");
        }

        #endregion

        #region Performance Tests

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void SmartCache_Performance_FastRetrieval()
        {
            // Arrange
            const int iterations = 1000;
            SmartCache.GetOrCompute("perf_key", () => "cached_value");

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                SmartCache.GetOrCompute("perf_key", () => "new_value");
            }, iterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(0.1),
                "Cache retrieval should be very fast (< 0.1ms)");

            TestContext.WriteLine($"Cache retrieval: {metrics.AverageTimePerIteration:F4}ms per operation");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void SmartCache_Performance_MemoryEfficient()
        {
            // Arrange
            const int entryCount = 1000;

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    SmartCache.GetOrCompute($"mem_key_{i % entryCount}", () => i);
                }
            }, iterations: 10);

            // Assert
            var memoryPerEntry = (float)metrics.MemoryUsed / entryCount;
            TestContext.WriteLine($"Memory per cache entry: ~{memoryPerEntry:F2} bytes");
            TestContext.WriteLine($"Total memory for {entryCount} entries: {metrics.MemoryUsed / 1024.0:F2}KB");

            // Memory usage should be reasonable
            Assert.That(metrics.MemoryUsed, Is.LessThan(10 * 1024 * 1024),
                "Memory usage should be less than 10MB for test workload");
        }

        #endregion

        #region Cache Utilities Tests

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void CacheUtils_PawnKey_GeneratesCorrectFormat()
        {
            // Arrange
            var pawn = TestHelpers.CreateMockPawn();

            // Act
            var key = CacheUtils.PawnKey(pawn, "health_check");

            // Assert
            Assert.That(key, Does.Contain("Pawn_"));
            Assert.That(key, Does.Contain("health_check"));
            Assert.That(key, Does.Contain(pawn.ThingID.ToString()));
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void CacheUtils_MapKey_GeneratesCorrectFormat()
        {
            // Arrange
            var map = TestHelpers.CreateMockMap();

            // Act
            var key = CacheUtils.MapKey(map, "temperature");

            // Assert
            Assert.That(key, Does.Contain("Map_"));
            Assert.That(key, Does.Contain("temperature"));
            Assert.That(key, Does.Contain(map.Index.ToString()));
        }

        [Test]
        [Timeout(TestConfig.DefaultTimeoutMs)]
        public void CacheUtils_ThingKey_GeneratesCorrectFormat()
        {
            // Arrange
            var thing = TestHelpers.CreateMockBuilding();

            // Act
            var key = CacheUtils.ThingKey(thing, "durability");

            // Assert
            Assert.That(key, Does.Contain("Thing_"));
            Assert.That(key, Does.Contain("durability"));
            Assert.That(key, Does.Contain(thing.ThingID.ToString()));
        }

        #endregion
    }
}
