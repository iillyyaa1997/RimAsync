# RimAsync Testing Guide

**Last Updated:** November 2, 2025
**Test Coverage:** 255 tests (100% passing ✅)

## Overview

RimAsync has comprehensive test coverage across multiple categories:
- **Unit Tests** (~180 tests) - Core functionality
- **Integration Tests** (~30 tests) - Component interactions
- **Performance Tests** (~15 tests) - Benchmarks and regression
- **Compatibility Tests** (~20 tests) - Mod compatibility
- **Stress Tests** (~9 tests) - Memory leaks and stability

## Quick Start

### Running All Tests

```bash
# Using Makefile (recommended)
make test

# Using Docker directly
docker-compose up test

# Quick unit tests only
make t
```

### Running Specific Test Categories

```bash
# Unit tests only
make test-run TARGET="Category=Unit"

# Integration tests
make test-run TARGET="Category=Integration"

# Performance tests
make test-run TARGET="Category=Performance"

# Compatibility tests
make test-run TARGET="Category=Compatibility"

# Stress tests
make test-run TARGET="Category=Stress"
```

### Running Specific Test Classes

```bash
# Run specific test class
make test-run TARGET="SmartCacheTests"

# Run specific test method
make test-run TARGET="FullyQualifiedName~SmartCache_GetOrCompute_ReturnsCachedValue"
```

## Test Categories

### 1. Unit Tests (`Tests/Unit/`)

**Purpose:** Test individual components in isolation

**Test Classes:**
- `CompilationTests.cs` - Project compilation verification
- `AsyncPatchTests.cs` - Async patch functionality
- `SmartCacheTests.cs` - Cache behavior and performance
- `ThreadLimitCalculatorTests.cs` - Thread limit calculations
- `AsyncSafeCollectionsTests.cs` - Thread-safe collections

**Example:**
```csharp
[Test]
[Category("Unit")]
public void SmartCache_GetOrCompute_ReturnsCachedValue()
{
    // Arrange
    const string key = "test_key";
    int computeCount = 0;

    // Act
    var result1 = SmartCache.GetOrCompute(key, () => {
        computeCount++;
        return 42;
    });
    var result2 = SmartCache.GetOrCompute(key, () => {
        computeCount++;
        return 100;
    });

    // Assert
    Assert.AreEqual(42, result1);
    Assert.AreEqual(42, result2); // Should return cached value
    Assert.AreEqual(1, computeCount); // Compute called once
}
```

### 2. Integration Tests (`Tests/Integration/`)

**Purpose:** Test component interactions and system integration

**Test Classes:**
- `AsyncTimeIntegrationTests.cs` - AsyncTime detection and integration
- `RimWorldInitializationTests.cs` - Mod initialization
- `RimWorldLoadingTests.cs` - Game loading scenarios
- `ProjectStructureTests.cs` - Project structure validation

**Example:**
```csharp
[Test]
[Category("Integration")]
public void AsyncTime_Detection_WorksCorrectly()
{
    // Arrange
    MultiplayerCompat.EnableTestMode(
        isInMultiplayer: true,
        asyncTimeEnabled: true
    );

    // Act
    var mode = RimAsyncCore.GetExecutionMode();

    // Assert
    Assert.AreEqual(ExecutionMode.AsyncWithTime, mode);

    // Cleanup
    MultiplayerCompat.DisableTestMode();
}
```

### 3. Performance Tests (`Tests/Performance/`)

**Purpose:** Benchmark performance and detect regressions

**Test Classes:**
- `PerformanceMonitorTests.cs` - TPS monitoring and metrics
- `AsyncPatchBenchmarkTests.cs` - Async operation benchmarks

**Key Metrics:**
- **Pathfinding:** < 10ms per operation
- **Cache operations:** < 0.001ms per operation
- **TPS monitoring:** < 1ms overhead
- **Memory per pawn:** < 100KB

**Example:**
```csharp
[Test]
[Category("Performance")]
public void Pathfinding_Benchmark_MeetsTargets()
{
    // Arrange
    const int iterations = 1000;
    var stopwatch = Stopwatch.StartNew();

    // Act
    for (int i = 0; i < iterations; i++)
    {
        // Simulate pathfinding
    }
    stopwatch.Stop();

    var averageMs = stopwatch.ElapsedMilliseconds / (double)iterations;

    // Assert
    Assert.Less(averageMs, 10.0,
        $"Pathfinding too slow: {averageMs}ms");
}
```

### 4. Compatibility Tests (`Tests/Compatibility/`)

**Purpose:** Verify mod compatibility and conflict detection

**Test Classes:**
- `ModCompatibilityTests.cs` - Mod detection and reports

**Tested Mods:**
- Combat Extended
- Vanilla Expanded (all variants)
- HugsLib
- Multiplayer
- Performance mods (Dubs, RocketMan)

**Example:**
```csharp
[Test]
[Category("Compatibility")]
public void CombatExtended_IsCompatible()
{
    // Arrange
    var modList = new List<string> { "CombatExtended" };

    // Act
    var result = ModCompatibility.CheckModList(modList);

    // Assert
    Assert.IsTrue(result.IsCompatible);
}
```

### 5. Stress Tests (`Tests/Stress/`)

**Purpose:** Test stability under load and detect memory leaks

**Test Classes:**
- `StressTests.cs` - Memory leaks, large colonies, long-running

**Test Categories:**
- **Memory Leak Tests** (3 tests)
  - 10,000 async operations
  - 10,000 cache operations
  - 1,000 pathfinding operations
- **Large Colony Tests** (3 tests)
  - 100 pawns performance
  - Concurrent operations
  - Memory usage per pawn
- **Long-Running Tests** (2 tests)
  - 10-hour simulation (marked [Explicit])
  - 30-second continuous operations
- **Regression Tests** (2 tests)
  - Performance baselines

**Example:**
```csharp
[Test]
[Category("Stress")]
public void MemoryLeak_AsyncOperations_NoLeaks()
{
    // Arrange
    var startMemory = GC.GetTotalMemory(true);

    // Act - Run 10,000 async operations
    for (int i = 0; i < 10000; i++)
    {
        var task = Task.Run(() => i * 2);
        task.Wait();
    }

    GC.Collect();
    var endMemory = GC.GetTotalMemory(true);
    var growth = endMemory - startMemory;

    // Assert - Memory growth < 10MB
    Assert.Less(growth, 10 * 1024 * 1024);
}
```

## UI Tests (`Tests/UI/`)

**Purpose:** Test UI components and overlays

**Test Classes:**
- `DebugOverlayTests.cs` - Debug overlay functionality (23 tests)
- `SettingsUITests.cs` - Settings UI components

**Coverage:**
- F11 toggle functionality
- Stats display formatting
- Quick status generation
- Memory leak prevention

## Mock Objects (`Tests/Mocks/`)

**Purpose:** Simulate RimWorld environment for testing

**Mock Classes:**
- `MockPawn` - Simulated pawn
- `MockMap` - Simulated map
- `MockPathFollower` - Pathfinding simulation
- `MockRimAsyncSettings` - Settings simulation
- `MockMultiplayerCompat` - Multiplayer simulation

## Test Helpers (`Tests/Utils/`)

**TestHelpers.cs** provides:
- `AssertThreadSafety()` - Concurrent execution testing
- `MeasurePerformance()` - Performance benchmarking
- `CreateMockPawn()` - Mock object creation

## Code Coverage

### Running Coverage Tests

```bash
# Quick unit test coverage
make coverage-quick

# Full coverage report
make coverage

# HTML coverage report
make coverage-html
```

### Coverage Requirements

- **New functionality:** Minimum 85% coverage
- **Critical systems:** Minimum 90% coverage
- **Bug fixes:** Must include reproducing tests
- **Performance improvements:** Must include benchmarks

### Current Coverage

**Overall:** ~85% line coverage

**By Component:**
- Core systems: 90%+ ✅
- Async patches: 85%+ ✅
- Utils: 90%+ ✅
- UI components: 80%+ ✅

## Writing Tests

### Test Structure (AAA Pattern)

```csharp
[Test]
[Category("YourCategory")]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Setup test data and mocks
    var input = CreateTestData();

    // Act - Execute the code under test
    var result = MethodUnderTest(input);

    // Assert - Verify results
    Assert.AreEqual(expectedValue, result);
}
```

### Best Practices

1. **One Assertion Per Test** (when possible)
   - Tests should be focused and specific
   - Makes failures easier to diagnose

2. **Use Descriptive Names**
   ```csharp
   // Good
   SmartCache_GetOrCompute_ReturnsCachedValue()

   // Bad
   Test1()
   ```

3. **Clean Up Resources**
   ```csharp
   [TearDown]
   public void TearDown()
   {
       AsyncManager.Shutdown();
       SmartCache.ClearAll();
   }
   ```

4. **Use TestContext for Debugging**
   ```csharp
   TestContext.WriteLine($"Performance: {elapsed}ms");
   ```

5. **Mark Long-Running Tests**
   ```csharp
   [Test]
   [Explicit("Long running test - run manually")]
   public void LongRunningTest() { }
   ```

## Continuous Integration

### Pre-Commit Checklist

- [ ] All tests pass: `make test`
- [ ] Code formatted: `make format-fix`
- [ ] No linter errors: `make lint`
- [ ] Coverage maintained: `make coverage-quick`

### Automated Testing

Tests run automatically in Docker containers:
- Isolated environment
- Consistent results
- No local RimWorld installation needed

## Troubleshooting

### Common Issues

**1. Tests timing out**
```bash
# Increase timeout in TestConfig.cs
public const int PerformanceTimeoutMs = 10000;
```

**2. Flaky tests**
```bash
# Run multiple times to verify
for i in {1..5}; do make test; done
```

**3. Memory issues**
```bash
# Run with more memory
docker-compose run --rm -m 4g test
```

**4. Specific test failing**
```bash
# Run just that test for debugging
make test-run TARGET="YourFailingTest"
```

### Debug Logging

Enable debug logging in tests:
```csharp
[SetUp]
public void SetUp()
{
    RimAsyncSettings.enableDebugMode = true;
    RimAsyncLogger.SetMinLevel(LogLevel.Debug);
}
```

## Performance Baselines

### Reference Performance (MacBook Pro M1)

- **Pathfinding:** 0.02ms avg
- **Cache lookup:** 0.0003ms avg
- **TPS recording:** 0.0001ms per tick
- **100 pawns tick:** < 500ms
- **Memory per pawn:** < 100KB

### Regression Detection

Performance tests fail if:
- >2x slower than baseline
- >50% memory increase
- TPS < 30 in stress tests

## Test Data

### Mock Data Location

Test data stored in:
- `Tests/Data/` - Test configurations
- `Tests/Mocks/` - Mock objects
- Memory - No persistent test data

### Creating Test Data

```csharp
// Simple mock
var pawn = new MockPawn
{
    Dead = false,
    Spawned = true
};

// Complex scenario
var colony = CreateTestColony(size: 100);
```

## Contributing Tests

### When to Add Tests

- **Always:** New features
- **Always:** Bug fixes
- **Recommended:** Performance improvements
- **Optional:** Documentation changes

### Test Review Checklist

- [ ] Tests follow AAA pattern
- [ ] Descriptive test names
- [ ] Proper categories assigned
- [ ] No hard-coded paths
- [ ] Cleanup in TearDown
- [ ] Performance expectations realistic

## Resources

- **NUnit Documentation:** https://docs.nunit.org/
- **Moq Documentation:** https://github.com/moq/moq4
- **Testing Best Practices:** `TESTING_GUIDE.md`
- **Docker Testing:** `DOCKER_USAGE.md`

---

**Questions?** Check the main README.md or open an issue on GitHub.
