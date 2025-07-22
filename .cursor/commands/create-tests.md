# Create Tests Command

**Команда:** `@create-tests`  
**Описание:** Создать comprehensive тесты для указанного компонента RimAsync (с автоматическим тестированием в Docker)

## 🐳 Docker Integration

**Все созданные тесты автоматически готовы для запуска в Docker контейнере!**

## 🎯 Использование

```
@create-tests [component-name]
```

**Примеры:**
```
@create-tests AsyncManager
@create-tests MultiplayerCompat
@create-tests Pawn_PathFollower_Patch
```

**После создания тестов автоматически запускается:**
```
docker-compose up test  # Проверка работоспособности созданных тестов
```

## 🧪 Что создается

### 1. Unit Tests
- Базовые функциональные тесты
- Edge cases и error handling
- Performance constraints тесты
- Thread safety тесты

### 2. Integration Tests  
- Взаимодействие с другими компонентами
- Harmony patch integration
- RimWorld API compatibility

### 3. Multiplayer Tests
- AsyncTime compatibility
- Sync safety validation
- Desync prevention tests

### 4. Performance Tests
- TPS improvement validation
- Memory usage benchmarks
- Thread utilization metrics

## 📁 Структура создаваемых файлов

```
Tests/
├── Unit/[ComponentCategory]/
│   └── [ComponentName]Tests.cs
├── Integration/
│   └── [ComponentName]IntegrationTests.cs
├── Performance/
│   └── [ComponentName]PerformanceTests.cs
└── Multiplayer/
    └── [ComponentName]MultiplayerTests.cs
```

## 🎯 Test Templates

### Unit Test Template
```csharp
using NUnit.Framework;
using RimAsync.[Namespace];

namespace RimAsync.Tests.Unit.[Category]
{
    [TestFixture]
    public class [ComponentName]Tests
    {
        private [ComponentName] _component;
        
        [SetUp]
        public void SetUp()
        {
            _component = new [ComponentName]();
        }
        
        [TearDown]
        public void TearDown()
        {
            _component?.Dispose();
        }
        
        [Test]
        public void Method_Scenario_ExpectedResult()
        {
            // Arrange
            
            // Act
            
            // Assert
        }
        
        [Test]
        public void Method_EdgeCase_HandledCorrectly()
        {
            // Test edge cases
        }
        
        [Test]
        public void Method_ThreadSafe_NoRaceConditions()
        {
            // Test thread safety
        }
    }
}
```

### Integration Test Template
```csharp
using NUnit.Framework;
using RimAsync.[Namespace];

namespace RimAsync.Tests.Integration
{
    [TestFixture]
    public class [ComponentName]IntegrationTests
    {
        [Test]
        public void Component_IntegratesWithCore_WorksCorrectly()
        {
            // Test integration with RimAsyncCore
        }
        
        [Test]
        public void Component_HarmonyPatch_AppliesCorrectly()
        {
            // Test Harmony patch integration
        }
        
        [Test]
        public void Component_RimWorldAPI_Compatible()
        {
            // Test RimWorld API compatibility
        }
    }
}
```

### Performance Test Template
```csharp
using NUnit.Framework;
using System.Diagnostics;
using RimAsync.[Namespace];

namespace RimAsync.Tests.Performance
{
    [TestFixture]
    public class [ComponentName]PerformanceTests
    {
        [Test]
        public void Component_Performance_MeetsTargets()
        {
            // Benchmark performance
            var stopwatch = Stopwatch.StartNew();
            
            // Execute operation
            
            stopwatch.Stop();
            Assert.Less(stopwatch.ElapsedMilliseconds, targetMs);
        }
        
        [Test]
        public void Component_MemoryUsage_WithinLimits()
        {
            // Test memory usage
        }
        
        [Test]
        public void Component_TPS_Improvement_Validated()
        {
            // Validate TPS improvement
        }
    }
}
```

### Multiplayer Test Template
```csharp
using NUnit.Framework;
using RimAsync.[Namespace];
using RimAsync.Utils;

namespace RimAsync.Tests.Multiplayer
{
    [TestFixture]
    public class [ComponentName]MultiplayerTests
    {
        [Test]
        public void Component_AsyncTime_SafeExecution()
        {
            // Mock AsyncTime enabled
            MultiplayerCompat.SetAsyncTimeEnabled(true);
            
            // Test safe async execution
        }
        
        [Test]
        public void Component_SyncMode_NoAsyncOperations()
        {
            // Mock multiplayer without AsyncTime
            MultiplayerCompat.SetAsyncTimeEnabled(false);
            
            // Verify no async operations occur
        }
        
        [Test]
        public void Component_Deterministic_NoDesync()
        {
            // Test deterministic behavior
        }
    }
}
```

## 🔧 Testing Infrastructure

### Test Utilities
```csharp
// Tests/Utils/TestHelpers.cs
public static class TestHelpers
{
    public static void MockRimWorldEnvironment() { }
    public static void MockMultiplayerState(bool asyncTime) { }
    public static void ValidateNoDesyncs() { }
    public static void MeasurePerformance(Action action) { }
}
```

### Mock Objects
```csharp
// Tests/Mocks/MockAsyncManager.cs
public class MockAsyncManager : IAsyncManager
{
    // Mock implementation for testing
}
```

## 📊 Test Categories

- **🔴 Critical:** Core functionality, multiplayer safety
- **🟠 High:** Performance, integration
- **🟡 Medium:** Edge cases, error handling
- **🟢 Low:** UI, documentation validation

## 🚀 Пример выполнения

```
User: @create-tests AsyncManager

AI: 🧪 Создаю comprehensive тесты для AsyncManager...

📁 Создаю структуру тестов:
- Tests/Unit/Threading/AsyncManagerTests.cs
- Tests/Integration/AsyncManagerIntegrationTests.cs  
- Tests/Performance/AsyncManagerPerformanceTests.cs
- Tests/Multiplayer/AsyncManagerMultiplayerTests.cs

✅ Unit Tests (12 тестов):
- Thread limiting functionality
- Cancellation token handling
- Execution mode switching
- Error handling и fallback

✅ Integration Tests (6 тестов):
- Core system integration
- Harmony patch compatibility
- Settings change handling

✅ Performance Tests (4 теста):
- Thread utilization benchmarks
- Memory usage validation
- TPS improvement measurement

✅ Multiplayer Tests (8 тестов):
- AsyncTime detection
- Sync safety validation
- Deterministic execution

🎉 Создано 30 тестов для AsyncManager!
```

---

**Примечание:** Команда автоматически определяет категорию компонента и создает соответствующие тесты. 