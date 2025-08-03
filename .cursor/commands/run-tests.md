# Run Tests Command

**Command:** `@run-tests`
**Description:** Execute comprehensive test suite in Docker environment with support for different test categories

## 🎯 Usage

### Basic Usage
```
@run-tests                    # Run all tests
@run-tests unit              # Run only unit tests
@run-tests integration       # Run only integration tests
@run-tests performance       # Run only performance tests
```

### Advanced Usage
```
@run-tests --verbose         # Run with detailed output
@run-tests --coverage        # Run with code coverage
@run-tests --parallel        # Run tests in parallel
@run-tests --timeout 300     # Set custom timeout (seconds)
```

## 🔧 What the command does

1. **Analyzes test configuration** from TestConfig.cs
2. **Builds project in Docker** using latest code
3. **Executes selected test categories** with proper isolation
4. **Collects test results** and coverage data
5. **Generates test report** with detailed metrics
6. **Identifies failing tests** for debugging

## 🐳 Docker Test Environment

All tests run in isolated Docker containers for consistency:

### 📋 Makefile Commands (Recommended):
- `make test` - run all tests with progress indication
- `make test-unit` - run only unit tests quickly
- `make test-integration` - run integration tests (slower)
- `make test-performance` - run performance benchmarks
- `make test-coverage` - run tests with coverage report

### 🔧 Raw Docker Commands:
- `docker-compose up test` - run all tests
- `docker-compose up test-unit` - unit tests only
- `docker-compose up test-integration` - integration tests only

## 📊 Test Categories

### 🧪 Unit Tests (`unit`)
- **Location:** `Tests/Unit/`
- **Purpose:** Test individual components in isolation
- **Speed:** Fast (< 1 minute)
- **Dependencies:** Minimal mocking

### 🔗 Integration Tests (`integration`)
- **Location:** `Tests/Integration/`
- **Purpose:** Test component interactions
- **Speed:** Medium (1-5 minutes)
- **Dependencies:** Full mock environment

### ⚡ Performance Tests (`performance`)
- **Location:** `Tests/Performance/`
- **Purpose:** Benchmark speed and memory usage
- **Speed:** Slow (5-15 minutes)
- **Dependencies:** Performance monitoring tools

### 🎮 Multiplayer Tests (`multiplayer`)
- **Location:** `Tests/Multiplayer/`
- **Purpose:** Test multiplayer compatibility
- **Speed:** Medium (2-10 minutes)
- **Dependencies:** Multiplayer simulation

## 📈 Test Results Format

### ✅ Success Output
```
🧪 Running RimAsync Test Suite
📁 Category: All Tests (51 total)

[1/51] CompilationTests.ProjectBuilds ✅ (234ms)
[2/51] AsyncManagerTests.Initialize ✅ (89ms)
[3/51] PathfindingTests.BasicAsync ✅ (156ms)
...
[51/51] PerformanceTests.MemoryUsage ✅ (2.1s)

📊 Test Results Summary:
✅ Passed: 51/51 (100%)
❌ Failed: 0/51 (0%)
⏱️ Total Time: 2m 34s
📈 Coverage: 87.3%
```

### ❌ Failure Output
```
🧪 Running RimAsync Test Suite
📁 Category: All Tests (51 total)

[1/51] CompilationTests.ProjectBuilds ✅ (234ms)
[2/51] AsyncManagerTests.Initialize ❌ (Failed)

❌ FAILURE DETAILS:
Test: AsyncManagerTests.Initialize
Error: System.NullReferenceException: RimAsyncMod.Settings is null
File: Tests/Unit/Threading/AsyncManagerTests.cs:45
Stack: AsyncManager.Initialize() line 23

[3/51] PathfindingTests.BasicAsync ⏭️ (Skipped - dependency failed)

📊 Test Results Summary:
✅ Passed: 48/51 (94.1%)
❌ Failed: 2/51 (3.9%)
⏭️ Skipped: 1/51 (2.0%)
⏱️ Total Time: 1m 12s (stopped early)
```

## 🔧 Test Configuration

### Environment Variables
```bash
RIMWORLD_VERSION=1.5       # Target RimWorld version
ENABLE_PERFORMANCE=true    # Enable performance monitoring
TEST_TIMEOUT=300          # Test timeout in seconds
PARALLEL_TESTS=4          # Number of parallel test runners
```

### Custom Test Attributes
```csharp
[Test]
[Category(TestConfig.CriticalPriority)]    // Critical tests (run first)
[Category(TestConfig.PerformanceTestCategory)] // Performance tests
[Timeout(TestConfig.PerformanceTimeoutMs)]     // Custom timeout
public void MyTest_Scenario_ExpectedResult()
{
    // Test implementation
}
```

## 🐛 Debugging Failed Tests

### Common Issues & Solutions

#### 1. **Compilation Errors**
```bash
@run-tests unit              # Run fast unit tests first
make build                   # Check compilation issues
make lint                    # Check code quality
```

#### 2. **Harmony Patch Failures**
```bash
@run-tests --verbose         # Get detailed error output
# Check for .NET Core vs .NET Framework compatibility
```

#### 3. **Performance Test Timeouts**
```bash
@run-tests performance --timeout 600  # Increase timeout
@run-tests --parallel 1      # Disable parallelization
```

#### 4. **Docker Environment Issues**
```bash
make clean                   # Clean Docker containers
make dev                     # Rebuild development environment
docker system prune -f      # Clean Docker cache
```

## 📋 Test Development Guidelines

### Creating New Tests
1. **Follow naming convention:** `ComponentName_Scenario_ExpectedResult`
2. **Use proper categories:** `[Category(TestConfig.UnitTestCategory)]`
3. **Add performance timeouts:** `[Timeout(TestConfig.DefaultTimeoutMs)]`
4. **Include setup/teardown:** `[SetUp]` and `[TearDown]` methods

### Test Structure
```csharp
[TestFixture]
public class ComponentNameTests
{
    private ComponentType _component;

    [SetUp]
    public void SetUp()
    {
        // Initialize test environment
        TestHelpers.MockRimWorldEnvironment();
        _component = new ComponentType();
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up resources
        TestHelpers.ResetMocks();
        _component?.Dispose();
    }

    [Test]
    [Category(TestConfig.UnitTestCategory)]
    public void Component_ValidInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = CreateValidInput();

        // Act
        var result = _component.ProcessInput(input);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(expectedValue, result.Value);
    }
}
```

---

**Note:** Use `make test` for best experience with automatic cleanup and colored output!
