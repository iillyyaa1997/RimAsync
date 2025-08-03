# RimAsync Testing Guide

## 🧪 Comprehensive Testing Commands

RimAsync provides flexible testing commands through the Makefile for different testing scenarios.

## 🎯 Basic Test Commands

### Standard Test Commands
```bash
make test                    # Run all tests
make test-unit              # Run unit tests only
make test-integration       # Run integration tests only
make test-performance       # Run performance tests only
```

## 🔧 Advanced Test Commands

### Custom Filter Tests
```bash
make test-filter FILTER="YourFilter"
```

**Examples:**
```bash
# Run specific test class
make test-filter FILTER="MultiplayerDetectionTests"

# Run tests by category
make test-filter FILTER="Category=Unit"
make test-filter FILTER="Category=Integration"
make test-filter FILTER="Category=Performance"

# Run tests containing specific name
make test-filter FILTER="Name~AsyncManager"
make test-filter FILTER="Name~Initialize"
```

### Specific Test Method
```bash
make test-method METHOD="TestMethodName"
```

**Examples:**
```bash
# Run specific test method
make test-method METHOD="IsInMultiplayer"
make test-method METHOD="Initialize"
make test-method METHOD="DoWindowContents"
```

### Specific Test Class
```bash
make test-class CLASS="TestClassName"
```

**Examples:**
```bash
# Run specific test classes
make test-class CLASS="MultiplayerDetectionTests"
make test-class CLASS="SettingsUITests"
make test-class CLASS="AsyncManagerTests"
```

### Advanced Test Runner
```bash
make test-advanced FILTER="..." [OPTIONS]
```

**Available Options:**
- `FILTER` - Custom filter expression (required)
- `VERBOSITY` - Logging verbosity: `quiet`, `minimal`, `normal`, `detailed`, `diagnostic`
- `RESULTS` - Results directory (default: `./TestResults`)
- `LOGGER` - Logger format (default: `console;verbosity=normal`)

**Examples:**
```bash
# Detailed output for unit tests
make test-advanced FILTER="Category=Unit" VERBOSITY="detailed"

# HTML report generation
make test-advanced FILTER="MultiplayerDetectionTests" LOGGER="html;LogFileName=report.html"

# Custom results directory
make test-advanced FILTER="Category=Integration" RESULTS="./CustomTestResults"

# Combination of options
make test-advanced FILTER="Name~Multiplayer" VERBOSITY="diagnostic" RESULTS="./DetailedResults"
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
make test-class CLASS="AsyncManagerTests"

# 3. Test specific method after changes
make test-method METHOD="Initialize"

# 4. Full integration test before commit
make test-integration
```

### Debugging Workflow
```bash
# 1. Run failing test with detailed output
make test-method METHOD="FailingTest"
# (automatically uses detailed verbosity)

# 2. Run test class with custom verbosity
make test-advanced FILTER="ProblematicTestClass" VERBOSITY="diagnostic"

# 3. Generate HTML report for analysis
make test-advanced FILTER="Category=Integration" LOGGER="html;LogFileName=debug_report.html"
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

## 🚀 Performance Testing

### Quick Performance Check
```bash
make test-performance
```

### Specific Performance Tests
```bash
make test-filter FILTER="Category=Performance&Name~Cache"
make test-advanced FILTER="Performance" VERBOSITY="detailed"
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
make test-advanced FILTER="YourFilter" VERBOSITY="diagnostic"
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
make test-filter FILTER="Category=CriticalPriority"

# Run high priority tests
make test-filter FILTER="Category=HighPriority|Category=CriticalPriority"
```

## 💡 Tips and Best Practices

1. **Use specific filters** during development to save time
2. **Run full test suite** before committing
3. **Use detailed verbosity** for debugging failing tests
4. **Generate HTML reports** for complex test analysis
5. **Test by priority** when time is limited
6. **Use performance tests** to catch regressions early

## 🔄 Quick Reference

| Command | Purpose | Example |
|---------|---------|---------|
| `make test` | All tests | `make test` |
| `make test-filter` | Custom filter | `make test-filter FILTER="Unit"` |
| `make test-method` | Specific method | `make test-method METHOD="Initialize"` |
| `make test-class` | Specific class | `make test-class CLASS="AsyncManager"` |
| `make test-advanced` | Full control | `make test-advanced FILTER="..." VERBOSITY="detailed"` |
