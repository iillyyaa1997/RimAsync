using NUnit.Framework;
using System;
using RimAsync.Utils;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Unit.Utils
{
    /// <summary>
    /// Comprehensive tests for ThreadLimitCalculator
    /// Tests automatic thread limit calculation based on CPU capabilities
    /// </summary>
    [TestFixture]
    public class ThreadLimitCalculatorTests
    {
        [SetUp]
        public void SetUp()
        {
            // Clear cache before each test
            ThreadLimitCalculator.ClearCache();
        }

        [TearDown]
        public void TearDown()
        {
            ThreadLimitCalculator.ClearCache();
        }

        #region Basic Functionality Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void ProcessorCount_ReturnsPositiveNumber()
        {
            // Act
            var processorCount = ThreadLimitCalculator.ProcessorCount;

            // Assert
            Assert.Greater(processorCount, 0, "Processor count should be positive");
            Assert.LessOrEqual(processorCount, 256, "Processor count should be reasonable (< 256)");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void CalculateOptimalThreadLimit_ReturnsValidRange()
        {
            // Act
            var optimalThreads = ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Assert
            Assert.GreaterOrEqual(optimalThreads, 1, "Optimal threads should be at least 1");
            Assert.LessOrEqual(optimalThreads, 8, "Optimal threads should not exceed 8");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void CalculateOptimalThreadLimit_IsCached()
        {
            // Act
            var first = ThreadLimitCalculator.CalculateOptimalThreadLimit();
            var second = ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Assert
            Assert.AreEqual(first, second, "Subsequent calls should return cached value");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void ClearCache_ResetsCalculation()
        {
            // Arrange
            var first = ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Act
            ThreadLimitCalculator.ClearCache();
            var second = ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Assert - Should be same value but freshly calculated
            Assert.AreEqual(first, second, "Values should be same after cache clear");
        }

        #endregion

        #region Thread Calculation Logic Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetRecommendedThreadLimit_WithAutoEnabled_ReturnsOptimal()
        {
            // Arrange
            int userPreference = 4;
            bool autoEnabled = true;

            // Act
            var recommended = ThreadLimitCalculator.GetRecommendedThreadLimit(userPreference, autoEnabled);
            var optimal = ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Assert
            Assert.AreEqual(optimal, recommended, "Auto mode should return optimal value");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetRecommendedThreadLimit_WithAutoDisabled_ReturnsUserPreference()
        {
            // Arrange
            int userPreference = 4;
            bool autoEnabled = false;

            // Act
            var recommended = ThreadLimitCalculator.GetRecommendedThreadLimit(userPreference, autoEnabled);

            // Assert
            Assert.AreEqual(userPreference, recommended, "Manual mode should return user preference");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetRecommendedThreadLimit_ClampsUserPreference()
        {
            // Test lower bound
            var tooLow = ThreadLimitCalculator.GetRecommendedThreadLimit(0, false);
            Assert.AreEqual(1, tooLow, "Should clamp to minimum 1");

            // Test upper bound
            var tooHigh = ThreadLimitCalculator.GetRecommendedThreadLimit(100, false);
            Assert.AreEqual(8, tooHigh, "Should clamp to maximum 8");
        }

        #endregion

        #region Performance Category Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetPerformanceCategory_ReturnsValidCategory()
        {
            // Act
            var category = ThreadLimitCalculator.GetPerformanceCategory();

            // Assert
            Assert.IsTrue(Enum.IsDefined(typeof(PerformanceCategory), category),
                "Should return valid performance category");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetPerformanceCategory_ConsistentWithProcessorCount()
        {
            // Arrange
            var processorCount = ThreadLimitCalculator.ProcessorCount;
            var category = ThreadLimitCalculator.GetPerformanceCategory();

            // Assert - Verify category matches processor count ranges
            if (processorCount <= 2)
                Assert.AreEqual(PerformanceCategory.Low, category);
            else if (processorCount <= 4)
                Assert.AreEqual(PerformanceCategory.Medium, category);
            else if (processorCount <= 8)
                Assert.AreEqual(PerformanceCategory.High, category);
            else
                Assert.AreEqual(PerformanceCategory.VeryHigh, category);
        }

        #endregion

        #region System Info Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetSystemInfo_ReturnsNonEmptyString()
        {
            // Act
            var systemInfo = ThreadLimitCalculator.GetSystemInfo();

            // Assert
            Assert.IsNotNull(systemInfo, "System info should not be null");
            Assert.IsNotEmpty(systemInfo, "System info should not be empty");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetSystemInfo_ContainsKeyMetrics()
        {
            // Act
            var systemInfo = ThreadLimitCalculator.GetSystemInfo();

            // Assert
            Assert.That(systemInfo.Contains("CPU Cores"), "Should contain CPU cores info");
            Assert.That(systemInfo.Contains("Optimal Threads"), "Should contain optimal threads info");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetRecommendationText_ReturnsNonEmptyString()
        {
            // Act
            var recommendation = ThreadLimitCalculator.GetRecommendationText();

            // Assert
            Assert.IsNotNull(recommendation, "Recommendation should not be null");
            Assert.IsNotEmpty(recommendation, "Recommendation should not be empty");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void GetRecommendationText_ContainsSystemCategory()
        {
            // Act
            var recommendation = ThreadLimitCalculator.GetRecommendationText();

            // Assert - Should mention system type
            Assert.That(
                recommendation.Contains("Low-end") ||
                recommendation.Contains("Mid-range") ||
                recommendation.Contains("High-end") ||
                recommendation.Contains("Very high-end"),
                "Should contain system category description"
            );
        }

        #endregion

        #region Performance Tests

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void CalculateOptimalThreadLimit_Performance_Fast()
        {
            // Arrange
            ThreadLimitCalculator.ClearCache();

            // Act - Measure first call (uncached)
            var metrics = TestHelpers.MeasurePerformance(
                () => ThreadLimitCalculator.CalculateOptimalThreadLimit(),
                100
            );

            // Assert - Should be very fast (< 1ms average)
            Assert.Less(metrics.AverageTimePerIteration, 1.0,
                $"Calculation should be fast, actual: {metrics.AverageTimePerIteration:F2}ms");
        }

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void CalculateOptimalThreadLimit_Cached_VeryFast()
        {
            // Arrange - Prime cache
            ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Act - Measure cached calls
            var metrics = TestHelpers.MeasurePerformance(
                () => ThreadLimitCalculator.CalculateOptimalThreadLimit(),
                1000
            );

            // Assert - Cached calls should be extremely fast (< 0.1ms)
            Assert.Less(metrics.AverageTimePerIteration, 0.1,
                $"Cached calls should be very fast, actual: {metrics.AverageTimePerIteration:F2}ms");
        }

        #endregion

        #region Edge Cases and Error Handling

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetRecommendedThreadLimit_WithZeroPreference_AutoMode()
        {
            // Arrange
            int userPreference = 0;
            bool autoEnabled = true;

            // Act
            var recommended = ThreadLimitCalculator.GetRecommendedThreadLimit(userPreference, autoEnabled);
            var optimal = ThreadLimitCalculator.CalculateOptimalThreadLimit();

            // Assert
            Assert.AreEqual(optimal, recommended, "Zero preference in auto mode should return optimal");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetRecommendedThreadLimit_WithNegativePreference_Clamped()
        {
            // Arrange
            int userPreference = -5;
            bool autoEnabled = false;

            // Act
            var recommended = ThreadLimitCalculator.GetRecommendedThreadLimit(userPreference, autoEnabled);

            // Assert
            Assert.AreEqual(1, recommended, "Negative preference should be clamped to 1");
        }

        #endregion

        #region Integration Tests

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void ThreadLimitCalculator_IntegrationWithEnvironment_Works()
        {
            // Act
            var processorCount = ThreadLimitCalculator.ProcessorCount;
            var optimal = ThreadLimitCalculator.CalculateOptimalThreadLimit();
            var category = ThreadLimitCalculator.GetPerformanceCategory();
            var systemInfo = ThreadLimitCalculator.GetSystemInfo();
            var recommendation = ThreadLimitCalculator.GetRecommendationText();

            // Assert - All methods should work together
            Assert.Greater(processorCount, 0);
            Assert.GreaterOrEqual(optimal, 1);
            Assert.LessOrEqual(optimal, 8);
            Assert.IsNotEmpty(systemInfo);
            Assert.IsNotEmpty(recommendation);
            
            // Verify logical consistency
            Assert.That(systemInfo.Contains(processorCount.ToString()),
                "System info should contain processor count");
            Assert.That(systemInfo.Contains(optimal.ToString()),
                "System info should contain optimal threads");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void ThreadLimitCalculator_MultipleCalls_Consistent()
        {
            // Act - Multiple calls in sequence
            var results = new int[10];
            for (int i = 0; i < 10; i++)
            {
                results[i] = ThreadLimitCalculator.CalculateOptimalThreadLimit();
            }

            // Assert - All results should be identical (cached)
            for (int i = 1; i < 10; i++)
            {
                Assert.AreEqual(results[0], results[i],
                    "All cached calls should return same result");
            }
        }

        #endregion

        #region Realistic Scenario Tests

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void Scenario_UserEnablesAuto_GetsOptimalThreads()
        {
            // Arrange - Simulate user enabling auto mode
            bool autoEnabled = true;
            int userManualPreference = 4;

            // Act
            var threadCount = ThreadLimitCalculator.GetRecommendedThreadLimit(
                userManualPreference,
                autoEnabled
            );

            // Assert
            var optimal = ThreadLimitCalculator.CalculateOptimalThreadLimit();
            Assert.AreEqual(optimal, threadCount,
                "Auto mode should override manual preference");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void Scenario_UserDisablesAuto_UsesManualSetting()
        {
            // Arrange - Simulate user disabling auto mode
            bool autoEnabled = false;
            int userManualPreference = 6;

            // Act
            var threadCount = ThreadLimitCalculator.GetRecommendedThreadLimit(
                userManualPreference,
                autoEnabled
            );

            // Assert
            Assert.AreEqual(userManualPreference, threadCount,
                "Manual mode should use user preference");
        }

        #endregion
    }
}

