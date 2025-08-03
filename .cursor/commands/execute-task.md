# Execute Task Command

**Команда:** `@execute-task`
**Описание:** Выполнить следующую приоритетную задачу из плана разработки RimAsync с обязательной разбивкой на подзадачи

## 🎯 Использование

В чате с LLM в Cursor IDE напишите:
```
@execute-task
```

## 🔧 Что делает команда

1. **Создает новую git ветку** от последнего master для задачи
2. **Анализирует текущий статус** проекта из Planning/Development_Plan.md
3. **Выбирает задачу** с наивысшим приоритетом
4. **ОБЯЗАТЕЛЬНО разбивает задачу** на 2-5 конкретных подзадач
5. **Создает TODO список** с четкими критериями успеха
6. **Создает тесты** для задачи (если требуется)
7. **Реализует функциональность** согласно техническому плану
8. **Компилирует проект в Docker** контейнере для безопасности
9. **Запускает тесты в Docker** для проверки реализации
10. **Коммитит изменения** в feature ветку
11. **Мержит в master** и удаляет feature ветку
12. **Обновляет статус** задачи в плане

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

## 🔧 Task Breakdown Guidelines

### 📝 Правила разбивки задач:

**ОБЯЗАТЕЛЬНО:** Каждая задача должна быть разбита на 2-5 конкретных подзадач

#### ✅ Хорошие подзадачи:
- **Конкретные и измеримые** - "Добавить метод Initialize() в AsyncManager"
- **Выполнимые за 1-2 часа** - не слишком большие или маленькие
- **Независимые** - можно выполнить в любом порядке
- **Тестируемые** - есть четкий критерий завершения

#### ❌ Плохие подзадачи:
- **Слишком общие** - "Улучшить производительность"
- **Слишком большие** - "Переписать всю систему"
- **Зависимые** - нельзя начать без предыдущей
- **Нетестируемые** - неясно когда завершена

### 🎯 Примеры разбивки:

**Задача:** "AsyncTime integration - тестирование detection логики"

**Подзадачи:**
1. 🔧 Создать mock для MultiplayerCompat.IsInMultiplayer
2. 🧪 Написать тесты для single player режима
3. 🧪 Написать тесты для multiplayer без AsyncTime
4. 🧪 Написать тесты для multiplayer с AsyncTime
5. 📊 Добавить performance тесты для переключения режимов

**Задача:** "Performance monitoring - real-time метрики TPS"

**Подзадачи:**
1. 🔧 Создать PerformanceMonitor класс с TPS tracking
2. 🔧 Добавить методы StartMeasuring/StopMeasuring
3. 🧪 Написать unit тесты для измерения производительности
4. 🎮 Интегрировать с RimAsyncCore для автоматического мониторинга

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

### 📋 Task Planning & Breakdown
4. **Анализирует Planning/Development_Plan.md**
5. **Выбирает следующую критическую задачу**
6. **ОБЯЗАТЕЛЬНО разбивает задачу на подзадачи** (2-5 подзадач максимум)
7. **Создает TODO список** с конкретными шагами выполнения
8. **Определяет критерии успеха** для каждой подзадачи

### 🎯 Task Execution
9. **Создает файл тестов:** `Tests/[TaskName]Tests.cs`
10. **Реализует функциональность** в соответствующем файле
11. **make format-fix** - автоформатирование кода (ОБЯЗАТЕЛЬНО)
12. **make build** - компиляция в Docker (удобная Makefile команда)
13. **make lint** - проверка качества кода (ОБЯЗАТЕЛЬНО)
14. **make test** - запуск тестов (удобная Makefile команда)
15. **Проверяет результаты** и исправляет ошибки если нужно

### 📝 Git Commit & Update
16. **git add .** - добавляет изменения
17. **git commit -m "feat(task): descriptive message"** - коммит с conventional commits
18. **Обновляет статус** в плане: ⏳ → 🔄 → ✅
19. **git push origin feature/task-name** - пушит ветку (опционально)

### 🔄 Branch Merge & Cleanup (ФИНАЛЬНЫЙ ЭТАП)
20. **git checkout master** - переключается на master
21. **git merge feature/task-name** - мержит feature ветку в master
22. **git push origin master** - пушит обновленный master
23. **git branch -d feature/task-name** - удаляет локальную feature ветку

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

- ✅ **Все подзадачи завершены** - каждая подзадача из TODO списка выполнена
- ✅ **Функциональность реализована** согласно техническому плану
- ✅ **Все тесты проходят** (Unit + Integration + Performance)
- ✅ **Код style соответствует** стандартам проекта
- ✅ **Документация обновлена** (если требуется)
- ✅ **Performance targets достигнуты** (если применимо)
- ✅ **Multiplayer compatibility** проверена (если применимо)
- ✅ **Статус обновлен** в Planning/Development_Plan.md
- ✅ **Feature ветка смержена** в master и удалена

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

🔧 Разбиваю задачу на подзадачи:
1. 🔍 Анализ ошибок компиляции в Docker
2. 🔧 Добавление недостающих using statements
3. 🔧 Исправление namespace conflicts
4. 🔧 Обновление mock references
5. 🧪 Создание comprehensive compilation тестов

📝 TODO List создан - 5 подзадач определены!

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

🔄 Мержу в master и очищаю ветки:
git checkout master
git merge feature/fix-compilation-errors ✅ Fast-forward merge
git push origin master ✅ Master обновлен
git branch -d feature/fix-compilation-errors ✅ Ветка удалена

🎉 Задача завершена! Следующая задача: "AsyncTime integration testing"
```

---

**Примечание:** Эта команда автоматически выбирает наиболее приоритетную задачу. Для выбора конкретной задачи используйте `@execute-task [task-name]`
