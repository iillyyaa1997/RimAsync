# Execute Task Command

**Команда:** `@execute-task`
**Описание:** Выполнить следующую приоритетную задачу из плана разработки RimAsync

## 🎯 Использование

В чате с LLM в Cursor IDE напишите:
```
@execute-task
```

## 🔧 Что делает команда

1. **Создает новую git ветку** от последнего master для задачи
2. **Анализирует текущий статус** проекта из Planning/Development_Plan.md
3. **Выбирает задачу** с наивысшим приоритетом
4. **Создает тесты** для задачи (если требуется)
5. **Реализует функциональность** согласно техническому плану
6. **Компилирует проект в Docker** контейнере для безопасности
7. **Запускает тесты в Docker** для проверки реализации
8. **Коммитит изменения** в feature ветку
9. **Обновляет статус** задачи в плане

## 🐳 Docker Environment

**ВСЕ операции компиляции и тестирования выполняются в Docker контейнерах!**

### 📋 Makefile Commands (Рекомендуемые):
- `make build` - компиляция проекта (с цветным выводом)
- `make test` - запуск тестов (с прогрессом)
- `make dev` - режим разработки
- `make quick-build` - быстрая компиляция для отладки
- `make help` - показать все доступные команды

### 🔧 Raw Docker Commands (альтернатива):
- `docker-compose up build` - компиляция проекта
- `docker-compose up test` - запуск тестов
- `docker-compose up dev` - режим разработки
- `docker-compose up quick-build` - быстрая компиляция для отладки

**💡 Используйте Makefile команды для лучшего UX и автоматической очистки!**

## 📋 Приоритетная очередь задач

### 🔴 КРИТИЧЕСКИЙ приоритет (выполняется первым):
1. **Компиляция проекта** - исправление ошибок компиляции
2. **AsyncTime integration** - тестирование detection логики
3. **Базовый async pathfinding** - проверка работоспособности
4. **Performance monitoring** - real-time метрики TPS

### 🟠 ВЫСОКИЙ приоритет:
1. **AI processing async** - расширение async функциональности
2. **Building updates async** - async обработка зданий
3. **Mod compatibility testing** - тестирование с топ-5 модами
4. **Memory optimization** - SmartCache improvements

### 🟡 СРЕДНИЙ приоритет:
1. **Advanced settings UI** - дополнительные опции
2. **Debug overlay** - расширенные метрики
3. **Documentation polish** - улучшение документации
4. **Community feedback integration** - интеграция отзывов

## 🧪 Testing Strategy

Каждая задача **ОБЯЗАТЕЛЬНО** включает:

### 1. Unit Tests
```csharp
[Test]
public void TaskName_Scenario_ExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```

### 2. Integration Tests
```csharp
[Test]
public void TaskName_Integration_WorksWithSystem()
{
    // Test integration with existing systems
}
```

### 3. Performance Tests
```csharp
[Test]
public void TaskName_Performance_MeetsTargets()
{
    // Benchmark performance improvements
}
```

### 4. Multiplayer Tests
```csharp
[Test]
public void TaskName_Multiplayer_NoDesync()
{
    // Test multiplayer compatibility
}
```

## 🔄 Workflow

### 🌿 Git Branch Management
1. **git checkout master** - переключение на основную ветку
2. **git pull origin master** - получение последних изменений
3. **git checkout -b feature/task-name** - создание новой feature ветки

### 🎯 Task Execution
4. **Анализирует Planning/Development_Plan.md**
5. **Выбирает следующую критическую задачу**
6. **Создает файл тестов:** `Tests/[TaskName]Tests.cs`
7. **Реализует функциональность** в соответствующем файле
8. **make format-fix** - автоформатирование кода (ОБЯЗАТЕЛЬНО)
9. **make build** - компиляция в Docker (удобная Makefile команда)
10. **make lint** - проверка качества кода (ОБЯЗАТЕЛЬНО)
11. **make test** - запуск тестов (удобная Makefile команда)
12. **Проверяет результаты** и исправляет ошибки если нужно

### 📝 Git Commit & Update
11. **git add .** - добавляет изменения
12. **git commit -m "feat(task): descriptive message"** - коммит с conventional commits
13. **Обновляет статус** в плане: ⏳ → 🔄 → ✅
14. **git push origin feature/task-name** - пушит ветку (опционально)

## 📁 Структура тестов

```
Tests/
├── Unit/
│   ├── Core/
│   │   ├── RimAsyncModTests.cs
│   │   ├── RimAsyncCoreTests.cs
│   │   └── RimAsyncSettingsTests.cs
│   ├── Threading/
│   │   ├── AsyncManagerTests.cs
│   │   └── PerformanceMonitorTests.cs
│   └── Utils/
│       ├── MultiplayerCompatTests.cs
│       └── SmartCacheTests.cs
├── Integration/
│   ├── HarmonyPatchTests.cs
│   ├── MultiplayerIntegrationTests.cs
│   └── ModCompatibilityTests.cs
├── Performance/
│   ├── TPSBenchmarkTests.cs
│   ├── MemoryUsageTests.cs
│   └── ThreadUtilizationTests.cs
└── Multiplayer/
    ├── AsyncTimeTests.cs
    ├── SyncSafetyTests.cs
    └── DesyncPreventionTests.cs
```

## 🎯 Критерии завершения задачи

- ✅ **Функциональность реализована** согласно техническому плану
- ✅ **Все тесты проходят** (Unit + Integration + Performance)
- ✅ **Код style соответствует** стандартам проекта
- ✅ **Документация обновлена** (если требуется)
- ✅ **Performance targets достигнуты** (если применимо)
- ✅ **Multiplayer compatibility** проверена (если применимо)
- ✅ **Статус обновлен** в Planning/Development_Plan.md

## 🚀 Пример выполнения

```
User: @execute-task

AI: 🌿 Создаю новую git ветку...
git checkout master
git pull origin master
git checkout -b feature/fix-compilation-errors

🎯 Анализирую текущие задачи...

📋 Выбрана задача: "Компиляция проекта - исправление ошибок компиляции"
🔴 Приоритет: КРИТИЧЕСКИЙ
📁 Файлы: Source/RimAsync/Core/*.cs

🧪 Создаю тесты:
- Tests/Unit/Core/CompilationTests.cs
- Tests/Integration/ProjectStructureTests.cs

🔧 Исправляю ошибки компиляции:
- Добавляю недостающие using statements
- Исправляю namespace conflicts
- Обновляю references

🎨 Автоформатирование кода:
make format-fix ✅ Стиль кода исправлен

🐳 Docker компиляция:
make build ✅

🔍 Проверка качества кода:
make lint ✅ Анализ пройден, 0 ошибок

🧪 Docker тестирование:
make test ✅ 8/8 тестов пройдено

📝 Коммичу изменения:
git add .
git commit -m "feat(compilation): fix compilation errors and add tests

- Add missing using statements for RimWorld/Verse namespaces
- Fix .csproj structure for .NET Standard 2.1
- Add mock references for Docker compilation
- Add comprehensive compilation tests

Resolves: Compilation errors in Docker environment"

✅ Статус обновлен: ⏳ → ✅
🌿 Ветка готова: feature/fix-compilation-errors

🎉 Задача завершена! Следующая задача: "AsyncTime integration testing"
```

---

**Примечание:** Эта команда автоматически выбирает наиболее приоритетную задачу. Для выбора конкретной задачи используйте `@execute-task [task-name]`
