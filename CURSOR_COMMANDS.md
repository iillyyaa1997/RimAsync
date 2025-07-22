# 🚀 RimAsync - Команды для Cursor IDE

**Версия:** 1.0
**Дата:** 19 июля 2025

## 🎯 Как использовать команды

### 📱 В чате Cursor IDE напишите одну из команд:

```
@execute-task                 # Выполнить следующую приоритетную задачу
@create-branch                # Создать git ветку для специальных случаев (НЕ для обычных задач)
@create-tests AsyncManager    # Создать тесты для компонента
@run-tests --performance      # Запустить performance тесты
@analyze-logs --desyncs       # Анализ логов ошибок и desyncs
```

## 🔥 Основные команды

### 1. `@execute-task` - Автоматическое выполнение задач
**Самая важная команда для разработки**

```
@execute-task                    # Автоматический выбор задачи
@execute-task "Компиляция"       # Конкретная задача
```

**Что делает:**
- 🎯 Анализирует `Planning/Development_Plan.md`
- 🔴 Выбирает задачу с наивысшим приоритетом
- 🧪 Создает comprehensive тесты
- ⚡ Реализует функциональность
- ✅ Обновляет статус в плане

### 2. `@create-tests` - Создание тестов
**Для новых компонентов**

```
@create-tests AsyncManager      # Тесты для AsyncManager
@create-tests SmartCache        # Тесты для SmartCache
@create-tests MultiplayerCompat # Тесты для multiplayer
```

**Создает:**
- 🔵 Unit tests (функциональность)
- 🟡 Integration tests (системная интеграция)
- 🔴 Performance tests (benchmarks)
- 🛡️ Multiplayer tests (AsyncTime compatibility)

### 3. `@run-tests` - Запуск тестов
**Гибкое тестирование**

```
@run-tests                      # Все тесты
@run-tests --unit               # Только unit тесты
@run-tests --performance        # Performance benchmarks
@run-tests --multiplayer        # Multiplayer тесты
@run-tests --critical           # Критические тесты
@run-tests --watch              # Continuous testing
@run-tests --component AsyncManager  # Тесты конкретного компонента
```

### 4. `@analyze-logs` - Анализ логов ошибок
**Диагностика проблем и ошибок**

```
@analyze-logs                   # Анализ всех доступных логов
@analyze-logs --docker          # Только Docker логи
@analyze-logs --desyncs         # Multiplayer desync логи
@analyze-logs --rimworld        # RimWorld Player.log
@analyze-logs --emergency       # Быстрый анализ критических ошибок
@analyze-logs --recent          # Только последние 24 часа
```

**Анализирует:**
- 🔴 **RimWorld Multiplayer desyncs** (`/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs`)
- 🐳 **Docker container логи** (build, test, dev)
- 🎮 **RimWorld Player.log** (основные ошибки игры)
- 📊 **RimAsync test results** (./TestResults/)

**Output:**
- 🚨 Критические ошибки с solutions
- ⚠️ Performance warnings
- 📈 Trend analysis и статистика
- 🎯 Приоритизированные recommended actions

### 5. `@create-branch` - Создание git веток (специальные случаи)
**⚠️ НЕ для обычной разработки! Используйте @execute-task для задач.**

```
@create-branch experiment/new-architecture  # Экспериментальная ветка
@create-branch hotfix/critical-bug          # Срочное исправление
@create-branch docs/api-documentation       # Документация
@create-branch refactor/project-structure   # Рефакторинг
```

**Когда использовать:**
- 🧪 **Experiments**: тестирование новых идей
- 🔧 **Hotfix**: критические исправления
- 📚 **Documentation**: работа только с docs
- 🏗️ **Refactoring**: крупные структурные изменения
- 🎨 **Configuration**: изменения настроек

**Git Workflow:**
- 🔄 **Pre-checks**: git статус, uncommitted changes
- 🌿 **Manual creation**: checkout master → pull → create branch
- 📏 **Manual naming**: полностью ручное указание имени

**Основной workflow:**
- **Для задач разработки** используйте `@execute-task` - автоматически создает ветки
- **Для специальных случаев** используйте `@create-branch` - ручное создание

## 📋 Типичный workflow

### 🚀 Feature Development Workflow:
```
1. @execute-task (ОСНОВНАЯ КОМАНДА - все в одном)
   └── Автоматически создает git ветку от последнего master
   └── Выбирает приоритетную задачу из плана
   └── Форматирует код (make format-fix)
   └── Создает comprehensive тесты
   └── Реализует функциональность
   └── Проверяет качество кода (make lint)
   └── Компилирует и тестирует в Docker
   └── Коммитит с conventional commits
   └── Обновляет статус: ⏳ → 🔄 → ✅

3. @run-tests --watch
   └── Continuous testing во время разработки
   └── Мониторинг изменений файлов
   └── Автоматический перезапуск тестов

4. @analyze-logs --auto
   └── Автоматический анализ логов после сборки
   └── Выявление проблем в реальном времени
   └── Диагностика Docker и RimWorld ошибок
```

### 🚨 Emergency Workflow (при критических ошибках):
```
1. @analyze-logs --emergency --last-10-minutes
   └── Быстрый анализ последних ошибок
   └── Идентификация критических проблем

2. @analyze-logs --desyncs --recent
   └── Специализированный анализ Multiplayer desyncs
   └── Проверка AsyncTime compatibility

3. @execute-task (fix identified issues)
   └── Исправление выявленных проблем
   └── Приоритизация критических ошибок
```

### 🔄 Iterative development:
```
4. @create-tests NewComponent
   └── Создание тестов для новых компонентов

5. @run-tests --component NewComponent
   └── Тестирование конкретного компонента

6. @execute-task "specific task"
   └── Выполнение конкретной задачи
```

## 🎯 Приоритеты задач

### Команды выбирают задачи по приоритету:

1. **🔴 КРИТИЧЕСКИЙ** - Компиляция, AsyncTime, базовый функционал
2. **🟠 ВЫСОКИЙ** - Performance, совместимость с модами
3. **🟡 СРЕДНИЙ** - UI, дополнительные функции

## 🧪 Testing Strategy

### Каждая команда включает тестирование:

```
🧪 Test Pyramid:
├── 🔵 Unit Tests (60%) - Core functionality, edge cases
├── 🟡 Integration Tests (25%) - System integration
├── 🔴 Performance Tests (10%) - TPS benchmarks
└── 🛡️ Multiplayer Tests (5%) - AsyncTime compatibility
```

### 📊 Quality Gates:
- ✅ **90%+ test coverage** обязательно
- ✅ **Все критические тесты** проходят
- ✅ **Performance targets** достигнуты
- ✅ **0 multiplayer desyncs** в AsyncTime режиме

## 📁 Создаваемая структура

### Tests/ - автоматически создается:
```
Tests/
├── Unit/
│   ├── Core/           # Основные системы
│   ├── Threading/      # Async компоненты
│   ├── Utils/          # Утилиты
│   └── Patches/        # Harmony патчи
├── Integration/        # Интеграционные тесты
├── Performance/        # Performance benchmarks
├── Multiplayer/        # Multiplayer совместимость
├── Utils/              # Test helpers
├── Mocks/              # Mock objects
└── TestConfig.cs       # Конфигурация тестов
```

## 🔍 Важные детали

### 📁 Multiplayer desyncs логи:
```
/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs
```
*Команды автоматически проверяют этот путь*

### 🎯 Performance targets:
- **Small Colony:** +15% TPS minimum
- **Medium Colony:** +20% TPS minimum
- **Large Colony:** +25% TPS minimum
- **Memory Usage:** <+10% increase

### 🛡️ Multiplayer compatibility:
- **AsyncTime detection** через reflection
- **Sync safety validation**
- **Automatic fallback** к sync режиму

## 🚀 Примеры выполнения

### Начало разработки:
```
User: @execute-task

AI: 🎯 Анализирую текущие задачи...
📋 Выбрана задача: "Компиляция проекта - исправление ошибок"
🔴 Приоритет: КРИТИЧЕСКИЙ

🧪 Создаю тесты:
- Tests/Unit/Core/CompilationTests.cs
- Tests/Integration/ProjectStructureTests.cs

🔧 Исправляю ошибки компиляции:
- Добавляю недостающие using statements
- Исправляю namespace conflicts

✅ Тесты пройдены: 8/8
✅ Компиляция успешна
✅ Статус обновлен: ⏳ → ✅

🎉 Задача завершена! Следующая: "AsyncTime integration testing"
```

### Создание тестов:
```
User: @create-tests AsyncManager

AI: 🧪 Создаю comprehensive тесты для AsyncManager...

📁 Создаю структуру тестов:
- Tests/Unit/Threading/AsyncManagerTests.cs (12 тестов)
- Tests/Integration/AsyncManagerIntegrationTests.cs (6 тестов)
- Tests/Performance/AsyncManagerPerformanceTests.cs (4 теста)
- Tests/Multiplayer/AsyncManagerMultiplayerTests.cs (8 тестов)

✅ Создано 30 тестов для AsyncManager!
🎯 Покрытие: Unit/Integration/Performance/Multiplayer
```

### Запуск тестов:
```
User: @run-tests --performance --verbose

AI: 🧪 Запускаю performance тесты RimAsync...

📊 Performance Test Suite:
[1/12] AsyncManager_ThreadUtilization ✅ (2.3s)
[2/12] PathFinding_LargeMap ✅ (89ms - Target: <100ms)
[3/12] SmartCache_HighVolume ✅ (Hit rate: 94.2%)

📊 Summary:
✅ All 12 tests passed
⏱️ Duration: 4m 12s
🎯 Average improvement: +22.7% TPS
```

## ⚡ Quick Start

### 1. Откройте проект в Cursor IDE
### 2. В чате напишите: `@execute-task`
### 3. Следуйте инструкциям AI
### 4. Команды автоматически:
   - Выберут задачу
   - Создадут тесты
   - Реализуют функциональность
   - Обновят планы

---

**🎯 Начните с `@execute-task` для запуска разработки RimAsync!**

*Команды полностью интегрированы с планами разработки и testing strategy* 🚀
