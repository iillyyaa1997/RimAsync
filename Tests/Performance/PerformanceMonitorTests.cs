using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RimAsync.Utils;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Performance
{
    /// <summary>
    /// Performance tests for PerformanceMonitor TPS tracking
    /// Tests TPS measurement accuracy and performance metrics recording
    /// </summary>
    [TestFixture]
    [Category(TestConfig.PerformanceTestCategory)]
    [Category(TestConfig.CriticalPriority)]
    public class PerformanceMonitorTests
    {
        [SetUp]
        public void SetUp()
        {
            // Reset PerformanceMonitor state before each test
            PerformanceMonitor.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up after tests
            PerformanceMonitor.Shutdown();
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void Initialize_WhenCalled_CompletesSuccessfully()
        {
            // Arrange & Act
            PerformanceMonitor.Initialize();

            // Assert
            Assert.That(PerformanceMonitor.CurrentTPS, Is.GreaterThan(0.0f),
                "TPS should be initialized to a positive value");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void CurrentTPS_InitialValue_ShouldBeReasonable()
        {
            // Arrange & Act
            var tps = PerformanceMonitor.CurrentTPS;

            // Assert
            Assert.That(tps, Is.GreaterThan(0.0f), "TPS should be positive");
            Assert.That(tps, Is.LessThanOrEqualTo(120.0f), "TPS should not exceed 120");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void AverageTPS_InitialValue_ShouldBeReasonable()
        {
            // Arrange & Act
            var avgTps = PerformanceMonitor.AverageTPS;

            // Assert
            Assert.That(avgTps, Is.GreaterThan(0.0f), "Average TPS should be positive");
            Assert.That(avgTps, Is.LessThanOrEqualTo(120.0f), "Average TPS should not exceed 120");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void IsPerformanceGood_WithHighTPS_ReturnsTrue()
        {
            // Arrange
            // Simulate high TPS by recording fast ticks
            for (int i = 0; i < 10; i++)
            {
                using (PerformanceMonitor.StartMeasuring("test_tick"))
                {
                    Thread.Sleep(1); // Very fast tick
                }
                PerformanceMonitor.UpdateMetrics(); // Simulates 60 TPS updates
            }

            // Act
            var isGood = PerformanceMonitor.IsPerformanceGood;

            // Assert
            Assert.That(isGood, Is.True, "Performance should be considered good with TPS > 50");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void UpdateMetrics_WithMultipleCalls_UpdatesTPSHistory()
        {
            // Arrange
            // Simulate multiple game ticks

            // Act
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(16); // Simulate ~60 FPS
                PerformanceMonitor.UpdateMetrics();
            }

            // Assert
            var avgTps = PerformanceMonitor.AverageTPS;
            Assert.That(avgTps, Is.GreaterThan(0.0f), "Average TPS should be positive");
            TestContext.WriteLine($"Average TPS: {avgTps:F2}");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void StartMeasuring_Dispose_RecordsMetric()
        {
            // Arrange
            const string operationName = "test_operation";

            // Act
            using (PerformanceMonitor.StartMeasuring(operationName))
            {
                Thread.Sleep(10); // Simulate work
            }

            // Assert
            var metric = PerformanceMonitor.GetMetric(operationName);
            Assert.That(metric, Is.Not.Null, "Metric should be recorded");
            Assert.That(metric.Average, Is.GreaterThan(0.0f), "Average time should be positive");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void RecordMetric_WithValue_StoresCorrectly()
        {
            // Arrange
            const string metricName = "test_metric";
            const float metricValue = 15.5f;

            // Act
            PerformanceMonitor.RecordMetric(metricName, metricValue);

            // Assert
            var metric = PerformanceMonitor.GetMetric(metricName);
            Assert.That(metric, Is.Not.Null, "Metric should exist");
            Assert.That(metric.Maximum, Is.GreaterThanOrEqualTo(metricValue), "Max should include recorded value");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void GetMetric_NonExistent_ReturnsNull()
        {
            // Arrange
            const string nonExistentMetric = "non_existent_metric_12345";

            // Act
            var metric = PerformanceMonitor.GetMetric(nonExistentMetric);

            // Assert
            Assert.That(metric, Is.Null, "Non-existent metric should return null");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void GetAllMetrics_AfterRecording_ReturnsMetrics()
        {
            // Arrange
            PerformanceMonitor.RecordMetric("metric1", 10.0f);
            PerformanceMonitor.RecordMetric("metric2", 20.0f);
            PerformanceMonitor.RecordMetric("metric3", 30.0f);

            // Act
            var allMetrics = PerformanceMonitor.GetAllMetrics();

            // Assert
            Assert.That(allMetrics, Is.Not.Null, "Metrics dictionary should not be null");
            Assert.That(allMetrics.Count, Is.GreaterThanOrEqualTo(3), "Should have at least 3 metrics");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void OnSettingsChanged_ClearsMetrics()
        {
            // Arrange
            PerformanceMonitor.RecordMetric("test_metric", 10.0f);
            Assert.That(PerformanceMonitor.GetMetric("test_metric"), Is.Not.Null, "Metric should exist before clear");

            // Act
            PerformanceMonitor.OnSettingsChanged();

            // Assert
            var metric = PerformanceMonitor.GetMetric("test_metric");
            Assert.That(metric, Is.Null, "Metrics should be cleared after settings change");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task PerformanceMonitor_ThreadSafety_HandlesMultipleConcurrentCalls()
        {
            // Arrange & Act
            await TestHelpers.AssertThreadSafety(async () =>
            {
                using (PerformanceMonitor.StartMeasuring("concurrent_test"))
                {
                    await Task.Delay(1);
                }
                PerformanceMonitor.UpdateMetrics();
            }, concurrency: 20);

            // Assert - If we get here without exceptions, thread safety is good
            Assert.Pass("Performance monitor handled concurrent access correctly");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void PerformanceMonitor_MeasureOperationOverhead_ShouldBeLow()
        {
            // Arrange
            const int iterations = 1000;
            const string operation = "overhead_test";

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                using (PerformanceMonitor.StartMeasuring(operation))
                {
                    // Minimal work
                }
            }, iterations);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(1.0),
                "Performance monitoring overhead should be less than 1ms per operation");

            TestContext.WriteLine($"Performance monitoring overhead: {metrics.AverageTimePerIteration:F4}ms per operation");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void UpdateMetrics_PerformanceTest_HandlesHighFrequency()
        {
            // Arrange
            const int tickCount = 300; // 5 seconds at 60 TPS

            // Act
            var metrics = TestHelpers.MeasurePerformance(() =>
            {
                PerformanceMonitor.UpdateMetrics();
            }, tickCount);

            // Assert
            Assert.That(metrics.AverageTimePerIteration, Is.LessThan(0.5),
                "TPS recording should be very fast (< 0.5ms per tick)");

            var avgTps = PerformanceMonitor.AverageTPS;
            Assert.That(avgTps, Is.GreaterThan(0.0f),
                "Average TPS should be positive");

            TestContext.WriteLine($"TPS recording performance: {metrics.AverageTimePerIteration:F4}ms per tick");
            TestContext.WriteLine($"Calculated average TPS: {avgTps:F2}");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void PerformanceMetric_Statistics_CalculatesCorrectly()
        {
            // Arrange
            const string metricName = "stats_test";
            float[] values = { 10.0f, 20.0f, 15.0f, 25.0f, 30.0f };

            // Act
            foreach (var value in values)
            {
                PerformanceMonitor.RecordMetric(metricName, value);
            }

            var metric = PerformanceMonitor.GetMetric(metricName);

            // Assert
            Assert.That(metric, Is.Not.Null, "Metric should exist");
            Assert.That(metric.Minimum, Is.EqualTo(10.0f), "Min should be 10.0");
            Assert.That(metric.Maximum, Is.EqualTo(30.0f), "Max should be 30.0");
            Assert.That(metric.Average, Is.GreaterThan(15.0f).And.LessThan(25.0f),
                "Average should be around 20.0");
        }

        [Test]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public void Shutdown_AfterInitialize_CleansUpCorrectly()
        {
            // Arrange
            PerformanceMonitor.Initialize();
            PerformanceMonitor.RecordMetric("test", 10.0f);

            // Act
            PerformanceMonitor.Shutdown();

            // Assert - Should not throw on re-initialization
            Assert.DoesNotThrow(() => PerformanceMonitor.Initialize(),
                "Should be able to re-initialize after shutdown");
        }
    }
}
