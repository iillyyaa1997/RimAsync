# RimAsync Testing Guide

## 🧪 Comprehensive Testing Commands

RimAsync provides flexible testing commands through the Makefile for different testing scenarios.

## 🎯 Basic Test Commands

### ⚡ Super Quick Commands (Recommended)
```bash
make t                       # Fastest - unit tests only (minimal output)
make t-coverage             # Fastest - unit tests with coverage (one command!)
make test-quick             # Interactive menu (4 most common options)
make test-quick-coverage    # Interactive menu with coverage options
make test-run               # Universal interactive runner
make test-run-coverage      # Universal runner with built-in coverage
```

### 📊 Built-in Coverage Commands (NEW!)

**One command = Tests + Coverage automatically!**

```bash
make test-unit-coverage              # Unit tests with coverage
make test-integration-coverage       # Integration tests with coverage
make test-performance-coverage       # Performance tests with coverage
make test-run-coverage TARGET="..."  # Universal runner with coverage
make test-find-coverage NAME="..."   # Smart search with coverage
make t-coverage                      # Super quick with coverage
```

**Benefits:**
- 🚀 **One command** instead of chaining with `&&`
- 📊 **Automatic coverage** - no need to remember separate commands
- 🎯 **Organized results** - coverage reports go to specific folders
- ⚡ **Same speed** - built-in integration, no overhead

### 🔍 Smart Search Commands
```bash
make test-find NAME="SearchTerm"    # Find tests by name
make test-run TARGET="..."          # Run with specific target
```

### Standard Test Commands
```bash
make test                    # Run all tests
make test-unit              # Run unit tests only
make test-integration       # Run integration tests only
make test-performance       # Run performance tests only
```

## 🔧 Universal Test Commands

### Universal Test Runner
```bash
make test-run [TARGET="..."] [OPTS="..."]
```

**Interactive Mode (no parameters):**
- Provides menu with 9 test options
- User-friendly prompts for all scenarios
- Automatic parameter handling

**Direct Mode (with parameters):**
```bash
# Run specific test class
make test-run TARGET="MultiplayerDetectionTests"

# Run tests by category
make test-run TARGET="Category=Unit"
make test-run TARGET="Category=Integration"
make test-run TARGET="Category=Performance"

# Run tests containing specific name
make test-run TARGET="Name~AsyncManager"
make test-run TARGET="Name~Initialize"

# With custom options
make test-run TARGET="Category=Unit" OPTS="--logger html;LogFileName=report.html"
make test-run TARGET="MultiplayerDetectionTests" OPTS="--logger console;verbosity=detailed"
```

## 📋 Filter Syntax Reference

### Common Filter Patterns

#### By Test Name
```bash
FILTER="Name~PartialName"     # Contains partial name
FILTER="TestMethodName"       # Exact method name
```

#### By Category
```bash
FILTER="Category=Unit"        # Unit tests
FILTER="Category=Integration" # Integration tests
FILTER="Category=Performance" # Performance tests
```

#### By Class Name
```bash
FILTER="MultiplayerDetectionTests"  # Specific class
FILTER="Name~AsyncManager"          # Classes containing "AsyncManager"
```

#### Complex Filters
```bash
FILTER="Category=Unit&Name~Multiplayer"     # Unit tests containing "Multiplayer"
FILTER="Category=Integration|Category=Unit"  # Integration OR Unit tests
```

## 🎮 Practical Testing Scenarios

### Development Workflow
```bash
# 1. Quick unit test check during development
make test-unit

# 2. Test specific functionality you're working on
make test-run TARGET="AsyncManagerTests"

# 3. Test specific method after changes
make test-run TARGET="Name~Initialize"

# 4. Full integration test before commit
make test-integration
```

### Debugging Workflow
```bash
# 1. Run failing test with detailed output
make test-run TARGET="Name~FailingTest" OPTS="--logger console;verbosity=detailed"

# 2. Run test class with diagnostic verbosity
make test-run TARGET="ProblematicTestClass" OPTS="--logger console;verbosity=diagnostic"

# 3. Generate HTML report for analysis
make test-run TARGET="Category=Integration" OPTS="--logger html;LogFileName=debug_report.html"
```

### CI/CD Workflow
```bash
# 1. Run all tests with normal output
make test

# 2. Generate coverage report (if needed)
make test-coverage

# 3. Generate test report
make test-report
```

## 🔍 Test Results Location

### Default Locations
- Console output: Terminal
- Coverage reports: `./TestResults/Coverage/`
- Test reports: `./TestResults/Reports/`
- Custom results: Specified by `RESULTS` parameter

### HTML Reports
When using HTML logger:
```bash
make test-advanced FILTER="..." LOGGER="html;LogFileName=MyReport.html"
```
Report will be saved to: `./TestResults/MyReport.html`

## 📊 Code Coverage Testing

### Quick Coverage Commands
```bash
make coverage-quick         # Unit tests coverage (fastest)
make coverage              # All tests coverage
make coverage-html         # HTML coverage report
```

### Coverage Results Location
- **XML Reports**: `./TestResults/Coverage/`
- **HTML Reports**: `./TestResults/Coverage/Html/index.html`
- **Quick Reports**: `./TestResults/Coverage/Quick/`

### Coverage Workflow
```bash
# 1. Quick coverage check during development
make coverage-quick

# 2. Full coverage analysis
make coverage

# 3. Generate HTML report for detailed analysis
make coverage-html
```

## 🚀 Performance Testing

### Quick Performance Check
```bash
make test-performance
```

### Specific Performance Tests
```bash
make test-run TARGET="Category=Performance&Name~Cache"
make test-run TARGET="Performance" OPTS="--logger console;verbosity=detailed"
```

## 🛠️ Troubleshooting

### No Tests Found
If you get "No test matches the given filter":
1. Check your filter syntax
2. Verify test class/method names
3. Use `make test` to see all available tests
4. Try broader filters like `Name~PartialName`

### Docker Issues
```bash
# Clean Docker environment if tests behave unexpectedly
make clean
make build
make test
```

### Verbose Output
For debugging test issues:
```bash
make test-run TARGET="YourFilter" OPTS="--logger console;verbosity=diagnostic"
```

## 📊 Test Categories

### Available Categories
- `Unit` - Fast, isolated unit tests
- `Integration` - Component integration tests
- `Performance` - Performance and benchmarking tests
- `CriticalPriority` - Critical functionality tests
- `HighPriority` - High priority tests
- `MediumPriority` - Medium priority tests
- `LowPriority` - Low priority tests

### Priority-Based Testing
```bash
# Run only critical tests for quick feedback
make test-run TARGET="Category=CriticalPriority"

# Run high priority tests
make test-run TARGET="Category=HighPriority|Category=CriticalPriority"
```

## 💡 Tips and Best Practices

1. **Use specific filters** during development to save time
2. **Run full test suite** before committing
3. **Use detailed verbosity** for debugging failing tests
4. **Generate HTML reports** for complex test analysis
5. **Test by priority** when time is limited
6. **Use performance tests** to catch regressions early

## 🎯 Recommended Workflow

### For Daily Development
```bash
# 1. Quick check during coding
make t                               # Just tests
# OR
make t-coverage                      # Tests + coverage in one command

# 2. Test specific functionality with coverage
make test-find-coverage NAME="Multiplayer"   # Smart search + coverage
# OR
make test-run-coverage TARGET="AsyncManager" # Specific target + coverage

# 3. Interactive selection with coverage
make test-quick-coverage             # Menu with coverage options
```

### For Debugging
```bash
# 1. Smart search for failing tests
make test-find NAME="FailingTest"

# 2. Run with specific target and options
make test-run TARGET="ProblematicClass" OPTS="--logger html;LogFileName=debug.html"
```

## 🔄 Quick Reference

| Command | Purpose | Example | Speed |
|---------|---------|---------|--------|
| `make t` | Super quick unit tests | `make t` | ⚡⚡⚡ |
| `make t-coverage` | Super quick + coverage | `make t-coverage` | ⚡⚡⚡ |
| `make test-quick` | Interactive menu | `make test-quick` | ⚡⚡ |
| `make test-quick-coverage` | Interactive menu + coverage | `make test-quick-coverage` | ⚡⚡ |
| `make test-find` | Smart search | `make test-find NAME="Multiplayer"` | ⚡⚡ |
| `make test-find-coverage` | Smart search + coverage | `make test-find-coverage NAME="Settings"` | ⚡⚡ |
| `make test-run` | Universal runner | `make test-run TARGET="Category=Unit"` | ⚡⚡ |
| `make test-run-coverage` | Universal runner + coverage | `make test-run-coverage TARGET="AsyncManager"` | ⚡⚡ |
| `make test-unit-coverage` | Unit tests + coverage | `make test-unit-coverage` | ⚡⚡ |
| `make coverage-quick` | Quick coverage analysis | `make coverage-quick` | ⚡⚡ |
| `make coverage` | Full coverage analysis | `make coverage` | ⚡ |
| `make coverage-html` | HTML coverage report | `make coverage-html` | 🐌 |
| `make test` | All tests | `make test` | ⚡ |
