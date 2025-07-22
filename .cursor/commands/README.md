# 🚀 RimAsync Commands для Cursor IDE

**Версия:** 1.0
**Дата создания:** 19 июля 2025

## 🎯 Обзор системы команд

Эта папка содержит **специализированные команды** для Cursor IDE, которые позволяют эффективно управлять разработкой RimAsync через чат с LLM. Все команды интегрированы с планами разработки и автоматизируют routine задачи.

## 📚 Доступные команды

### 🔥 Основные команды

#### 📈 [`@execute-task`](./execute-task.md) - Выполнение задач
**Основная команда для разработки** (автоматически создает git ветки)

**Использование:**
```
@execute-task                    # Автоматический выбор приоритетной задачи
@execute-task [task-name]        # Выполнение конкретной задачи
```

**Что делает:**
- 🎯 Анализирует Planning/Development_Plan.md
- 🌿 **Автоматически создает git ветку от master**
- 🔴 Выбирает задачу с наивысшим приоритетом
- 🎨 Форматирует код (make format-fix)
- 🧪 Создает comprehensive тесты
- 🔧 Реализует функциональность
- 🔍 Проверяет качество кода (make lint)
- ✅ Обновляет статус в плане

---

#### 🧪 [`@create-tests`](./create-tests.md) - Создание тестов
**Comprehensive test creation**

**Использование:**
```
@create-tests [component-name]   # Тесты для компонента
```

**Что создает:**
- 🎯 Unit tests для всех public методов
- 🔄 Integration tests для async компонентов
- ⚡ Performance benchmarks
- 🌐 Multiplayer compatibility тесты
- 📊 Coverage reports

**Структура тестов:**
```
Tests/
├── Unit/              # Изолированные unit тесты
├── Integration/       # Тесты взаимодействий
├── Performance/       # Benchmarks
└── Multiplayer/       # Multiplayer tests
```

---

#### 🧪 [`@run-tests`](./run-tests.md) - Запуск тестов
**Docker-based comprehensive testing**

**Использование:**
```
@run-tests [options]             # Запуск тестов в Docker
```

**Опции фильтрации:**
- `--unit`, `--integration`, `--performance`, `--multiplayer`
- `--critical`, `--high`, `--medium` (приоритет)
- `--component [name]` (тесты для компонента)
- `--watch` (continuous testing)
- `--export-json`, `--export-html`

---

#### 🔍 [`@analyze-logs`](./analyze-logs.md) - Анализ логов ошибок
**Comprehensive log analysis and diagnostics**

**Использование:**
```
@analyze-logs [options]         # Анализ всех доступных логов
```

**Анализирует:**
- 🔴 RimWorld Multiplayer desyncs (`/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs`)
- 🐳 Docker контейнер логи
- 🎮 RimWorld Player.log
- 📊 RimAsync тест результаты

**Опции:**
- `--docker`, `--rimworld`, `--desyncs`, `--compilation`
- `--recent`, `--errors-only`, `--emergency`
- `--export-json`, `--export-html`

---

#### 🌿 [`@create-branch`](./create-branch.md) - Создание git веток (специальные случаи)
**⚠️ НЕ для обычной разработки! Используйте @execute-task для задач.**

**Использование:**
```
@create-branch <branch-name>    # Только ручное создание для специальных случаев
```

**Когда использовать:**
- 🧪 Экспериментальные ветки (experiment/)
- 🔧 Hotfix ветки (hotfix/)
- 📚 Документация без кода (docs/)
- 🏗️ Рефакторинг (refactor/)
- 🎨 Конфигурации (config/)

**Примеры:**
- `@create-branch experiment/new-async-architecture` - тестирование идей
- `@create-branch hotfix/critical-memory-leak` - срочные исправления

## 🔄 Workflow использования

### 📋 Типичный цикл разработки:

```
1. @create-branch (или @execute-task автоматически)
   └── Создает новую git ветку от последнего master
   └── Выбирает задачу и обновляет статус: ⏳ → 🔄
   └── Настраивает tracking ветки в плане

2. @execute-task
   └── Работает в созданной ветке
   └── Создает тесты автоматически
   └── Реализует функциональность
   └── Запускает тесты в Docker
   └── Коммитит изменения с conventional commits

3. @run-tests --watch
   └── Continuous testing во время разработки
   └── Мониторинг изменений файлов
   └── Автоматический перезапуск тестов

4. @analyze-logs --auto
   └── Автоматический анализ логов после компиляции
   └── Выявление проблем в реальном времени
   └── Диагностика Docker и RimWorld ошибок

5. @create-tests [NewComponent]
   └── Создание тестов для новых компонентов
   └── Покрытие всех аспектов: Unit/Integration/Performance/MP
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

### 🎯 Приоритизация задач:

1. **🔴 КРИТИЧЕСКИЙ** - Компиляция, AsyncTime, базовый функционал
2. **🟠 ВЫСОКИЙ** - Производительность, совместимость с модами
3. **🟡 СРЕДНИЙ** - UI, дополнительные функции

## 🧪 Testing Strategy

### Каждая команда включает тестирование:

```
🧪 Test Pyramid:
├── 🔵 Unit Tests (60%)
│   ├── Core functionality
│   ├── Error handling
│   ├── Edge cases
│   └── Thread safety
├── 🟡 Integration Tests (25%)
│   ├── System integration
│   ├── Harmony patches
│   └── RimWorld API
├── 🔴 Performance Tests (10%)
│   ├── TPS benchmarks
│   ├── Memory usage
│   └── Thread efficiency
└── 🛡️ Multiplayer Tests (5%)
    ├── AsyncTime detection
    ├── Sync safety
    └── Desync prevention
```

### 📊 Quality Gates:

- ✅ **90%+ test coverage** обязательно
- ✅ **Все критические тесты** должны проходить
- ✅ **Performance targets** должны достигаться
- ✅ **0 multiplayer desyncs** в AsyncTime режиме

## 🔧 Настройка команд

### Как активировать команды в Cursor IDE:

1. **Откройте проект** RimAsync в Cursor IDE
2. **Команды автоматически доступны** через символ `@`
3. **В чате начните печатать** `@execute-task` или другую команду
4. **Следуйте инструкциям** AI для выполнения задач

### Интеграция с планами:

Команды автоматически читают и обновляют:
- 📈 `Planning/Development_Plan.md` - текущие задачи
- 🔧 `Planning/Features_Plan.md` - приоритеты функций
- 🧪 `Planning/Testing_Plan.md` - стратегии тестирования
- 📊 `Planning/Performance_Plan.md` - цели производительности

## 📁 Структура проекта для команд

### Рабочие директории:
```
RimAsync/
├── Source/RimAsync/           # Основной код
├── Tests/                     # Все тесты (создается командами)
│   ├── Unit/
│   ├── Integration/
│   ├── Performance/
│   └── Multiplayer/
├── Planning/                  # Планы разработки
└── .cursor/commands/          # Команды (эта папка)
```

### Создаваемые файлы:
- **Test files:** `Tests/[Category]/[Component]Tests.cs`
- **Config files:** `Tests/TestConfig.cs`, `Tests/Utils/TestHelpers.cs`
- **Reports:** Автоматические отчеты в консоли

## 🎯 Примеры использования

### Начало работы над проектом:
```
User: @execute-task

AI: 🎯 Анализирую текущие задачи...
📋 Выбрана задача: "Компиляция проекта"
🔧 Исправляю ошибки компиляции...
🧪 Создаю тесты для компиляции...
✅ Задача завершена!
```

### Создание тестов для нового компонента:
```
User: @create-tests SmartCache

AI: 🧪 Создаю comprehensive тесты для SmartCache...
📁 Создано 4 файла тестов
✅ 28 тестов готовы к выполнению
```

### Запуск specific тестов:
```
User: @run-tests --multiplayer --verbose

AI: 🛡️ Запускаю multiplayer тесты...
📊 AsyncTime detection: ✅
🔍 Sync safety validation: ✅
📁 Checking MpDesyncs folder: ✅ No new files
```

## 🚨 Важные моменты

### ⚠️ Перед использованием команд:
1. **Убедитесь** что проект открыт в Cursor IDE
2. **Проверьте** что файлы планирования существуют
3. **Настройте** Git repository для commits

### 🔍 Мониторинг результатов:
- **Логи тестов** сохраняются в консоли
- **Performance metrics** отображаются в реальном времени
- **Desync logs** проверяются в `/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs`

### 📊 Отслеживание прогресса:
- **Статусы задач** автоматически обновляются в `Planning/Development_Plan.md`
- **Test results** включают regression analysis
- **Performance trends** отслеживаются между запусками

## 🔮 Будущие команды (в планах)

### Дополнительные команды:
- `@benchmark` - Глубокий performance анализ
- `@deploy` - Подготовка к релизу и Steam Workshop
- `@compatibility-check` - Тестирование с популярными модами
- `@update-plan` - Обновление планов разработки
- `@generate-docs` - Автоматическая генерация документации

## 🤝 Contribution

### Добавление новых команд:
1. Создайте файл `new-command.md` в этой папке
2. Следуйте структуре существующих команд
3. Обновите этот README.md
4. Протестируйте команду с реальными сценариями

### Улучшение существующих команд:
1. Обновите соответствующий `.md` файл
2. Добавьте новые примеры использования
3. Обновите test templates если нужно

---

## 📝 Quick Reference

### Основные команды:
```bash
@execute-task                 # Выполнить следующую задачу
@create-tests [component]     # Создать тесты
@run-tests [options]          # Запустить тесты
```

### Полезные опции:
```bash
--critical                    # Только критические тесты
--watch                       # Continuous testing
--performance                 # Performance benchmarks
--multiplayer                 # Multiplayer compatibility
--verbose                     # Подробный вывод
```

---

**🚀 Начните с команды `@execute-task` для запуска разработки!**

*"Автоматизация - это ключ к успеху RimAsync!"* ⚡
