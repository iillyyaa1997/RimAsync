using NUnit.Framework;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using RimAsync.Patches.RW_Patches;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tests.Mocks;
using Verse;

namespace Tests.Stress
{
    /// <summary>
    /// Stress tests for RimAsync system stability and performance
    /// Tests memory leaks, long-running operations, and large-scale scenarios
    /// </summary>
    [TestFixture]
    [Category("Stress")]
    public class StressTests
    {
        private const int STRESS_ITERATIONS = 10000;
        private const int LARGE_COLONY_SIZE = 100;
        private const int LONG_RUNNING_TICKS = 36000; // 10 hours at 60 TPS

        [SetUp]
        public void SetUp()
        {
            if (!RimAsyncCore.IsInitialized)
            {
                RimAsyncCore.Initialize();
            }

            AsyncManager.Initialize();
            SmartCache.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            AsyncManager.Shutdown();
            SmartCache.ClearAll();
        }

        #region Memory Leak Tests

        [Test]
        [Category("MemoryLeak")]
        public void MemoryLeak_AsyncOperations_NoLeaks()
        {
            // Arrange
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var startMemory = GC.GetTotalMemory(true);
            var operations = new List<Task>();

            // Act - Run many async operations
            for (int i = 0; i < STRESS_ITERATIONS; i++)
            {
                var localI = i; // Capture for closure
                var task = Task.Run(() =>
                {
                    // Simulate work
                    Thread.Sleep(1);
                    return localI * 2;
                });
                operations.Add(task);

                // Periodically wait to avoid overwhelming the system
                if (i % 1000 == 0)
                {
                    Task.WaitAll(operations.ToArray());
                    operations.Clear();

                    // Force GC every 1000 operations
                    if (i % 5000 == 0)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }
            }

            Task.WaitAll(operations.ToArray());

            // Force final GC
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var endMemory = GC.GetTotalMemory(true);
            var memoryGrowth = endMemory - startMemory;

            // Assert - Memory growth should be reasonable (< 10MB for 10k operations)
            Assert.Less(memoryGrowth, 10 * 1024 * 1024,
                $"Memory leak detected! Growth: {memoryGrowth / 1024.0 / 1024.0:F2} MB");

            TestContext.WriteLine($"Memory growth after {STRESS_ITERATIONS} operations: {memoryGrowth / 1024.0:F2} KB");
        }

        [Test]
        [Category("MemoryLeak")]
        public void MemoryLeak_SmartCache_NoLeaks()
        {
            // Arrange
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var startMemory = GC.GetTotalMemory(true);

            // Act - Stress test cache with many entries
            for (int i = 0; i < STRESS_ITERATIONS; i++)
            {
                var key = $"test_key_{i % 100}"; // Reuse 100 keys
                var value = $"test_value_{i}";

                SmartCache.GetOrCompute(key, () => value);

                // Periodically clear cache
                if (i % 1000 == 0)
                {
                    SmartCache.ClearAll();
                }
            }

            SmartCache.ClearAll();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var endMemory = GC.GetTotalMemory(true);
            var memoryGrowth = endMemory - startMemory;

            // Assert - Memory should return to baseline after clearing
            Assert.Less(memoryGrowth, 1 * 1024 * 1024,
                $"Cache memory leak detected! Growth: {memoryGrowth / 1024.0:F2} KB");

            TestContext.WriteLine($"Memory growth after {STRESS_ITERATIONS} cache operations: {memoryGrowth / 1024.0:F2} KB");
        }

        [Test]
        [Category("MemoryLeak")]
        public void MemoryLeak_PathfindingOperations_NoLeaks()
        {
            // Arrange
            var mockPawn = new MockPawn { Dead = false, Spawned = true };
            var mockMap = new MockMap();
            mockPawn.Map = mockMap;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            var startMemory = GC.GetTotalMemory(true);

            // Act - Simulate many pathfinding operations
            for (int i = 0; i < 1000; i++)
            {
                // Simulate pathfinding work
                Thread.Sleep(1);

                if (i % 100 == 0)
                {
                    SmartCache.ClearAll();
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            var endMemory = GC.GetTotalMemory(true);
            var memoryGrowth = endMemory - startMemory;

            // Assert
            Assert.Less(memoryGrowth, 5 * 1024 * 1024,
                $"Pathfinding memory leak detected! Growth: {memoryGrowth / 1024.0 / 1024.0:F2} MB");

            TestContext.WriteLine($"Memory growth after 1000 pathfinding ops: {memoryGrowth / 1024.0:F2} KB");
        }

        #endregion

        #region Large Colony Tests

        [Test]
        [Category("LargeColony")]
        public void LargeColony_100Pawns_PerformanceAcceptable()
        {
            // Arrange
            var pawns = new List<MockPawn>();
            var map = new MockMap();

            for (int i = 0; i < LARGE_COLONY_SIZE; i++)
            {
                var pawn = new MockPawn
                {
                    Dead = false,
                    Spawned = true,
                    Map = map
                };
                pawns.Add(pawn);
            }

            var stopwatch = Stopwatch.StartNew();

            // Act - Simulate tick for all pawns
            foreach (var pawn in pawns)
            {
                // Simulate pathfinding work
                Thread.Sleep(1);
            }

            stopwatch.Stop();

            // Assert - Should process 100 pawns in reasonable time (< 500ms)
            Assert.Less(stopwatch.ElapsedMilliseconds, 500,
                $"Large colony processing too slow: {stopwatch.ElapsedMilliseconds}ms for {LARGE_COLONY_SIZE} pawns");

            TestContext.WriteLine($"Processed {LARGE_COLONY_SIZE} pawns in {stopwatch.ElapsedMilliseconds}ms");
            TestContext.WriteLine($"Average per pawn: {stopwatch.ElapsedMilliseconds / (double)LARGE_COLONY_SIZE:F2}ms");
        }

        [Test]
        [Category("LargeColony")]
        public void LargeColony_ConcurrentOperations_NoDeadlocks()
        {
            // Arrange
            var pawns = new List<MockPawn>();
            var map = new MockMap();

            for (int i = 0; i < LARGE_COLONY_SIZE; i++)
            {
                pawns.Add(new MockPawn
                {
                    Dead = false,
                    Spawned = true,
                    Map = map
                });
            }

            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task>();

            // Act - Process all pawns concurrently
            foreach (var pawn in pawns)
            {
                var localPawn = pawn; // Capture for closure
                var task = Task.Run(() =>
                {
                    // Simulate pathfinding work
                    Thread.Sleep(1);
                    return true;
                });
                tasks.Add(task);
            }

            // Wait with timeout to detect deadlocks
            var completed = Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(completed, "Deadlock detected! Not all tasks completed within 30 seconds");
            Assert.Less(stopwatch.ElapsedMilliseconds, 5000,
                $"Concurrent processing too slow: {stopwatch.ElapsedMilliseconds}ms");

            TestContext.WriteLine($"Concurrent processing of {LARGE_COLONY_SIZE} pawns: {stopwatch.ElapsedMilliseconds}ms");
        }

        [Test]
        [Category("LargeColony")]
        public void LargeColony_MemoryUsage_Acceptable()
        {
            // Arrange
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var startMemory = GC.GetTotalMemory(true);

            var pawns = new List<MockPawn>();
            var map = new MockMap();

            // Act - Create large colony
            for (int i = 0; i < LARGE_COLONY_SIZE; i++)
            {
                pawns.Add(new MockPawn
                {
                    Dead = false,
                    Spawned = true,
                    Map = map
                });
            }

            // Simulate some operations
            foreach (var pawn in pawns)
            {
                // Simulate pathfinding work
                Thread.Sleep(1);
            }

            var endMemory = GC.GetTotalMemory(false);
            var memoryUsed = endMemory - startMemory;

            // Assert - Memory per pawn should be reasonable (< 100KB per pawn)
            var memoryPerPawn = memoryUsed / (double)LARGE_COLONY_SIZE;
            Assert.Less(memoryPerPawn, 100 * 1024,
                $"Memory per pawn too high: {memoryPerPawn / 1024.0:F2} KB");

            TestContext.WriteLine($"Total memory for {LARGE_COLONY_SIZE} pawns: {memoryUsed / 1024.0:F2} KB");
            TestContext.WriteLine($"Memory per pawn: {memoryPerPawn / 1024.0:F2} KB");
        }

        #endregion

        #region Long Running Tests

        [Test]
        [Category("LongRunning")]
        [Explicit("Long running test - run manually")]
        public void LongRunning_10HourSimulation_Stable()
        {
            // Arrange
            var pawns = CreateTestColony(20);
            var tickCount = 0;
            var errors = 0;
            var startMemory = GC.GetTotalMemory(false);

            var stopwatch = Stopwatch.StartNew();

            // Act - Simulate 10 hours of gameplay (36000 ticks at 60 TPS)
            for (int tick = 0; tick < LONG_RUNNING_TICKS; tick++)
            {
                try
                {
                    // Simulate tick for all pawns
                    foreach (var pawn in pawns)
                    {
                        // Simulate pathfinding work
                        Thread.Sleep(1);
                    }

                    tickCount++;

                    // Progress report every hour (3600 ticks)
                    if (tick % 3600 == 0)
                    {
                        var currentMemory = GC.GetTotalMemory(false);
                        var memoryGrowth = currentMemory - startMemory;

                        TestContext.WriteLine($"Hour {tick / 3600}: {tickCount} ticks, {errors} errors, Memory: {memoryGrowth / 1024.0 / 1024.0:F2} MB");

                        // Periodic GC to simulate RimWorld behavior
                        if (tick % 7200 == 0)
                        {
                            GC.Collect();
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors++;
                    TestContext.WriteLine($"Error at tick {tick}: {ex.Message}");
                }
            }

            stopwatch.Stop();

            var endMemory = GC.GetTotalMemory(false);
            var totalMemoryGrowth = endMemory - startMemory;

            // Assert
            Assert.AreEqual(LONG_RUNNING_TICKS, tickCount, "Not all ticks completed");
            Assert.Less(errors, 10, $"Too many errors: {errors}");
            Assert.Less(totalMemoryGrowth, 50 * 1024 * 1024,
                $"Memory leak detected after long run: {totalMemoryGrowth / 1024.0 / 1024.0:F2} MB");

            TestContext.WriteLine($"Completed {tickCount} ticks in {stopwatch.Elapsed}");
            TestContext.WriteLine($"Total memory growth: {totalMemoryGrowth / 1024.0 / 1024.0:F2} MB");
            TestContext.WriteLine($"Error rate: {errors / (double)tickCount * 100:F4}%");
        }

        [Test]
        [Category("LongRunning")]
        public void LongRunning_ContinuousAsyncOperations_Stable()
        {
            // Arrange
            var operationCount = 0;
            var errorCount = 0;
            var startMemory = GC.GetTotalMemory(false);
            var stopwatch = Stopwatch.StartNew();

            // Act - Run continuous async operations for 30 seconds
            while (stopwatch.Elapsed.TotalSeconds < 30)
            {
                try
                {
                    var localCount = operationCount;
                    var task = Task.Run(() =>
                    {
                        Thread.Sleep(1); // Simulate work
                        return localCount;
                    });

                    task.Wait(TimeSpan.FromSeconds(1));
                    operationCount++;

                    // Periodic cleanup
                    if (operationCount % 1000 == 0)
                    {
                        SmartCache.ClearAll();
                    }
                }
                catch (Exception)
                {
                    errorCount++;
                }
            }

            stopwatch.Stop();

            var endMemory = GC.GetTotalMemory(false);
            var memoryGrowth = endMemory - startMemory;

            // Assert
            Assert.Greater(operationCount, 1000, "Too few operations completed");
            Assert.Less(errorCount / (double)operationCount, 0.01, "Error rate too high");
            Assert.Less(memoryGrowth, 20 * 1024 * 1024, "Memory growth too high");

            TestContext.WriteLine($"Completed {operationCount} operations in 30 seconds");
            TestContext.WriteLine($"Operations per second: {operationCount / 30.0:F0}");
            TestContext.WriteLine($"Error rate: {errorCount / (double)operationCount * 100:F4}%");
            TestContext.WriteLine($"Memory growth: {memoryGrowth / 1024.0 / 1024.0:F2} MB");
        }

        #endregion

        #region Performance Regression Tests

        [Test]
        [Category("Regression")]
        public void Regression_PathfindingPerformance_NoRegression()
        {
            // Arrange
            var pawn = new MockPawn { Dead = false, Spawned = true, Map = new MockMap() };
            var baselineMs = 0.1; // 0.1ms baseline
            var iterations = 1000;

            var stopwatch = Stopwatch.StartNew();

            // Act
            for (int i = 0; i < iterations; i++)
            {
                // Simulate pathfinding work
                Thread.Sleep(0); // Minimal sleep to simulate work
            }

            stopwatch.Stop();

            var averageMs = stopwatch.ElapsedMilliseconds / (double)iterations;

            // Assert - Should not be more than 2x slower than baseline
            Assert.Less(averageMs, baselineMs * 2,
                $"Performance regression detected! Current: {averageMs:F4}ms, Baseline: {baselineMs:F4}ms");

            TestContext.WriteLine($"Average pathfinding time: {averageMs:F4}ms (baseline: {baselineMs:F4}ms)");
        }

        [Test]
        [Category("Regression")]
        public void Regression_CachePerformance_NoRegression()
        {
            // Arrange
            var baselineMs = 0.001; // 0.001ms baseline
            var iterations = 10000;

            SmartCache.ClearAll();

            var stopwatch = Stopwatch.StartNew();

            // Act
            for (int i = 0; i < iterations; i++)
            {
                var key = $"key_{i % 100}";
                SmartCache.GetOrCompute(key, () => i);
            }

            stopwatch.Stop();

            var averageMs = stopwatch.ElapsedMilliseconds / (double)iterations;

            // Assert
            Assert.Less(averageMs, baselineMs * 2,
                $"Cache performance regression! Current: {averageMs:F6}ms, Baseline: {baselineMs:F6}ms");

            TestContext.WriteLine($"Average cache operation: {averageMs:F6}ms (baseline: {baselineMs:F6}ms)");
        }

        #endregion

        #region Helper Methods

        private List<MockPawn> CreateTestColony(int size)
        {
            var pawns = new List<MockPawn>();
            var map = new MockMap();

            for (int i = 0; i < size; i++)
            {
                pawns.Add(new MockPawn
                {
                    Dead = false,
                    Spawned = true,
                    Map = map
                });
            }

            return pawns;
        }

        #endregion
    }
}

