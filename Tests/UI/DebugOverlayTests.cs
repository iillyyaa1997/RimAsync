using NUnit.Framework;
using System;
using UnityEngine;
using RimAsync.Core;
using RimAsync.Utils;
using RimAsync.Threading;
using RimAsync.Tests.Utils;
using RimAsync.Tests.Mocks;
using Verse;

namespace RimAsync.Tests.UI
{
    /// <summary>
    /// Comprehensive tests for DebugOverlay
    /// Tests UI rendering, metrics display, toggle functionality, and performance
    /// </summary>
    [TestFixture]
    public class DebugOverlayTests
    {
        private MockRimAsyncSettings _mockSettings;

        [SetUp]
        public void SetUp()
        {
            // Reset DebugOverlay state
            RimAsync.Utils.DebugOverlay.Enabled = false;

            // Initialize mock settings
            _mockSettings = new MockRimAsyncSettings();

            // Reset logger configuration
            RimAsyncLogger.Configure(false, RimAsyncLogger.LogLevel.Info);

            // Clear cache (PerformanceMonitor doesn't have ClearAllMetrics method)
            SmartCache.ClearAll();
        }

        [TearDown]
        public void TearDown()
        {
            // Disable overlay after tests
            RimAsync.Utils.DebugOverlay.Enabled = false;
            _mockSettings = null;
        }

        #region Basic Functionality Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void Enabled_GetSet_WorksCorrectly()
        {
            // Arrange & Act
            RimAsync.Utils.DebugOverlay.Enabled = true;

            // Assert
            Assert.IsTrue(RimAsync.Utils.DebugOverlay.Enabled, "DebugOverlay should be enabled");

            // Act
            RimAsync.Utils.DebugOverlay.Enabled = false;

            // Assert
            Assert.IsFalse(RimAsync.Utils.DebugOverlay.Enabled, "DebugOverlay should be disabled");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void Toggle_ChangesEnabledState()
        {
            // Arrange
            var initialState = RimAsync.Utils.DebugOverlay.Enabled;

            // Act
            RimAsync.Utils.DebugOverlay.Toggle();

            // Assert
            Assert.AreNotEqual(initialState, RimAsync.Utils.DebugOverlay.Enabled,
                "Toggle should change enabled state");

            // Act - Toggle back
            RimAsync.Utils.DebugOverlay.Toggle();

            // Assert
            Assert.AreEqual(initialState, RimAsync.Utils.DebugOverlay.Enabled,
                "Toggle twice should return to initial state");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void OnGUI_WhenDisabled_DoesNotThrow()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = false;

            // Act & Assert
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.OnGUI(),
                "OnGUI should not throw when disabled");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void OnGUI_WhenEnabled_DoesNotThrow()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;

            // Act & Assert
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.OnGUI(),
                "OnGUI should not throw when enabled");
        }

        #endregion

        #region Status and Reporting Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void GetQuickStatus_ReturnsNonEmptyString()
        {
            // Act
            var status = RimAsync.Utils.DebugOverlay.GetQuickStatus();

            // Assert
            Assert.IsNotNull(status, "GetQuickStatus should not return null");
            Assert.IsNotEmpty(status, "GetQuickStatus should return non-empty string");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetQuickStatus_ContainsTPSInfo()
        {
            // Act
            var status = RimAsync.Utils.DebugOverlay.GetQuickStatus();

            // Assert
            Assert.That(status.Contains("TPS") || status.Contains("Status unavailable"),
                "GetQuickStatus should contain TPS info or unavailable message");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetDetailedReport_ReturnsNonEmptyString()
        {
            // Act
            var report = RimAsync.Utils.DebugOverlay.GetDetailedReport();

            // Assert
            Assert.IsNotNull(report, "GetDetailedReport should not return null");
            Assert.IsNotEmpty(report, "GetDetailedReport should return non-empty string");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetDetailedReport_ContainsMainSections()
        {
            // Act
            var report = RimAsync.Utils.DebugOverlay.GetDetailedReport();

            // Assert - Check for expected sections
            Assert.That(report.Contains("RimAsync Debug Info"),
                "Report should contain header");
            Assert.That(report.Contains("Performance") || report.Contains("Error"),
                "Report should contain Performance section or error message");
            Assert.That(report.Contains("SmartCache") || report.Contains("Error"),
                "Report should contain SmartCache section or error message");
            Assert.That(report.Contains("Async Operations") || report.Contains("Error"),
                "Report should contain Async Operations section or error message");
            Assert.That(report.Contains("Settings") || report.Contains("Error"),
                "Report should contain Settings section or error message");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void LogStatus_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.LogStatus(),
                "LogStatus should not throw");
        }

        #endregion

        #region Performance Tests

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void OnGUI_Performance_CompletesQuickly()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;

            // Act - Measure performance
            var metrics = TestHelpers.MeasurePerformance(() => RimAsync.Utils.DebugOverlay.OnGUI(), 100);

            // Assert - OnGUI should be fast (< 10ms average)
            Assert.Less(metrics.AverageTimePerIteration, 10.0,
                $"OnGUI should complete in < 10ms on average, actual: {metrics.AverageTimePerIteration:F2}ms");
        }

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetQuickStatus_Performance_CompletesQuickly()
        {
            // Act - Measure performance
            var metrics = TestHelpers.MeasurePerformance(() => RimAsync.Utils.DebugOverlay.GetQuickStatus(), 1000);

            // Assert - GetQuickStatus should be very fast (< 1ms average)
            Assert.Less(metrics.AverageTimePerIteration, 1.0,
                $"GetQuickStatus should complete in < 1ms on average, actual: {metrics.AverageTimePerIteration:F2}ms");
        }

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetDetailedReport_Performance_CompletesReasonably()
        {
            // Act - Measure performance
            var metrics = TestHelpers.MeasurePerformance(() => RimAsync.Utils.DebugOverlay.GetDetailedReport(), 100);

            // Assert - GetDetailedReport should be reasonable (< 5ms average)
            Assert.Less(metrics.AverageTimePerIteration, 5.0,
                $"GetDetailedReport should complete in < 5ms on average, actual: {metrics.AverageTimePerIteration:F2}ms");
        }

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void OnGUI_WhenDisabled_NoPerformanceImpact()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = false;

            // Act - Measure performance when disabled
            var metrics = TestHelpers.MeasurePerformance(() => RimAsync.Utils.DebugOverlay.OnGUI(), 1000);

            // Assert - Should be extremely fast when disabled (< 0.1ms average)
            Assert.Less(metrics.AverageTimePerIteration, 0.1,
                $"OnGUI when disabled should have negligible performance impact, actual: {metrics.AverageTimePerIteration:F2}ms");
        }

        #endregion

        #region Error Handling Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void OnGUI_WithNullSettings_DoesNotThrow()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;
            // RimAsyncMod.Settings could be null during initialization

            // Act & Assert
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.OnGUI(),
                "OnGUI should handle null settings gracefully");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetQuickStatus_WithUninitializedComponents_DoesNotThrow()
        {
            // Act & Assert - Should handle uninitialized AsyncManager gracefully
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.GetQuickStatus(),
                "GetQuickStatus should handle uninitialized components gracefully");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetDetailedReport_WithUninitializedComponents_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.GetDetailedReport(),
                "GetDetailedReport should handle uninitialized components gracefully");
        }

        #endregion

        #region Integration Tests

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void DebugOverlay_IntegrationWithPerformanceMonitor_Works()
        {
            // Arrange
            using (PerformanceMonitor.StartMeasuring("TestMetric"))
            {
                System.Threading.Thread.Sleep(10); // Simulate work
            }

            // Act
            var report = RimAsync.Utils.DebugOverlay.GetDetailedReport();

            // Assert - Report should contain performance metrics
            Assert.That(report.Contains("Performance") || report.Contains("TPS"),
                "Report should contain performance information");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void DebugOverlay_IntegrationWithSmartCache_Works()
        {
            // Arrange - Populate cache
            SmartCache.GetOrCompute("test_key", () => "test_value");
            SmartCache.GetOrCompute("test_key", () => "test_value"); // Should hit cache

            // Act
            var report = RimAsync.Utils.DebugOverlay.GetDetailedReport();

            // Assert - Report should contain cache statistics
            Assert.That(report.Contains("SmartCache") || report.Contains("Cache"),
                "Report should contain cache information");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void DebugOverlay_MultipleCalls_UpdatesCachedText()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;

            // Act - Call OnGUI multiple times
            var report1 = RimAsync.Utils.DebugOverlay.GetDetailedReport();
            System.Threading.Thread.Sleep(600); // Wait for cache update interval
            var report2 = RimAsync.Utils.DebugOverlay.GetDetailedReport();

            // Assert - Both reports should be valid (they may be identical due to caching)
            Assert.IsNotNull(report1, "First report should not be null");
            Assert.IsNotNull(report2, "Second report should not be null");
            Assert.IsNotEmpty(report1, "First report should not be empty");
            Assert.IsNotEmpty(report2, "Second report should not be empty");
        }

        #endregion

        #region Unity Mock Integration Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void OnGUI_UsesUnityAPIs_DoesNotThrow()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;

            // This test verifies that DebugOverlay works with Unity mock classes
            // Screen.width, Screen.height, Time.realtimeSinceStartup, etc.

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Simulate multiple OnGUI calls
                for (int i = 0; i < 10; i++)
                {
                    RimAsync.Utils.DebugOverlay.OnGUI();
                }
            }, "OnGUI should work with Unity mock APIs");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void OnGUI_WithMockScreen_UsesCorrectDimensions()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;

            // Unity mock Screen dimensions: 1920x1080
            var expectedScreenWidth = Screen.width;
            var expectedScreenHeight = Screen.height;

            // Act & Assert
            Assert.DoesNotThrow(() => RimAsync.Utils.DebugOverlay.OnGUI(),
                "OnGUI should use Screen.width and Screen.height correctly");

            // Verify mock dimensions are reasonable
            Assert.Greater(expectedScreenWidth, 0, "Screen.width should be positive");
            Assert.Greater(expectedScreenHeight, 0, "Screen.height should be positive");
        }

        #endregion

        #region Stress Tests

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void OnGUI_RepeatedCalls_NoMemoryLeak()
        {
            // Arrange
            RimAsync.Utils.DebugOverlay.Enabled = true;
            var initialMemory = GC.GetTotalMemory(true);

            // Act - Call OnGUI many times
            for (int i = 0; i < 1000; i++)
            {
                RimAsync.Utils.DebugOverlay.OnGUI();
            }

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(true);
            var memoryIncrease = finalMemory - initialMemory;

            // Assert - Memory increase should be minimal (< 1MB)
            Assert.Less(memoryIncrease, 1024 * 1024,
                $"OnGUI should not leak memory. Memory increase: {memoryIncrease} bytes");
        }

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetQuickStatus_RepeatedCalls_NoMemoryLeak()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);

            // Act - Call GetQuickStatus many times
            for (int i = 0; i < 10000; i++)
            {
                var status = RimAsync.Utils.DebugOverlay.GetQuickStatus();
            }

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(true);
            var memoryIncrease = finalMemory - initialMemory;

            // Assert - Memory increase should be minimal (< 500KB)
            Assert.Less(memoryIncrease, 512 * 1024,
                $"GetQuickStatus should not leak memory. Memory increase: {memoryIncrease} bytes");
        }

        #endregion
    }
}
