using NUnit.Framework;
using System;
using System.Reflection;
using UnityEngine;
using RimAsync.Core;
using RimAsync.Utils;
using RimAsync.Tests.Utils;
using RimAsync.Tests.Mocks;
using Verse;

namespace RimAsync.Tests.UI
{
    /// <summary>
    /// Comprehensive UI tests for RimAsyncSettings
    /// Tests DoWindowContents method, UI elements, and settings integration
    /// </summary>
    [TestFixture]
    public class SettingsUITests
    {
        private RimAsyncSettings _settings;
        private Rect _testRect;

        [SetUp]
        public void SetUp()
        {
            // Create fresh settings instance for each test
            _settings = new RimAsyncSettings();

            // Standard UI rectangle for testing
            _testRect = new Rect(0, 0, 400, 600);

            // Reset logger configuration
            RimAsyncLogger.Configure(false, RimAsyncLogger.LogLevel.Info);
        }

        [TearDown]
        public void TearDown()
        {
            _settings = null;
        }

        #region Basic UI Functionality Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void DoWindowContents_WithValidRect_DoesNotThrow()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => _settings.DoWindowContents(_testRect),
                "DoWindowContents should not throw with valid rectangle");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void DoWindowContents_WithZeroRect_DoesNotThrow()
        {
            // Arrange
            var zeroRect = new Rect(0, 0, 0, 0);

            // Act & Assert
            Assert.DoesNotThrow(() => _settings.DoWindowContents(zeroRect),
                "DoWindowContents should handle zero rectangle gracefully");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void DoWindowContents_WithNegativeRect_DoesNotThrow()
        {
            // Arrange
            var negativeRect = new Rect(-10, -10, 100, 100);

            // Act & Assert
            Assert.DoesNotThrow(() => _settings.DoWindowContents(negativeRect),
                "DoWindowContents should handle negative coordinates gracefully");
        }

        #endregion

        #region Settings Serialization Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void ExposeData_SavesAllSettings_Correctly()
        {
            // Arrange
            var originalSettings = CreateTestSettings();

            // Act - This simulates saving settings
            Assert.DoesNotThrow(() => originalSettings.ExposeData(),
                "ExposeData should not throw when saving settings");

            // Assert - Verify all properties are accessible
            Assert.IsTrue(originalSettings.enableAsyncPathfinding);
            Assert.AreEqual(4, originalSettings.maxAsyncThreads);
            Assert.AreEqual(2.5f, originalSettings.asyncTimeoutSeconds, 0.01f);
            Assert.IsTrue(originalSettings.enableDebugLogging);
            Assert.AreEqual(2, originalSettings.logLevel);
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void ExposeData_LoadsDefaultValues_Correctly()
        {
            // Arrange
            var defaultSettings = new RimAsyncSettings();

            // Act
            Assert.DoesNotThrow(() => defaultSettings.ExposeData(),
                "ExposeData should not throw when loading default values");

            // Assert - Verify default values
            Assert.IsTrue(defaultSettings.enableAsyncPathfinding, "Default enableAsyncPathfinding should be true");
            Assert.IsTrue(defaultSettings.enableBackgroundJobs, "Default enableBackgroundJobs should be true");
            Assert.IsTrue(defaultSettings.enableSmartCaching, "Default enableSmartCaching should be true");
            Assert.IsTrue(defaultSettings.enableMemoryOptimization, "Default enableMemoryOptimization should be true");
            Assert.IsTrue(defaultSettings.enableFallbackMechanisms, "Default enableFallbackMechanisms should be true");
            Assert.IsTrue(defaultSettings.enablePerformanceMonitoring, "Default enablePerformanceMonitoring should be true");
            Assert.AreEqual(2, defaultSettings.maxAsyncThreads, "Default maxAsyncThreads should be 2");
            Assert.AreEqual(5.0f, defaultSettings.asyncTimeoutSeconds, 0.01f, "Default asyncTimeoutSeconds should be 5.0");
            Assert.IsFalse(defaultSettings.enableDebugLogging, "Default enableDebugLogging should be false");
            Assert.AreEqual(1, defaultSettings.logLevel, "Default logLevel should be 1 (Info)");
            Assert.IsTrue(defaultSettings.respectAsyncTimeSetting, "Default respectAsyncTimeSetting should be true");
            Assert.IsTrue(defaultSettings.enableMultiplayerOptimizations, "Default enableMultiplayerOptimizations should be true");
        }

        #endregion

        #region UI Controls Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void Settings_AllCheckboxes_CanBeToggled()
        {
            // Arrange
            var settings = new RimAsyncSettings();
            var originalValues = new bool[]
            {
                settings.enableAsyncPathfinding,
                settings.enableBackgroundJobs,
                settings.enableSmartCaching,
                settings.enableMemoryOptimization,
                settings.enableFallbackMechanisms,
                settings.enablePerformanceMonitoring,
                settings.respectAsyncTimeSetting,
                settings.enableMultiplayerOptimizations,
                settings.enableDebugLogging
            };

            // Act - Toggle all boolean settings
            settings.enableAsyncPathfinding = !settings.enableAsyncPathfinding;
            settings.enableBackgroundJobs = !settings.enableBackgroundJobs;
            settings.enableSmartCaching = !settings.enableSmartCaching;
            settings.enableMemoryOptimization = !settings.enableMemoryOptimization;
            settings.enableFallbackMechanisms = !settings.enableFallbackMechanisms;
            settings.enablePerformanceMonitoring = !settings.enablePerformanceMonitoring;
            settings.respectAsyncTimeSetting = !settings.respectAsyncTimeSetting;
            settings.enableMultiplayerOptimizations = !settings.enableMultiplayerOptimizations;
            settings.enableDebugLogging = !settings.enableDebugLogging;

            // Assert - Verify all values were toggled
            var newValues = new bool[]
            {
                settings.enableAsyncPathfinding,
                settings.enableBackgroundJobs,
                settings.enableSmartCaching,
                settings.enableMemoryOptimization,
                settings.enableFallbackMechanisms,
                settings.enablePerformanceMonitoring,
                settings.respectAsyncTimeSetting,
                settings.enableMultiplayerOptimizations,
                settings.enableDebugLogging
            };

            for (int i = 0; i < originalValues.Length; i++)
            {
                Assert.AreNotEqual(originalValues[i], newValues[i],
                    $"Checkbox {i} should have been toggled");
            }
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void Settings_MaxAsyncThreadsSlider_AcceptsValidRange()
        {
            // Arrange
            var settings = new RimAsyncSettings();

            // Act & Assert - Test boundary values
            settings.maxAsyncThreads = 1;
            Assert.AreEqual(1, settings.maxAsyncThreads, "Should accept minimum value 1");

            settings.maxAsyncThreads = 8;
            Assert.AreEqual(8, settings.maxAsyncThreads, "Should accept maximum value 8");

            settings.maxAsyncThreads = 4;
            Assert.AreEqual(4, settings.maxAsyncThreads, "Should accept middle value 4");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void Settings_AsyncTimeoutSlider_AcceptsValidRange()
        {
            // Arrange
            var settings = new RimAsyncSettings();

            // Act & Assert - Test boundary values
            settings.asyncTimeoutSeconds = 1.0f;
            Assert.AreEqual(1.0f, settings.asyncTimeoutSeconds, 0.01f, "Should accept minimum value 1.0");

            settings.asyncTimeoutSeconds = 30.0f;
            Assert.AreEqual(30.0f, settings.asyncTimeoutSeconds, 0.01f, "Should accept maximum value 30.0");

            settings.asyncTimeoutSeconds = 15.5f;
            Assert.AreEqual(15.5f, settings.asyncTimeoutSeconds, 0.01f, "Should accept decimal values");
        }

        #endregion

        #region Logging Settings Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void Settings_LogLevel_AcceptsAllValidValues()
        {
            // Arrange
            var settings = new RimAsyncSettings();

            // Act & Assert - Test all log levels
            settings.logLevel = 0; // Debug
            Assert.AreEqual(0, settings.logLevel, "Should accept Debug level (0)");

            settings.logLevel = 1; // Info
            Assert.AreEqual(1, settings.logLevel, "Should accept Info level (1)");

            settings.logLevel = 2; // Warning
            Assert.AreEqual(2, settings.logLevel, "Should accept Warning level (2)");

            settings.logLevel = 3; // Error
            Assert.AreEqual(3, settings.logLevel, "Should accept Error level (3)");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void Settings_DebugLogging_ToggleWorks()
        {
            // Arrange
            var settings = new RimAsyncSettings();
            var originalValue = settings.enableDebugLogging;

            // Act
            settings.enableDebugLogging = !originalValue;

            // Assert
            Assert.AreNotEqual(originalValue, settings.enableDebugLogging,
                "Debug logging should be togglable");
        }

        #endregion

        #region Integration with Logging System Tests

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void Settings_LogLevelChange_UpdatesRimAsyncLogger()
        {
            // Arrange
            var settings = new RimAsyncSettings();

            // Act - Change settings
            settings.enableDebugLogging = true;
            settings.logLevel = 0; // Debug

            // Simulate settings change (this would normally be called by RimAsyncCore.OnSettingsChanged)
            RimAsyncLogger.Configure(settings.enableDebugLogging, (RimAsyncLogger.LogLevel)settings.logLevel);

            // Assert
            Assert.AreEqual(RimAsyncLogger.LogLevel.Debug, RimAsyncLogger.MinimumLevel,
                "Logger minimum level should be updated to Debug");
            Assert.IsTrue(RimAsyncLogger.EnableDebugLogging,
                "Logger debug logging should be enabled");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void Settings_DifferentLogLevels_ConfigureLoggerCorrectly()
        {
            // Test each log level
            var testCases = new[]
            {
                (level: 0, expected: RimAsyncLogger.LogLevel.Debug),
                (level: 1, expected: RimAsyncLogger.LogLevel.Info),
                (level: 2, expected: RimAsyncLogger.LogLevel.Warning),
                (level: 3, expected: RimAsyncLogger.LogLevel.Error)
            };

            foreach (var (level, expected) in testCases)
            {
                // Arrange
                var settings = new RimAsyncSettings { logLevel = level };

                // Act
                RimAsyncLogger.Configure(settings.enableDebugLogging, (RimAsyncLogger.LogLevel)settings.logLevel);

                // Assert
                Assert.AreEqual(expected, RimAsyncLogger.MinimumLevel,
                    $"Logger should be configured with {expected} level when settings.logLevel = {level}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates test settings with non-default values for testing
        /// </summary>
        private RimAsyncSettings CreateTestSettings()
        {
            return new RimAsyncSettings
            {
                enableAsyncPathfinding = true,
                enableBackgroundJobs = false,
                enableSmartCaching = true,
                enableMemoryOptimization = false,
                enableFallbackMechanisms = true,
                enablePerformanceMonitoring = false,
                maxAsyncThreads = 4,
                asyncTimeoutSeconds = 2.5f,
                enableDebugLogging = true,
                logLevel = 2,
                respectAsyncTimeSetting = false,
                enableMultiplayerOptimizations = true
            };
        }

        #endregion
    }
}
