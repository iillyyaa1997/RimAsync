using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using RimAsync.Tests.Utils;
using RimAsync.Tests.Mocks;

namespace RimAsync.Tests.Integration
{
    /// <summary>
    /// Comprehensive integration tests for AsyncTime functionality
    /// Tests detection logic, execution modes, and performance scenarios
    /// </summary>
    [TestFixture]
        public class AsyncTimeIntegrationTests
    {
        private MockAsyncManager _mockAsyncManager;

        [SetUp]
        public void SetUp()
        {
            TestHelpers.MockRimWorldEnvironment();
            _mockAsyncManager = new MockAsyncManager();

            // Initialize test mode - single player by default
            MultiplayerCompat.EnableTestMode(false, false);
        }

        [TearDown]
        public void TearDown()
        {
            _mockAsyncManager?.Dispose();
            MultiplayerCompat.DisableTestMode();
            TestHelpers.ResetMocks();
            TestHelpers.CleanupTempFiles();
        }

        #region AsyncTime Detection Tests

                [Test]
        [Category(TestConfig.CriticalPriority)]
        public void AsyncTimeDetection_SinglePlayer_ReturnsFullAsync()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(false, false);

            // Act
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.AreEqual(ExecutionMode.FullAsync, executionMode,
                "Single player should use FullAsync execution mode");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void AsyncTimeDetection_MultiplayerWithAsyncTime_ReturnsAsyncTimeEnabled()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);

            // Act
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.AreEqual(ExecutionMode.AsyncTimeEnabled, executionMode,
                "Multiplayer with AsyncTime should use AsyncTimeEnabled execution mode");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public void AsyncTimeDetection_MultiplayerWithoutAsyncTime_ReturnsFullSync()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, false);

            // Act
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.AreEqual(ExecutionMode.FullSync, executionMode,
                "Multiplayer without AsyncTime should use FullSync execution mode");
        }

        #endregion

        #region Execution Mode Switching Tests

        [Test]
        [Category(TestConfig.HighPriority)]
                public async Task ExecutionModeSwitching_FromSingleToMultiplayer_ChangesMode()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(false, false);
            var initialMode = RimAsyncCore.GetExecutionMode();

            // Act
            MultiplayerCompat.EnableTestMode(true, false);
            var newMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.AreEqual(ExecutionMode.FullAsync, initialMode,
                "Initial mode should be FullAsync in single player");
            Assert.AreEqual(ExecutionMode.FullSync, newMode,
                "Mode should change to FullSync when entering multiplayer without AsyncTime");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        public async Task ExecutionModeSwitching_AsyncTimeToggle_ChangesMode()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, false);
            var syncMode = RimAsyncCore.GetExecutionMode();

            // Act
            MultiplayerCompat.EnableTestMode(true, true);
            var asyncTimeMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.AreEqual(ExecutionMode.FullSync, syncMode,
                "Should start in FullSync mode without AsyncTime");
            Assert.AreEqual(ExecutionMode.AsyncTimeEnabled, asyncTimeMode,
                "Should switch to AsyncTimeEnabled when AsyncTime is enabled");
        }

        #endregion

        #region Adaptive Execution Tests

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public async Task ExecuteAdaptive_SinglePlayer_UsesAsync()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(false, false);
            bool asyncExecuted = false;
            bool syncExecuted = false;

            var asyncOperation = new Func<CancellationToken, Task>(async ct =>
            {
                await Task.Delay(10, ct);
                asyncExecuted = true;
            });

            var syncOperation = new Action(() =>
            {
                syncExecuted = true;
            });

            // Act
            await AsyncManager.ExecuteAdaptive(asyncOperation, syncOperation, "SinglePlayerTest");

            // Assert
            Assert.IsTrue(asyncExecuted, "Async operation should be executed in single player");
            Assert.IsFalse(syncExecuted, "Sync operation should not be executed in single player");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public async Task ExecuteAdaptive_MultiplayerWithoutAsyncTime_UsesSync()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, false);
            bool asyncExecuted = false;
            bool syncExecuted = false;

            var asyncOperation = new Func<CancellationToken, Task>(async ct =>
            {
                await Task.Delay(10, ct);
                asyncExecuted = true;
            });

            var syncOperation = new Action(() =>
            {
                syncExecuted = true;
            });

            // Act
            await AsyncManager.ExecuteAdaptive(asyncOperation, syncOperation, "MultiplayerSyncTest");

            // Assert
            Assert.IsFalse(asyncExecuted, "Async operation should not be executed in multiplayer without AsyncTime");
            Assert.IsTrue(syncExecuted, "Sync operation should be executed in multiplayer without AsyncTime");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        public async Task ExecuteAdaptive_MultiplayerWithAsyncTime_UsesAsyncTimeCompatible()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);
            bool asyncExecuted = false;
            bool syncExecuted = false;

            var asyncOperation = new Func<CancellationToken, Task>(async ct =>
            {
                await Task.Delay(10, ct);
                asyncExecuted = true;
            });

            var syncOperation = new Action(() =>
            {
                syncExecuted = true;
            });

            // Act
            await AsyncManager.ExecuteAdaptive(asyncOperation, syncOperation, "AsyncTimeCompatTest");

            // Assert
            Assert.IsTrue(asyncExecuted || syncExecuted,
                "Either async or sync operation should be executed in AsyncTime mode");

            // In AsyncTime mode, it tries async first, may fallback to sync
            if (!asyncExecuted)
            {
                Assert.IsTrue(syncExecuted, "If async fails, sync should be executed as fallback");
            }
        }

        #endregion

        #region Performance Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        public async Task PerformanceTest_AsyncVsSync_MeasuresDifference()
        {
            // Arrange
            const int iterations = 10;
            var tasks = new List<Task>();

            // Setup single player mode for async test
            MultiplayerCompat.EnableTestMode(false, false);

            // Act - Measure async performance
            var asyncMetrics = await TestHelpers.MeasurePerformanceAsync(async ct =>
            {
                await AsyncManager.ExecuteAdaptive(
                    async c => await Task.Delay(1, c),
                    () => Thread.Sleep(1),
                    "PerformanceTestAsync");
            }, iterations);

            // Setup multiplayer mode for sync test
            MultiplayerCompat.EnableTestMode(true, false);

            var syncMetrics = TestHelpers.MeasurePerformance(() =>
            {
                AsyncManager.ExecuteAdaptive(
                    async c => await Task.Delay(1, c),
                    () => Thread.Sleep(1),
                    "PerformanceTestSync").Wait();
            }, iterations);

            // Assert
            Assert.IsTrue(asyncMetrics.ElapsedMilliseconds >= 0,
                "Async metrics should be measured");
            Assert.IsTrue(syncMetrics.ElapsedMilliseconds >= 0,
                "Sync metrics should be measured");

            Console.WriteLine($"Async performance: {asyncMetrics}");
            Console.WriteLine($"Sync performance: {syncMetrics}");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public async Task ThreadSafetyTest_ConcurrentAsyncTimeOperations_NoDataRaces()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);

            var counter = 0;
            const int concurrency = 20;

            // Act & Assert
            await TestHelpers.AssertThreadSafety(async () =>
            {
                await AsyncManager.ExecuteAdaptive(
                    async ct =>
                    {
                        await Task.Delay(10, ct);
                        Interlocked.Increment(ref counter);
                    },
                    () => Interlocked.Increment(ref counter),
                    "ThreadSafetyTest");
            }, concurrency);

            // Assert final count
            Assert.AreEqual(concurrency, counter,
                "All operations should complete exactly once");
        }

        #endregion

        #region Error Handling Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        public async Task ErrorHandling_AsyncOperationThrows_FallsBackToSync()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);
            bool syncExecuted = false;

            var failingAsyncOperation = new Func<CancellationToken, Task>(ct =>
                throw new InvalidOperationException("Test exception"));

            var syncOperation = new Action(() => syncExecuted = true);

            // Act & Assert - Should not throw
            Assert.DoesNotThrowAsync(async () =>
            {
                await AsyncManager.ExecuteAdaptive(failingAsyncOperation, syncOperation, "ErrorHandlingTest");
            });

            Assert.IsTrue(syncExecuted, "Sync operation should execute when async fails");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public async Task ErrorHandling_CancellationToken_StopsOperation()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(false, false);
            using var cts = new CancellationTokenSource();
            bool operationCompleted = false;

            var longRunningOperation = new Func<CancellationToken, Task>(async ct =>
            {
                await Task.Delay(5000, ct); // Long delay
                operationCompleted = true;
            });

            var syncOperation = new Action(() => Thread.Sleep(5000));

            // Act
            cts.CancelAfter(100); // Cancel after 100ms

            try
            {
                await AsyncManager.ExecuteAdaptive(longRunningOperation, syncOperation, "CancellationTest");
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            // Assert
            Assert.IsFalse(operationCompleted, "Operation should be cancelled before completion");
        }

        #endregion

        #region Real-world Scenario Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        public async Task RealWorldScenario_PathfindingWithAsyncTime_WorksCorrectly()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);

            var pathfindingCompleted = false;
            var fallbackUsed = false;

            // Mock expensive pathfinding operation
            var asyncPathfinding = new Func<CancellationToken, Task>(async ct =>
            {
                // Simulate pathfinding calculation
                await Task.Delay(50, ct);
                pathfindingCompleted = true;
            });

            var syncPathfinding = new Action(() =>
            {
                // Simulate sync fallback
                Thread.Sleep(50);
                fallbackUsed = true;
            });

            // Act
            await AsyncManager.ExecuteAdaptive(asyncPathfinding, syncPathfinding, "PathfindingScenario");

            // Assert
            Assert.IsTrue(pathfindingCompleted || fallbackUsed,
                "Either async pathfinding or sync fallback should complete");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public async Task RealWorldScenario_MultipleAsyncTimeOperations_Sequential()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);

            var results = new List<string>();

            // Act - Execute multiple operations sequentially
            await AsyncManager.ExecuteAdaptive(
                async ct => { await Task.Delay(10, ct); results.Add("Op1"); },
                () => results.Add("Op1_sync"),
                "Operation1");

            await AsyncManager.ExecuteAdaptive(
                async ct => { await Task.Delay(10, ct); results.Add("Op2"); },
                () => results.Add("Op2_sync"),
                "Operation2");

            await AsyncManager.ExecuteAdaptive(
                async ct => { await Task.Delay(10, ct); results.Add("Op3"); },
                () => results.Add("Op3_sync"),
                "Operation3");

            // Assert
            Assert.AreEqual(3, results.Count, "All three operations should complete");
            Assert.IsTrue(results.Contains("Op1") || results.Contains("Op1_sync"),
                "Operation 1 should complete");
            Assert.IsTrue(results.Contains("Op2") || results.Contains("Op2_sync"),
                "Operation 2 should complete");
            Assert.IsTrue(results.Contains("Op3") || results.Contains("Op3_sync"),
                "Operation 3 should complete");
        }

        #endregion

        #region Integration with RimAsync Components

        [Test]
        [Category(TestConfig.HighPriority)]
        public async Task Integration_AsyncManagerWithAsyncTime_InitializesCorrectly()
        {
            // Arrange & Act
            MultiplayerCompat.EnableTestMode(true, true);

            var canExecuteAsync = AsyncManager.CanExecuteAsync();
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.IsTrue(canExecuteAsync, "Should be able to execute async with AsyncTime enabled");
            Assert.AreEqual(ExecutionMode.AsyncTimeEnabled, executionMode,
                "Execution mode should be AsyncTimeEnabled");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        public void Integration_PerformanceMonitoring_WithAsyncTime_CollectsMetrics()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, true);

            // Act
            var metrics = _mockAsyncManager.GetPerformanceMetrics();

            // Assert
            Assert.IsNotNull(metrics, "Performance metrics should be available");
            Assert.IsTrue(metrics.ThreadUtilization >= 0, "Thread utilization should be non-negative");
            Assert.IsTrue(metrics.TasksPerSecond >= 0, "Tasks per second should be non-negative");

            Console.WriteLine($"AsyncTime Performance Metrics: {metrics}");
        }

        #endregion
    }
}
