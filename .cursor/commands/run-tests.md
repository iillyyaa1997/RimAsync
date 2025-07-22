# Run Tests Command

**Команда:** `@run-tests`  
**Описание:** Запустить тесты RimAsync в Docker контейнере с различными опциями фильтрации и отчетности

## 🐳 Docker Environment

**ВСЕ тесты выполняются в Docker контейнере для обеспечения изолированной и воспроизводимой среды!**

## 🎯 Использование

```
@run-tests [options]
```

**Примеры:**
```
@run-tests                           # Все тесты в Docker
@run-tests --unit                    # Только unit тесты
@run-tests --integration             # Только integration тесты
@run-tests --performance             # Только performance тесты
@run-tests --multiplayer             # Только multiplayer тесты
@run-tests --component AsyncManager  # Тесты для конкретного компонента
@run-tests --critical                # Только критические тесты
@run-tests --watch                   # Continuous testing mode
```

### 📋 Makefile Commands (Рекомендуемые):
```
make test                # Полный запуск тестов (с цветным выводом)
make test-unit           # Unit тесты
make test-integration    # Integration тесты  
make test-performance    # Performance тесты
make test-coverage       # Тесты с coverage отчетом
make test-report         # HTML отчет тестов
```

### 🔧 Raw Docker Commands (альтернатива):
```
docker-compose up test               # Полный запуск тестов
docker-compose run test dotnet test Tests/Unit/     # Unit тесты
docker-compose run test dotnet test Tests/Integration/ # Integration тесты
```

## 🔧 Опции команды

### По типу тестов:
- `--unit` - Unit tests
- `--integration` - Integration tests  
- `--performance` - Performance benchmarks
- `--multiplayer` - Multiplayer compatibility tests

### По приоритету:
- `--critical` - 🔴 Критические тесты
- `--high` - 🟠 Высокий приоритет
- `--medium` - 🟡 Средний приоритет  
- `--all` - Все приоритеты (по умолчанию)

### По компоненту:
- `--component [name]` - Тесты для конкретного компонента
- `--category [category]` - Тесты для категории (Core, Threading, Utils, etc.)

### Режимы выполнения:
- `--watch` - Continuous testing (пересборка при изменениях)
- `--parallel` - Параллельный запуск тестов
- `--verbose` - Подробный вывод
- `--coverage` - Анализ покрытия кода

## 📊 Отчеты

### Console Output
```
🧪 Running RimAsync Tests...

📁 Test Categories:
├── Unit Tests: 45/45 ✅
├── Integration Tests: 18/20 ⚠️ (2 failed)
├── Performance Tests: 12/12 ✅  
└── Multiplayer Tests: 8/10 ❌ (2 failed)

📊 Summary:
✅ Passed: 83/87 (95.4%)
❌ Failed: 4/87 (4.6%)
⏱️ Duration: 2m 34s
📈 Coverage: 87.3%
```

### Performance Metrics
```
⚡ Performance Benchmarks:

🎯 TPS Improvements:
├── Small Colony: +18.5% (Target: +15%) ✅
├── Medium Colony: +24.2% (Target: +20%) ✅
└── Large Colony: +31.7% (Target: +25%) ✅

💾 Memory Usage:
├── Baseline: 2.1 GB
├── Current: 2.3 GB (+9.5%)
└── Target: <+10% ✅

🧵 Thread Utilization:
├── Max Threads: 6/8
├── Average Load: 65%
└── Efficiency: 92.1%
```

### Multiplayer Results
```
🛡️ Multiplayer Compatibility:

AsyncTime Tests:
├── Detection: ✅ 100% accuracy
├── Safe Execution: ✅ No desyncs
├── Fallback Mode: ✅ Sync guaranteed
└── State Switching: ⚠️ 1 edge case

📁 Desync Logs: /Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs
└── No new desync files created ✅
```

## 🔄 Continuous Testing

### Watch Mode
```
@run-tests --watch --unit

🔄 Watching for changes...
📁 Monitoring: Source/RimAsync/**/*.cs

[12:34:56] File changed: AsyncManager.cs
🧪 Running AsyncManager tests...
✅ All tests passed (2.1s)

[12:35:12] File changed: MultiplayerCompat.cs  
🧪 Running MultiplayerCompat tests...
❌ 2 tests failed (see details below)
```

## 🎯 Test Categories

### Critical Tests (--critical)
```
🔴 Critical Priority Tests:
├── Core initialization ✅
├── Harmony patch application ✅
├── AsyncTime detection ✅
├── Multiplayer sync safety ✅
├── Performance targets ✅
└── Memory leak prevention ✅
```

### Component Tests (--component AsyncManager)
```
🔧 AsyncManager Test Suite:

Unit Tests (12):
├── Thread limiting ✅
├── Cancellation tokens ✅
├── Execution modes ✅
└── Error handling ✅

Integration Tests (6):
├── Core integration ✅
├── Settings application ✅
└── Harmony compatibility ✅

Performance Tests (4):
├── Thread efficiency ✅
├── Memory usage ✅
└── TPS improvement ✅

Multiplayer Tests (8):
├── AsyncTime safety ✅
├── Sync fallback ✅
└── Deterministic execution ✅
```

## 🐛 Failure Analysis

### Failed Test Details
```
❌ Failed Tests:

1. MultiplayerCompat_EdgeCase_HandlesCorrectly
   📍 File: Tests/Unit/Utils/MultiplayerCompatTests.cs:45
   🔍 Assertion: Expected True but was False
   💡 Suggestion: Check AsyncTime detection logic
   
2. AsyncManager_HighLoad_MaintainsPerformance  
   📍 File: Tests/Performance/AsyncManagerPerformanceTests.cs:78
   🔍 Assertion: Expected <100ms but was 145ms
   💡 Suggestion: Optimize thread scheduling
```

### Performance Regressions
```
⚠️ Performance Warnings:

Thread Utilization:
├── Current: 78% (was 85%)
├── Trend: ↓ -7% over last 3 runs
└── Action: Review recent threading changes

Memory Usage:
├── Current: 2.4 GB (was 2.1 GB)  
├── Trend: ↑ +14% over last week
└── Action: Check for memory leaks
```

## 🔧 Test Infrastructure

### Test Discovery
```csharp
// Auto-discovery of test files
Tests/
├── **/*Tests.cs (Unit tests)
├── **/*IntegrationTests.cs (Integration)
├── **/*PerformanceTests.cs (Performance)
└── **/*MultiplayerTests.cs (Multiplayer)
```

### Test Configuration
```csharp
// Tests/TestConfig.cs
public static class TestConfig
{
    public const int DefaultTimeoutMs = 5000;
    public const int PerformanceTimeoutMs = 30000;
    public const string MpDesyncPath = "/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs";
}
```

## 🚀 Пример выполнения

```
User: @run-tests --performance --verbose

AI: 🧪 Запускаю performance тесты RimAsync...

📊 Performance Test Suite:
├── 🔍 Discovering tests... (12 found)
├── 🏗️ Setting up test environment...
├── 📈 Warming up benchmarks...
└── ▶️ Running tests...

[1/12] AsyncManager_ThreadUtilization_Efficient
├── Threads: 6/8 (75% utilization)
├── Duration: 2.3s
└── Result: ✅ PASS

[2/12] PathFinding_LargeMap_Performance  
├── Map Size: 250x250
├── Path Length: 847 cells
├── Duration: 89ms (Target: <100ms)
└── Result: ✅ PASS (+11ms margin)

[3/12] SmartCache_HighVolume_Performance
├── Cache Size: 10,000 entries
├── Hit Rate: 94.2%
├── Avg Access: 0.03ms
└── Result: ✅ PASS

📊 Performance Summary:
✅ All 12 tests passed
⏱️ Total Duration: 4m 12s
🎯 All targets met or exceeded
📈 Average improvement: +22.7% TPS

🎉 Performance tests completed successfully!
```

---

**Примечание:** Команда автоматически создает детальные отчеты и сохраняет результаты для отслеживания регрессий. 