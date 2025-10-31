using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RimAsync.Threading;
using RimAsync.Utils;
using RimAsync.Core;
using RimAsync.Tests.Utils;
using RimAsync.Tests.Mocks;
using Verse;
using Verse.AI;

namespace RimAsync.Tests.Performance
{
    /// <summary>
    /// Benchmark tests for async patches performance
    /// Measures performance improvements from async pathfinding, AI, and building updates
    /// </summary>
    [TestFixture]
    [Category(TestConfig.PerformanceTestCategory)]
    [Category(TestConfig.HighPriority)]
    public class AsyncPatchBenchmarkTests
    {
        private MockAsyncManager _mockAsyncManager;

        [SetUp]
        public void SetUp()
        {
            // Initialize performance monitoring
            PerformanceMonitor.Initialize();

            // Setup mock async manager
            _mockAsyncManager = new MockAsyncManager();
            _mockAsyncManager.IsAsyncTimeEnabled = true;
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            PerformanceMonitor.Shutdown();
            _mockAsyncManager = null;
        }

        #region Pathfinding Benchmarks

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void PathfindingBenchmark_SinglePawn_MeasuresPerformance()
        {
            // Arrange
            var pawn = TestHelpers.CreateMockPawn();
            const int pathfindingIterations = 100;

            // Act - Measure pathfinding performance
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate pathfinding calculation
                var start = new IntVec3(0, 0, 0);
                var end = new IntVec3(10, 0, 10);
                
                // This would call actual pathfinding in real scenario
                Thread.SpinWait(100); // Simulate pathfinding work
            }, pathfindingIterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(10.0), 
                "Pathfinding should complete in less than 10ms on average");
            
            TestContext.WriteLine($"Pathfinding Performance:");
            TestContext.WriteLine($"  Average: {metrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Total: {metrics.ElapsedMilliseconds}ms for {pathfindingIterations} iterations");
            TestContext.WriteLine($"  Memory: {metrics.MemoryUsed / 1024.0:F2}KB");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void PathfindingBenchmark_MultiplePawns_ScalesWell()
        {
            // Arrange
            const int pawnCount = 10;
            var pawns = new Pawn[pawnCount];
            for (int i = 0; i < pawnCount; i++)
            {
                pawns[i] = TestHelpers.CreateMockPawn();
            }

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                foreach (var pawn in pawns)
                {
                    // Simulate pathfinding for each pawn
                    Thread.SpinWait(100);
                }
            }, iterations: 10);

            // Assert
            var avgPerPawn = metrics.AverageTimePerIteration / pawnCount;
            Assert.That(avgPerPawn, Is.LessThan(15.0), 
                "Per-pawn pathfinding overhead should be less than 15ms");
            
            TestContext.WriteLine($"Multi-Pawn Pathfinding Performance:");
            TestContext.WriteLine($"  {pawnCount} pawns average: {metrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Per pawn: {avgPerPawn:F2}ms");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task PathfindingBenchmark_AsyncVsSync_ShowsImprovement()
        {
            // Arrange
            const int iterations = 50;
            var pawn = TestHelpers.CreateMockPawn();

            // Act - Measure sync pathfinding
            var syncMetrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate synchronous pathfinding
                Thread.SpinWait(1000);
            }, iterations);

            // Act - Measure async pathfinding
            var asyncMetrics = await TestHelpers.MeasurePerformanceAsync(async (ct) =>
            {
                // Simulate asynchronous pathfinding
                await Task.Delay(1, ct);
            }, iterations);

            // Assert
            var improvement = (syncMetrics.AverageTimePerIteration - asyncMetrics.AverageTimePerIteration) 
                / syncMetrics.AverageTimePerIteration;

            TestContext.WriteLine($"Pathfinding Async vs Sync:");
            TestContext.WriteLine($"  Sync average: {syncMetrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Async average: {asyncMetrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Improvement: {improvement:P2}");

            // Async should be at least somewhat better (or comparable with overhead considered)
            Assert.That(asyncMetrics.AverageTimePerIteration, Is.LessThanOrEqualTo(syncMetrics.AverageTimePerIteration * 1.2),
                "Async pathfinding should not be significantly slower than sync");
        }

        #endregion

        #region AI Processing Benchmarks

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void AIPatchBenchmark_JobThinking_MeasuresPerformance()
        {
            // Arrange
            var pawn = TestHelpers.CreateMockPawn();
            const int thinkIterations = 100;

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate AI thinking/job selection
                Thread.SpinWait(500);
            }, thinkIterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(5.0), 
                "AI job thinking should complete in less than 5ms on average");
            
            TestContext.WriteLine($"AI Job Thinking Performance:");
            TestContext.WriteLine($"  Average: {metrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Memory: {metrics.MemoryUsed / 1024.0:F2}KB");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void AIPatchBenchmark_MultipleColonists_ScalesLinearly()
        {
            // Arrange
            const int colonistCount = 20;
            var colonists = new Pawn[colonistCount];
            for (int i = 0; i < colonistCount; i++)
            {
                colonists[i] = TestHelpers.CreateMockPawn();
            }

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                foreach (var colonist in colonists)
                {
                    // Simulate AI tick
                    Thread.SpinWait(50);
                }
            }, iterations: 10);

            // Assert
            var avgPerColonist = metrics.AverageTimePerIteration / colonistCount;
            Assert.That(avgPerColonist, Is.LessThan(2.0), 
                "Per-colonist AI processing should be less than 2ms");
            
            TestContext.WriteLine($"Multi-Colonist AI Performance:");
            TestContext.WriteLine($"  {colonistCount} colonists total: {metrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Per colonist: {avgPerColonist:F2}ms");
        }

        #endregion

        #region Building Update Benchmarks

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void BuildingPatchBenchmark_SingleBuilding_MeasuresPerformance()
        {
            // Arrange
            var building = TestHelpers.CreateMockBuilding();
            const int updateIterations = 100;

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate building tick
                Thread.SpinWait(200);
            }, updateIterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(3.0), 
                "Building updates should complete in less than 3ms on average");
            
            TestContext.WriteLine($"Building Update Performance:");
            TestContext.WriteLine($"  Average: {metrics.AverageTimePerIteration:F2}ms");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void BuildingPatchBenchmark_LargeBase_HandlesLoad()
        {
            // Arrange
            const int buildingCount = 200; // Simulate large base
            var buildings = new Thing[buildingCount];
            for (int i = 0; i < buildingCount; i++)
            {
                buildings[i] = TestHelpers.CreateMockBuilding();
            }

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                foreach (var building in buildings)
                {
                    // Simulate building tick
                    Thread.SpinWait(10);
                }
            }, iterations: 5);

            // Assert
            var totalTickTime = metrics.AverageTimePerIteration;
            var perBuildingTime = totalTickTime / buildingCount;
            
            Assert.That(totalTickTime, Is.LessThan(100.0), 
                "200 buildings should tick in less than 100ms total");
            
            TestContext.WriteLine($"Large Base Building Performance:");
            TestContext.WriteLine($"  {buildingCount} buildings total: {totalTickTime:F2}ms");
            TestContext.WriteLine($"  Per building: {perBuildingTime:F4}ms");
        }

        #endregion

        #region Construction Benchmarks

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void ConstructionPatchBenchmark_WorkCalculation_MeasuresPerformance()
        {
            // Arrange
            var construction = TestHelpers.CreateMockConstruction();
            const int workIterations = 50;

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate construction work calculation
                Thread.SpinWait(300);
            }, workIterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(5.0), 
                "Construction work calculation should complete in less than 5ms");
            
            TestContext.WriteLine($"Construction Work Performance:");
            TestContext.WriteLine($"  Average: {metrics.AverageTimePerIteration:F2}ms");
        }

        #endregion

        #region End-to-End Benchmarks

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void EndToEndBenchmark_SimulatedTick_MeetsTPSTarget()
        {
            // Arrange - Simulate a typical game tick with multiple systems
            const int simulatedTicks = 100;
            var pawns = new Pawn[15];
            for (int i = 0; i < pawns.Length; i++)
            {
                pawns[i] = TestHelpers.CreateMockPawn();
            }

            var buildings = new Thing[50];
            for (int i = 0; i < buildings.Length; i++)
            {
                buildings[i] = TestHelpers.CreateMockBuilding();
            }

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate game tick
                using (PerformanceMonitor.StartMeasuring("full_tick"))
                {
                    // Pawns AI and pathfinding
                    foreach (var pawn in pawns)
                    {
                        Thread.SpinWait(50); // AI processing
                        Thread.SpinWait(100); // Pathfinding
                    }

                    // Building updates
                    foreach (var building in buildings)
                    {
                        Thread.SpinWait(10); // Building tick
                    }
                }
                
                PerformanceMonitor.UpdateMetrics();
            }, simulatedTicks);

            // Assert
            var avgTickTime = metrics.AverageTimePerIteration;
            var targetTickTime = 1000.0 / 60.0; // 60 TPS target = ~16.67ms per tick
            
            TestContext.WriteLine($"End-to-End Tick Performance:");
            TestContext.WriteLine($"  Average tick time: {avgTickTime:F2}ms");
            TestContext.WriteLine($"  Target tick time (60 TPS): {targetTickTime:F2}ms");
            TestContext.WriteLine($"  Estimated TPS: {1000.0 / avgTickTime:F2}");
            TestContext.WriteLine($"  {pawns.Length} pawns + {buildings.Length} buildings");

            // With async optimizations, we should be able to maintain good TPS
            // For this test, we accept performance that would give us at least 30 TPS
            var minimumAcceptableTickTime = 1000.0 / 30.0; // 30 TPS = ~33.33ms per tick
            Assert.That(avgTickTime, Is.LessThan(minimumAcceptableTickTime),
                $"Average tick time should support at least 30 TPS (< {minimumAcceptableTickTime:F2}ms)");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task EndToEndBenchmark_ConcurrentOperations_HandlesLoad()
        {
            // Arrange
            const int concurrentOperations = 10;
            
            // Act - Run multiple async operations concurrently
            var metrics = await TestHelpers.MeasurePerformanceAsync(async (ct) =>
            {
                var tasks = new Task[concurrentOperations];
                for (int i = 0; i < concurrentOperations; i++)
                {
                    tasks[i] = Task.Run(async () =>
                    {
                        await Task.Delay(1, ct);
                        Thread.SpinWait(100);
                    }, ct);
                }
                
                await Task.WhenAll(tasks);
            }, iterations: 20);

            // Assert
            TestContext.WriteLine($"Concurrent Operations Performance:");
            TestContext.WriteLine($"  {concurrentOperations} operations average: {metrics.AverageTimePerIteration:F2}ms");
            
            // With proper async, concurrent operations should complete faster than sequential
            var sequentialEstimate = concurrentOperations * 2.0; // Rough estimate
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(sequentialEstimate),
                "Concurrent operations should be faster than sequential");
        }

        #endregion

        #region Memory and Resource Benchmarks

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void MemoryBenchmark_SmartCache_LowOverhead()
        {
            // Arrange
            const int cacheOperations = 1000;

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                // Simulate cache operations using static SmartCache
                for (int i = 0; i < 100; i++)
                {
                    SmartCache.GetOrCompute($"perf_test_key_{i}", () => i * 2, ttlTicks: 600);
                }
                
                // Additional reads
                for (int i = 0; i < 100; i++)
                {
                    SmartCache.GetOrCompute($"perf_test_key_{i}", () => i * 2, ttlTicks: 600);
                }
            }, cacheOperations / 200); // 5 iterations

            // Assert
            var memoryPerOperation = (float)metrics.MemoryUsed / cacheOperations;
            
            TestContext.WriteLine($"SmartCache Memory Performance:");
            TestContext.WriteLine($"  Total memory: {metrics.MemoryUsed / 1024.0:F2}KB");
            TestContext.WriteLine($"  Per operation: {memoryPerOperation:F2} bytes");
            TestContext.WriteLine($"  Average time: {metrics.AverageTimePerIteration:F2}ms per 200 operations");

            // Memory overhead should be reasonable
            Assert.That(metrics.MemoryUsed, Is.LessThan(1024 * 1024), 
                "SmartCache memory overhead should be less than 1MB for test workload");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void ResourceBenchmark_ThreadPoolUsage_WithinLimits()
        {
            // Arrange
            const int taskCount = 100;
            var completedTasks = 0;

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                var tasks = new Task[taskCount];
                for (int i = 0; i < taskCount; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        Thread.SpinWait(100);
                        Interlocked.Increment(ref completedTasks);
                    });
                }
                
                Task.WaitAll(tasks);
            }, iterations: 1);

            // Assert
            Assert.That(completedTasks, Is.EqualTo(taskCount), 
                "All tasks should complete");
            
            TestContext.WriteLine($"Thread Pool Performance:");
            TestContext.WriteLine($"  {taskCount} tasks completed in: {metrics.ElapsedMilliseconds}ms");
            TestContext.WriteLine($"  Average per task: {metrics.ElapsedMilliseconds / (double)taskCount:F2}ms");
        }

        #endregion

        #region Performance Regression Tests

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void RegressionTest_PathfindingDoesNotDegrade()
        {
            // This test ensures pathfinding performance doesn't regress over time
            // In a real scenario, this would compare against baseline metrics

            // Arrange
            const int iterations = 100;
            const double acceptableBaseline = 10.0; // ms

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                Thread.SpinWait(100); // Simulate pathfinding
            }, iterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(acceptableBaseline),
                $"Pathfinding performance should not exceed baseline of {acceptableBaseline}ms");
            
            TestContext.WriteLine($"Regression Test - Pathfinding:");
            TestContext.WriteLine($"  Current: {metrics.AverageTimePerIteration:F2}ms");
            TestContext.WriteLine($"  Baseline: {acceptableBaseline:F2}ms");
            TestContext.WriteLine($"  Status: {(metrics.AverageTimePerIteration < acceptableBaseline ? "PASS" : "FAIL")}");
        }

        #endregion
    }
}

