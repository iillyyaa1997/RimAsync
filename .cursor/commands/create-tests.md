# Create Tests Command

**Command:** `@create-tests`
**Description:** Generate comprehensive test suites for specific components or features

## 🎯 Usage

### Basic Usage
```
@create-tests AsyncManager           # Create tests for AsyncManager class
@create-tests SmartCache            # Create tests for SmartCache component
@create-tests MultiplayerCompat     # Create tests for multiplayer compatibility
```

### Advanced Usage
```
@create-tests Pathfinding --performance    # Include performance tests
@create-tests Core --integration          # Focus on integration tests
@create-tests Utils --all-types          # Create all test types
@create-tests Settings --mocking         # Include mocking examples
```

## 🔧 What the command does

1. **Analyzes target component** structure and dependencies
2. **Generates test file structure** with proper naming conventions
3. **Creates unit tests** for all public methods and properties
4. **Adds integration tests** for component interactions
5. **Includes performance tests** for critical operations
6. **Sets up mock objects** for external dependencies
7. **Configures test categories** and priorities

## 📁 Test Structure Generated

### 🧪 Unit Tests (`Tests/Unit/[Component]/`)
```
Tests/Unit/AsyncManager/
├── AsyncManagerTests.cs (main test class)
├── AsyncManagerInitializationTests.cs
├── AsyncManagerThreadingTests.cs
├── AsyncManagerErrorHandlingTests.cs
└── AsyncManagerPerformanceTests.cs
```

### 🔗 Integration Tests (`Tests/Integration/[Component]/`)
```
Tests/Integration/AsyncManager/
├── AsyncManagerIntegrationTests.cs
├── AsyncManagerWithCoreTests.cs
├── AsyncManagerWithSettingsTests.cs
└── AsyncManagerMultiplayerTests.cs
```

### ⚡ Performance Tests (`Tests/Performance/[Component]/`)
```
Tests/Performance/AsyncManager/
├── AsyncManagerBenchmarkTests.cs
├── AsyncManagerMemoryTests.cs
├── AsyncManagerThroughputTests.cs
└── AsyncManagerStressTests.cs
```

## 🎯 Test Categories

### 🔴 Critical Priority Tests
- **Core functionality** - Essential operations
- **Error handling** - Exception scenarios
- **Multiplayer safety** - Deterministic behavior
- **Memory management** - Leak prevention

### 🟠 High Priority Tests
- **Performance benchmarks** - Speed requirements
- **Integration scenarios** - Component interactions
- **Configuration validation** - Settings handling
- **Thread safety** - Concurrent operations

### 🟡 Medium Priority Tests
- **Edge cases** - Boundary conditions
- **Optimization paths** - Alternative implementations
- **Compatibility scenarios** - Different configurations
- **User interface** - Settings UI behavior

## 📝 Generated Test Templates

### Unit Test Template
```csharp
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using RimAsync.Threading;
using RimAsync.Tests.Utils;
using RimAsync.Tests.Mocks;

namespace RimAsync.Tests.Unit.Threading
{
    /// <summary>
    /// Unit tests for AsyncManager class
    /// Tests core functionality in isolation with mocked dependencies
    /// </summary>
    [TestFixture]
    public class AsyncManagerTests
    {
        private AsyncManager _asyncManager;
        private MockRimAsyncSettings _mockSettings;

        [SetUp]
        public void SetUp()
        {
            // Initialize test environment
            TestHelpers.MockRimWorldEnvironment();
            _mockSettings = new MockRimAsyncSettings();
            _asyncManager = new AsyncManager(_mockSettings);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up resources
            _asyncManager?.Dispose();
            TestHelpers.ResetMocks();
        }

        #region Initialization Tests

        [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.UnitTestCategory)]
        public void Initialize_ValidSettings_InitializesSuccessfully()
        {
            // Arrange
            _mockSettings.maxAsyncThreads = 4;
            _mockSettings.enableAsyncPathfinding = true;

            // Act
            var result = _asyncManager.Initialize();

            // Assert
            Assert.IsTrue(result, "AsyncManager should initialize successfully with valid settings");
            Assert.IsTrue(_asyncManager.IsInitialized, "IsInitialized should be true after successful initialization");
            Assert.AreEqual(4, _asyncManager.MaxThreads, "MaxThreads should match settings");
        }

        [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.UnitTestCategory)]
        public void Initialize_InvalidThreadCount_ThrowsArgumentException()
        {
            // Arrange
            _mockSettings.maxAsyncThreads = -1; // Invalid value

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _asyncManager.Initialize(),
                "Should throw ArgumentException for invalid thread count");
        }

        #endregion

        #region Execution Tests

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.UnitTestCategory)]
        public async Task ExecuteAsync_ValidOperation_CompletesSuccessfully()
        {
            // Arrange
            _asyncManager.Initialize();
            var testOperation = new Func<Task<bool>>(async () =>
            {
                await Task.Delay(100);
                return true;
            });

            // Act
            var result = await _asyncManager.ExecuteAsync(testOperation);

            // Assert
            Assert.IsTrue(result, "Async operation should complete successfully");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.UnitTestCategory)]
        public void CanExecuteAsync_WhenInitialized_ReturnsTrue()
        {
            // Arrange
            _asyncManager.Initialize();

            // Act
            var canExecute = _asyncManager.CanExecuteAsync();

            // Assert
            Assert.IsTrue(canExecute, "Should be able to execute async operations when initialized");
        }

        #endregion

        #region Error Handling Tests

        [Test]
        [Category(TestConfig.CriticalPriority)]
        [Category(TestConfig.UnitTestCategory)]
        public async Task ExecuteAsync_OperationThrows_HandlesGracefully()
        {
            // Arrange
            _asyncManager.Initialize();
            var failingOperation = new Func<Task<bool>>(() => throw new InvalidOperationException("Test exception"));

            // Act & Assert
            Assert.DoesNotThrowAsync(async () =>
            {
                var result = await _asyncManager.ExecuteAsync(failingOperation);
                Assert.IsFalse(result, "Should return false when operation throws");
            }, "Should handle exceptions gracefully without crashing");
        }

        #endregion
    }
}
```

### Integration Test Template
```csharp
using NUnit.Framework;
using System.Threading.Tasks;
using RimAsync.Core;
using RimAsync.Threading;
using RimAsync.Utils;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Integration.Threading
{
    /// <summary>
    /// Integration tests for AsyncManager with other system components
    /// Tests real component interactions and workflows
    /// </summary>
    [TestFixture]
    public class AsyncManagerIntegrationTests
    {
        [SetUp]
        public void SetUp()
        {
            TestHelpers.MockRimWorldEnvironment();
            RimAsyncCore.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            RimAsyncCore.Shutdown();
            TestHelpers.ResetMocks();
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public async Task AsyncManager_WithRimAsyncCore_IntegratesCorrectly()
        {
            // Arrange
            var coreInitialized = RimAsyncCore.IsInitialized;
            Assert.IsTrue(coreInitialized, "RimAsyncCore should be initialized");

            // Act
            var canExecuteAsync = AsyncManager.CanExecuteAsync();
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.IsTrue(canExecuteAsync, "AsyncManager should work with initialized core");
            Assert.IsNotNull(executionMode, "Execution mode should be determined");
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.IntegrationTestCategory)]
        public void AsyncManager_WithMultiplayerCompat_RespectsSafetyMode()
        {
            // Arrange
            MultiplayerCompat.EnableTestMode(true, false); // Multiplayer mode, no AsyncTime

            // Act
            var canExecuteAsync = AsyncManager.CanExecuteAsync();
            var executionMode = RimAsyncCore.GetExecutionMode();

            // Assert
            Assert.IsFalse(canExecuteAsync, "Should not allow async in multiplayer without AsyncTime");
            Assert.AreEqual(ExecutionMode.Synchronous, executionMode, "Should use synchronous mode");
        }
    }
}
```

### Performance Test Template
```csharp
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RimAsync.Threading;
using RimAsync.Tests.Utils;

namespace RimAsync.Tests.Performance.Threading
{
    /// <summary>
    /// Performance tests for AsyncManager
    /// Validates performance requirements and benchmarks
    /// </summary>
    [TestFixture]
    public class AsyncManagerPerformanceTests
    {
        private AsyncManager _asyncManager;

        [SetUp]
        public void SetUp()
        {
            TestHelpers.MockRimWorldEnvironment();
            _asyncManager = new AsyncManager();
            _asyncManager.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            _asyncManager?.Dispose();
            TestHelpers.ResetMocks();
        }

        [Test]
        [Category(TestConfig.HighPriority)]
        [Category(TestConfig.PerformanceTestCategory)]
        [Timeout(TestConfig.PerformanceTimeoutMs)]
        public async Task ExecuteAsync_HighVolume_MeetsPerformanceTarget()
        {
            // Arrange
            const int operationCount = 1000;
            const int maxDurationMs = 5000; // 5 seconds for 1000 operations
            var stopwatch = Stopwatch.StartNew();

            // Act
            var tasks = new Task[operationCount];
            for (int i = 0; i < operationCount; i++)
            {
                tasks[i] = _asyncManager.ExecuteAsync(async () =>
                {
                    await Task.Delay(1); // Simulate work
                    return true;
                });
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.IsTrue(stopwatch.ElapsedMilliseconds < maxDurationMs,
                $"High volume execution should complete within {maxDurationMs}ms. " +
                $"Actual: {stopwatch.ElapsedMilliseconds}ms");

            var averageMs = stopwatch.ElapsedMilliseconds / (double)operationCount;
            Assert.IsTrue(averageMs < 5.0,
                $"Average operation time should be < 5ms. Actual: {averageMs:F2}ms");
        }

        [Test]
        [Category(TestConfig.MediumPriority)]
        [Category(TestConfig.PerformanceTestCategory)]
        public void ThreadUtilization_UnderLoad_StaysWithinLimits()
        {
            // Arrange
            const int maxThreads = 8;
            _asyncManager.MaxThreads = maxThreads;

            // Act
            var initialMemory = GC.GetTotalMemory(true);

            // Simulate high load
            var tasks = new Task[100];
            for (int i = 0; i < 100; i++)
            {
                tasks[i] = _asyncManager.ExecuteAsync(async () =>
                {
                    await Task.Delay(50);
                    return true;
                });
            }

            Task.WaitAll(tasks, 10000); // 10 second timeout

            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            Assert.IsTrue(memoryIncrease < 50 * 1024 * 1024, // 50MB
                $"Memory usage should not increase significantly. Increase: {memoryIncrease / 1024 / 1024}MB");
        }
    }
}
```

## 🔧 Command Options

### Test Type Options
- `--unit` - Generate unit tests only
- `--integration` - Generate integration tests only
- `--performance` - Generate performance tests only
- `--all-types` - Generate all test types (default)

### Coverage Options
- `--basic` - Basic test coverage
- `--comprehensive` - Comprehensive test coverage
- `--critical-only` - Only critical priority tests

### Mock Options
- `--mocking` - Include detailed mocking examples
- `--no-mocks` - Minimal mocking
- `--mock-external` - Mock external dependencies only

### Framework Options
- `--nunit` - Use NUnit framework (default)
- `--include-benchmarks` - Include BenchmarkDotNet tests
- `--async-patterns` - Focus on async/await patterns

## 🎯 Component-Specific Templates

### AsyncManager Tests
- **Threading behavior** validation
- **Cancellation token** handling
- **Error recovery** mechanisms
- **Performance benchmarks**

### SmartCache Tests
- **Cache hit/miss** scenarios
- **Memory management** validation
- **Thread safety** verification
- **Performance optimization** tests

### MultiplayerCompat Tests
- **AsyncTime detection** accuracy
- **Deterministic behavior** validation
- **Fallback mechanisms** testing
- **Synchronization safety** verification

### Settings Tests
- **Configuration validation** logic
- **Persistence mechanisms** testing
- **UI interaction** validation
- **Default value** verification

## 🚀 Usage Examples

### Complete Test Suite Generation
```
@create-tests AsyncManager --all-types --comprehensive

🧪 Generating comprehensive test suite for AsyncManager...

📁 Creating test structure:
├── ✅ Tests/Unit/Threading/AsyncManagerTests.cs
├── ✅ Tests/Unit/Threading/AsyncManagerInitializationTests.cs
├── ✅ Tests/Unit/Threading/AsyncManagerThreadingTests.cs
├── ✅ Tests/Integration/Threading/AsyncManagerIntegrationTests.cs
├── ✅ Tests/Performance/Threading/AsyncManagerPerformanceTests.cs
└── ✅ Tests/Mocks/MockAsyncManager.cs

📊 Generated test coverage:
├── Unit tests: 15 test methods
├── Integration tests: 8 test methods
├── Performance tests: 5 test methods
└── Mock objects: 3 mock classes

🎯 Test categories:
├── Critical priority: 8 tests
├── High priority: 12 tests
├── Medium priority: 8 tests

✅ Complete test suite generated successfully!
```

### Quick Unit Test Generation
```
@create-tests SmartCache --unit --basic

🧪 Generating basic unit tests for SmartCache...

📁 Creating test files:
├── ✅ Tests/Unit/Utils/SmartCacheTests.cs
└── ✅ Tests/Mocks/MockSmartCache.cs

📊 Generated:
├── Unit tests: 8 test methods
├── Mock objects: 1 mock class

✅ Basic unit tests generated!
```

## 📋 Best Practices Included

### Test Organization
- **Clear test naming** conventions
- **Proper categorization** by priority and type
- **Logical grouping** by functionality
- **Comprehensive documentation**

### Test Quality
- **AAA pattern** (Arrange, Act, Assert)
- **Single responsibility** per test method
- **Deterministic results** with proper setup/teardown
- **Meaningful assertions** with descriptive messages

### Performance Testing
- **Realistic benchmarks** with appropriate timeouts
- **Memory usage** validation
- **Concurrency testing** for thread safety
- **Scalability verification** under load

---

**Note:** Generated tests serve as starting templates. Review and customize them based on specific component requirements and business logic.
