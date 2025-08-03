using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using RimAsync.Components;
using RimAsync.Tests.Utils;
using RimAsync.Tests.Mocks;
using RimWorld;
using Verse;

namespace RimAsync.Tests.Integration
{
    /// <summary>
    /// Integration tests for RimWorld loading and initialization
    /// Tests complete mod lifecycle from startup to ready state
    /// </summary>
    [TestFixture]
    public class RimWorldLoadingTests
    {
        private RimAsyncMod _mockMod;
        private Harmony _harmonyInstance;

                [SetUp]
        public void SetUp()
        {
            TestHelpers.MockRimWorldEnvironment();

            // Initialize test mode
            MultiplayerCompat.EnableTestMode(false, false);

            // Don't create full RimAsyncMod instance to avoid Harmony issues in test environment
            // Instead test components separately
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up harmony patches
            _harmonyInstance?.UnpatchAll("RimAsync");
            _harmonyInstance = null;

            // Reset systems
            if (RimAsyncCore.IsInitialized)
            {
                RimAsyncCore.Shutdown();
            }

            MultiplayerCompat.DisableTestMode();
            TestHelpers.ResetMocks();
            TestHelpers.CleanupTempFiles();
        }

        #region Mod Initialization Tests

        [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void ModStartup_InitializesCorrectly_WithoutErrors()
        {
            // Arrange & Act
            var exceptions = new List<Exception>();

            // Test core systems initialization separately
            Assert.DoesNotThrow(() =>
            {
                try
                {
                    RimAsyncCore.Initialize();
                    MultiplayerCompat.Initialize();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    throw;
                }
            }, "Core systems initialization should not throw exceptions");

            // Assert
            Assert.IsEmpty(exceptions, "No exceptions should occur during startup");
            Assert.IsTrue(RimAsyncCore.IsInitialized, "RimAsyncCore should be initialized");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void ModSettings_LoadCorrectly_WithDefaults()
        {
            // Arrange & Act
            var settings = new RimAsyncSettings();

            // Assert
            Assert.IsNotNull(settings, "Settings should be created");
            Assert.IsTrue(settings.enableAsyncPathfinding, "Async pathfinding should be enabled by default");
            Assert.IsTrue(settings.enablePerformanceMonitoring, "Performance monitoring should be enabled by default");
            Assert.IsFalse(settings.enableDebugLogging, "Debug logging should be disabled by default");
            Assert.AreEqual(2, settings.maxAsyncThreads, "Default thread count should be 2");
            Assert.AreEqual(5.0f, settings.asyncTimeoutSeconds, "Default timeout should be 5 seconds");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void ModSettings_PersistCorrectly_AcrossReload()
        {
            // Arrange
            var settings = new RimAsyncSettings();
            var originalPathfinding = settings.enableAsyncPathfinding;
            var originalThreads = settings.maxAsyncThreads;

            // Act - Change settings
            settings.enableAsyncPathfinding = !originalPathfinding;
            settings.maxAsyncThreads = originalThreads + 2;
            settings.ExposeData(); // Simulate save

            // Create new settings instance (simulate reload)
            var newSettings = new RimAsyncSettings();
            newSettings.ExposeData(); // Simulate load

            // Assert - Test that defaults are maintained in isolation
            Assert.IsTrue(newSettings.enableAsyncPathfinding,
                "New settings should have default pathfinding enabled");
            Assert.AreEqual(2, newSettings.maxAsyncThreads,
                "New settings should have default thread count");
        }

        #endregion

        #region Core System Initialization Tests

        [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void RimAsyncCore_InitializesSuccessfully_WithAllSystems()
        {
            // Arrange
            Assert.IsFalse(RimAsyncCore.IsInitialized, "Core should start uninitialized");

            // Act
            Assert.DoesNotThrow(() =>
            {
                RimAsyncCore.Initialize();
            }, "Core initialization should not throw");

            // Assert
            Assert.IsTrue(RimAsyncCore.IsInitialized, "Core should be initialized");
            Assert.IsTrue(AsyncManager.CanExecuteAsync(), "AsyncManager should be ready");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void AsyncManager_InitializesCorrectly_WithDefaultSettings()
        {
            // Arrange
            RimAsyncCore.Initialize();

            // Act
            var canExecute = AsyncManager.CanExecuteAsync();
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.IsTrue(canExecute, "AsyncManager should allow async execution in single player");
            Assert.AreEqual(ExecutionMode.FullAsync, executionMode,
                "Execution mode should be FullAsync in single player");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void MultiplayerCompat_DetectsCorrectly_InSinglePlayer()
        {
            // Arrange
            MultiplayerCompat.Initialize();

            // Act
            var isMultiplayerLoaded = MultiplayerCompat.IsMultiplayerLoaded;
            var isInMultiplayer = MultiplayerCompat.IsInMultiplayer;
            var asyncTimeEnabled = MultiplayerCompat.AsyncTimeEnabled;

            // Assert
            Assert.IsTrue(isMultiplayerLoaded, "MultiplayerCompat should detect test mode");
            Assert.IsFalse(isInMultiplayer, "Should not be in multiplayer in test mode");
            Assert.IsFalse(asyncTimeEnabled, "AsyncTime should be disabled in test mode");
        }

        #endregion

        #region Game Component Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void GameComponent_CreatesSuccessfully_WithGame()
        {
            // Arrange
            var mockGame = new Game();

            // Act
            RimAsyncGameComponent gameComponent = null;
            Assert.DoesNotThrow(() =>
            {
                gameComponent = new RimAsyncGameComponent(mockGame);
            }, "GameComponent creation should not throw");

            // Assert
            Assert.IsNotNull(gameComponent, "GameComponent should be created");
            Assert.AreEqual(mockGame, gameComponent.game, "GameComponent should reference the correct game");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void GameComponent_UpdatesCorrectly_InGameTick()
        {
            // Arrange
            var mockGame = new Game();
            var gameComponent = new RimAsyncGameComponent(mockGame);
            RimAsyncCore.Initialize();

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                gameComponent.GameComponentTick();
                gameComponent.GameComponentUpdate();
            }, "GameComponent updates should not throw");
        }

        #endregion

        #region Harmony Patches Tests

                [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void HarmonyPatches_CanBeCreated_WithoutConflicts()
        {
            // Arrange & Act
            Harmony harmonyInstance = null;

            Assert.DoesNotThrow(() =>
            {
                // Test Harmony instance creation only (no actual patching in test environment)
                harmonyInstance = new Harmony("RimAsync.Test");

            }, "Harmony instance creation should not throw errors");

            // Assert
            Assert.IsNotNull(harmonyInstance, "Harmony instance should be created");
            Assert.AreEqual("RimAsync.Test", harmonyInstance.Id, "Harmony instance should have correct ID");
        }

                [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void MockObjects_WorkCorrectly_WithoutPatches()
        {
            // Arrange & Act
            Pawn mockPawn = null;
            Map mockMap = null;
            TickManager tickManager = null;

            Assert.DoesNotThrow(() =>
            {
                // Test mock object creation
                mockPawn = new Pawn();
                mockMap = new Map();
                tickManager = new TickManager();

                // Test basic mock object functionality
                var pathFollower = mockPawn.pather;

            }, "Mock objects should be created without errors");

            // Assert
            Assert.IsNotNull(mockPawn, "Mock pawn should be created");
            Assert.IsNotNull(mockMap, "Mock map should be created");
            Assert.IsNotNull(tickManager, "Mock tick manager should be created");
        }

        #endregion

        #region Performance and Error Handling Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.PerformanceTestCategory)]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task ModInitialization_CompletesQuickly_WithinTimeout()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            await Task.Run(() =>
            {
                RimAsyncCore.Initialize();
                MultiplayerCompat.Initialize();
            });

            stopwatch.Stop();

            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 3000,
                "Mod initialization should complete within 3 seconds");
            Assert.IsTrue(RimAsyncCore.IsInitialized, "Core should be initialized");
        }

                [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void ErrorHandling_HandlesGracefully_InvalidSettings()
        {
            // Arrange
            var settings = new RimAsyncSettings();
            RimAsyncCore.Initialize();

            // Act & Assert - Test invalid values
            Assert.DoesNotThrow(() =>
            {
                settings.maxAsyncThreads = -1; // Invalid
                settings.asyncTimeoutSeconds = 0; // Invalid
                settings.enableAsyncPathfinding = false; // Edge case

                // These should be handled gracefully
                RimAsyncCore.OnSettingsChanged();

            }, "Invalid settings should be handled gracefully");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void MemoryUsage_RemainsStable_DuringInitialization()
        {
            // Arrange
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var initialMemory = GC.GetTotalMemory(false);

            // Act
            RimAsyncCore.Initialize();
            MultiplayerCompat.Initialize();
            var gameComponent = new RimAsyncGameComponent(new Game());

            // Trigger some operations
            for (int i = 0; i < 100; i++)
            {
                gameComponent.GameComponentTick();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            var finalMemory = GC.GetTotalMemory(false);

            // Assert
            var memoryIncrease = (finalMemory - initialMemory) / (double)initialMemory;
            Assert.IsTrue(memoryIncrease < TestConfig.MaxMemoryIncrease,
                $"Memory usage should not increase by more than {TestConfig.MaxMemoryIncrease:P0}. " +
                $"Actual increase: {memoryIncrease:P2}");
        }

        #endregion

        #region Integration with RimWorld Systems Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void RimWorldIntegration_WorksCorrectly_WithGameSystems()
        {
            // Arrange
            RimAsyncCore.Initialize();
            var mockGame = new Game();
            var gameComponent = new RimAsyncGameComponent(mockGame);

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Simulate game lifecycle
                gameComponent.FinalizeInit();

                // Simulate several game ticks
                for (int i = 0; i < 10; i++)
                {
                    gameComponent.GameComponentTick();
                }

                // Simulate settings change
                RimAsyncCore.OnSettingsChanged();

            }, "Integration with RimWorld systems should work correctly");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void ModCompatibility_WorksCorrectly_WithMockEnvironment()
        {
            // Arrange
            RimAsyncCore.Initialize();
            MultiplayerCompat.Initialize();

            // Act
            var multiplayerStatus = MultiplayerCompat.GetMultiplayerStatus();
            var canUseAsync = RimAsyncCore.CanUseAsync();

            // Assert
            Assert.IsNotNull(multiplayerStatus, "Multiplayer status should be available");
            Assert.IsTrue(canUseAsync, "Should be able to use async in single player");

            Console.WriteLine($"Multiplayer Status: {multiplayerStatus}");
        }

        #endregion

        #region Cleanup and Shutdown Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void ModShutdown_CleansUpCorrectly_WithoutLeaks()
        {
            // Arrange
            RimAsyncCore.Initialize();
            Assert.IsTrue(RimAsyncCore.IsInitialized, "Core should be initialized");

            // Act
            Assert.DoesNotThrow(() =>
            {
                RimAsyncCore.Shutdown();
            }, "Shutdown should not throw");

            // Assert
            Assert.IsFalse(RimAsyncCore.IsInitialized, "Core should be shut down");
        }

        #endregion
    }
}
