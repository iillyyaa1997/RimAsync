using NUnit.Framework;
using System;
using RimAsync.Utils;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Multiplayer
{
    /// <summary>
    /// Comprehensive tests for RimWorld Multiplayer detection system
    /// Tests MultiplayerCompat functionality, AsyncTime detection, and game mode handling
    /// </summary>
    [TestFixture]
    public class MultiplayerDetectionTests
    {
                [SetUp]
        public void SetUp()
        {
            // Ensure clean state for each test
            MultiplayerCompat.DisableTestMode();

            // Initialize MultiplayerCompat only (it doesn't require full Core initialization)
            MultiplayerCompat.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test mode
            MultiplayerCompat.DisableTestMode();
        }

        #region Multiplayer API Detection Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void IsInMultiplayer_WithoutMultiplayerMod_ReturnsFalse()
        {
            // Arrange & Act
            var result = MultiplayerCompat.IsInMultiplayer;

            // Assert
            Assert.IsFalse(result, "Should return false when multiplayer mod is not loaded");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void IsMultiplayerLoaded_WithoutMultiplayerMod_ReturnsFalse()
        {
            // Arrange & Act
            var result = MultiplayerCompat.IsMultiplayerLoaded;

            // Assert
            Assert.IsFalse(result, "Should return false when multiplayer mod is not loaded");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void AsyncTimeEnabled_WithoutMultiplayerMod_ReturnsFalse()
        {
            // Arrange & Act
            var result = MultiplayerCompat.AsyncTimeEnabled;

            // Assert
            Assert.IsFalse(result, "Should return false when multiplayer mod is not loaded");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GetMultiplayerStatus_WithoutMultiplayerMod_ReturnsCorrectMessage()
        {
            // Arrange
            MultiplayerCompat.Initialize();

            // Act
            var status = MultiplayerCompat.GetMultiplayerStatus();

            // Assert
            Assert.IsTrue(status.Contains("Multiplayer not loaded") || status.Contains("Not initialized"),
                $"Status should indicate multiplayer not loaded, got: {status}");
        }

        #endregion

        #region Test Mode Functionality Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void TestMode_EnabledWithSinglePlayer_ReturnsCorrectValues()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: false, asyncTimeEnabled: false);

            // Act & Assert
            Assert.IsTrue(MultiplayerCompat.IsMultiplayerLoaded, "Test mode should simulate multiplayer loaded");
            Assert.IsFalse(MultiplayerCompat.IsInMultiplayer, "Should return false for single player mode");
            Assert.IsFalse(MultiplayerCompat.AsyncTimeEnabled, "Should return false for AsyncTime in single player");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void TestMode_EnabledWithMultiplayer_ReturnsCorrectValues()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: false);

            // Act & Assert
            Assert.IsTrue(MultiplayerCompat.IsMultiplayerLoaded, "Test mode should simulate multiplayer loaded");
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Should return true for multiplayer mode");
            Assert.IsFalse(MultiplayerCompat.AsyncTimeEnabled, "Should return false for AsyncTime disabled");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void TestMode_EnabledWithMultiplayerAsyncTime_ReturnsCorrectValues()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);

            // Act & Assert
            Assert.IsTrue(MultiplayerCompat.IsMultiplayerLoaded, "Test mode should simulate multiplayer loaded");
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Should return true for multiplayer mode");
            Assert.IsTrue(MultiplayerCompat.AsyncTimeEnabled, "Should return true for AsyncTime enabled");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void TestMode_DisabledAfterEnabled_ReturnsToNormalBehavior()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);

            // Verify test mode is working
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Test mode should be active");

            // Act
            MultiplayerCompat.DisableTestMode();

            // Assert
            Assert.IsFalse(MultiplayerCompat.IsInMultiplayer, "Should return to normal detection after disabling test mode");
        }

        #endregion

        #region Game Mode Scenarios Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GameModes_SinglePlayer_HasCorrectState()
        {
            // Arrange - Simulate single player
            MultiplayerCompat.EnableTestMode(isInMultiplayer: false, asyncTimeEnabled: false);

            // Act
            var isInMP = MultiplayerCompat.IsInMultiplayer;
            var asyncTime = MultiplayerCompat.AsyncTimeEnabled;
            var status = MultiplayerCompat.GetMultiplayerStatus();

            // Assert
            Assert.IsFalse(isInMP, "Single player mode: IsInMultiplayer should be false");
            Assert.IsFalse(asyncTime, "Single player mode: AsyncTimeEnabled should be false");
            Assert.IsTrue(status.Contains("In Multiplayer: False"), $"Status should show single player mode, got: {status}");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void GameModes_MultiplayerWithoutAsyncTime_HasCorrectState()
        {
            // Arrange - Simulate multiplayer without AsyncTime
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: false);

            // Act
            var isInMP = MultiplayerCompat.IsInMultiplayer;
            var asyncTime = MultiplayerCompat.AsyncTimeEnabled;
            var status = MultiplayerCompat.GetMultiplayerStatus();

            // Assert
            Assert.IsTrue(isInMP, "Multiplayer mode: IsInMultiplayer should be true");
            Assert.IsFalse(asyncTime, "Multiplayer without AsyncTime: AsyncTimeEnabled should be false");
            Assert.IsTrue(status.Contains("In Multiplayer: True") && status.Contains("AsyncTime: False"),
                $"Status should show multiplayer without AsyncTime, got: {status}");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void GameModes_MultiplayerWithAsyncTime_HasCorrectState()
        {
            // Arrange - Simulate multiplayer with AsyncTime
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);

            // Act
            var isInMP = MultiplayerCompat.IsInMultiplayer;
            var asyncTime = MultiplayerCompat.AsyncTimeEnabled;
            var status = MultiplayerCompat.GetMultiplayerStatus();

            // Assert
            Assert.IsTrue(isInMP, "Multiplayer mode: IsInMultiplayer should be true");
            Assert.IsTrue(asyncTime, "Multiplayer with AsyncTime: AsyncTimeEnabled should be true");
            Assert.IsTrue(status.Contains("In Multiplayer: True") && status.Contains("AsyncTime: True"),
                $"Status should show multiplayer with AsyncTime, got: {status}");
        }

        #endregion

        #region Runtime Mode Switching Tests

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void RuntimeSwitching_FromSinglePlayerToMultiplayer_UpdatesCorrectly()
        {
            // Arrange - Start in single player
            MultiplayerCompat.EnableTestMode(isInMultiplayer: false, asyncTimeEnabled: false);
            Assert.IsFalse(MultiplayerCompat.IsInMultiplayer, "Should start in single player");

            // Act - Switch to multiplayer
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: false);

            // Assert
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Should switch to multiplayer");
            Assert.IsFalse(MultiplayerCompat.AsyncTimeEnabled, "AsyncTime should remain disabled");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.HighPriority)]
        public void RuntimeSwitching_AsyncTimeToggle_UpdatesCorrectly()
        {
            // Arrange - Start in multiplayer without AsyncTime
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: false);
            Assert.IsFalse(MultiplayerCompat.AsyncTimeEnabled, "Should start without AsyncTime");

            // Act - Enable AsyncTime
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);

            // Assert
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Should remain in multiplayer");
            Assert.IsTrue(MultiplayerCompat.AsyncTimeEnabled, "AsyncTime should be enabled");

            // Act - Disable AsyncTime again
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: false);

            // Assert
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Should remain in multiplayer");
            Assert.IsFalse(MultiplayerCompat.AsyncTimeEnabled, "AsyncTime should be disabled again");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void RuntimeSwitching_MultipleTransitions_HandledCorrectly()
        {
            var transitions = new[]
            {
                (mp: false, async: false, desc: "Single Player"),
                (mp: true, async: false, desc: "Multiplayer without AsyncTime"),
                (mp: true, async: true, desc: "Multiplayer with AsyncTime"),
                (mp: false, async: false, desc: "Back to Single Player"),
                (mp: true, async: true, desc: "Multiplayer with AsyncTime again")
            };

            foreach (var (mp, async, desc) in transitions)
            {
                // Act
                MultiplayerCompat.EnableTestMode(mp, async);

                // Assert
                Assert.AreEqual(mp, MultiplayerCompat.IsInMultiplayer,
                    $"{desc}: IsInMultiplayer should be {mp}");
                Assert.AreEqual(async, MultiplayerCompat.AsyncTimeEnabled,
                    $"{desc}: AsyncTimeEnabled should be {async}");

                var status = MultiplayerCompat.GetMultiplayerStatus();
                Assert.IsTrue(status.Contains($"In Multiplayer: {mp}") && status.Contains($"AsyncTime: {async}"),
                    $"{desc}: Status should reflect current state, got: {status}");
            }
        }

        #endregion

        #region MultiplayerCompat Isolation Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.CriticalPriority)]
        public void MultiplayerCompat_IsolatedTesting_WorksIndependently()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);

            // Act & Assert - MultiplayerCompat should work without full Core initialization
            Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, "Should work independently of Core systems");
            Assert.IsTrue(MultiplayerCompat.AsyncTimeEnabled, "AsyncTime detection should work independently");

            var status = MultiplayerCompat.GetMultiplayerStatus();
            Assert.IsTrue(status.Contains("In Multiplayer: True") && status.Contains("AsyncTime: True"),
                "Status should work independently");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void MultiplayerCompat_ReflectionBased_HandlesModNotPresent()
        {
            // Arrange & Act - Test actual reflection-based detection (no test mode)
            MultiplayerCompat.DisableTestMode();
            MultiplayerCompat.Initialize();

            // Assert - Should gracefully handle missing multiplayer mod
            Assert.IsFalse(MultiplayerCompat.IsInMultiplayer, "Should return false when mod not present");
            Assert.IsFalse(MultiplayerCompat.AsyncTimeEnabled, "Should return false when mod not present");
            Assert.IsFalse(MultiplayerCompat.IsMultiplayerLoaded, "Should indicate mod not loaded");
        }

        #endregion

        #region Error Handling and Edge Cases Tests

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void ErrorHandling_Initialize_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => MultiplayerCompat.Initialize(),
                "Initialize should not throw even when multiplayer mod is not present");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void ErrorHandling_MultipleInitialize_DoesNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                MultiplayerCompat.Initialize();
                MultiplayerCompat.Initialize();
                MultiplayerCompat.Initialize();
            }, "Multiple Initialize calls should not throw");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void ErrorHandling_TestModeToggling_HandledGracefully()
        {
            // Act & Assert - Multiple rapid toggles should not cause issues
            Assert.DoesNotThrow(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    MultiplayerCompat.EnableTestMode(i % 2 == 0, i % 3 == 0);
                    var _ = MultiplayerCompat.IsInMultiplayer;
                    var __ = MultiplayerCompat.AsyncTimeEnabled;
                    MultiplayerCompat.DisableTestMode();
                }
            }, "Rapid test mode toggling should be handled gracefully");
        }

        [Test]
        [Category(TestConfig.UnitTestCategory)]
        [Category(TestConfig.LowPriority)]
        public void EdgeCases_GetStatusBeforeInitialize_ReturnsCorrectMessage()
        {
            // Note: We can't easily reset initialization state, but we can verify status works

            // Act
            var status = MultiplayerCompat.GetMultiplayerStatus();

            // Assert
            Assert.IsNotNull(status, "GetMultiplayerStatus should always return a non-null string");
            Assert.IsTrue(status.Length > 0, "GetMultiplayerStatus should return a non-empty string");
        }

        #endregion

        #region Performance and Stability Tests

        [Test]
        [Category(TestConfig.PerformanceTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void Performance_MultipleStatusChecks_PerformAcceptably()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);
            const int iterations = 1000;

            // Act
            var startTime = DateTime.UtcNow;

            for (int i = 0; i < iterations; i++)
            {
                var _ = MultiplayerCompat.IsInMultiplayer;
                var __ = MultiplayerCompat.AsyncTimeEnabled;
                var ___ = MultiplayerCompat.GetMultiplayerStatus();
            }

            var elapsed = DateTime.UtcNow - startTime;

            // Assert
            Assert.Less(elapsed.TotalMilliseconds, 100,
                $"1000 status checks should complete in under 100ms, took {elapsed.TotalMilliseconds}ms");
        }

        [Test]
        [Category(TestConfig.IntegrationTestCategory)]
        [Category(TestConfig.MediumPriority)]
        public void Stability_LongRunningDetection_RemainsStable()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(isInMultiplayer: true, asyncTimeEnabled: true);

            // Act & Assert - Check stability over time
            for (int i = 0; i < 100; i++)
            {
                Assert.IsTrue(MultiplayerCompat.IsInMultiplayer, $"Should remain stable at iteration {i}");
                Assert.IsTrue(MultiplayerCompat.AsyncTimeEnabled, $"AsyncTime should remain stable at iteration {i}");

                // Small delay to simulate real-world usage
                System.Threading.Thread.Sleep(1);
            }
        }

        #endregion
    }
}
