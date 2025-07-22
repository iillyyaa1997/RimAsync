# Create Branch Command

**Команда:** `@create-branch`
**Описание:** Создать git ветку вручную для специальных случаев (НЕ для обычного development workflow)

## ⚠️ ВАЖНО: Когда НЕ использовать эту команду

**🚫 НЕ используйте `@create-branch` для обычной разработки!**

Для выполнения задач используйте `@execute-task` - она автоматически создает ветку и выполняет задачу.

## 🎯 Когда использовать @create-branch

Эта команда предназначена для **специальных случаев**:

- 🧪 **Экспериментальные ветки** - для тестирования идей
- 🔧 **Hotfix ветки** - для срочных исправлений
- 📚 **Документация** - когда работаете только с docs
- 🏗️ **Рефакторинг** - для крупных структурных изменений
- 🎨 **Настройка среды** - изменения конфигураций без кода

## 🌿 Использование

```
@create-branch <branch-name>
```

**Примеры специальных случаев:**
```
@create-branch experiment/new-architecture
@create-branch hotfix/critical-memory-leak
@create-branch docs/api-documentation
@create-branch refactor/project-structure
@create-branch config/docker-optimization
```

## 🔄 Workflow

### 1. **Pre-branch проверки:**
- ✅ Проверяет что нет uncommitted changes
- ✅ Переключается на master
- ✅ Обновляет master (git pull origin master)
- ✅ Проверяет что master актуальный

### 2. **Создание ветки:**
- 🌿 Создает новую ветку от последнего master
- 🔀 Переключается на новую ветку
- 📋 Показывает статус

### 3. **После создания:**
- 💡 Напоминает о следующих шагах
- 📝 Предлагает использовать `@execute-task` для задач

## 🚨 Error Handling

### Uncommitted Changes:
```
❌ Ошибка: У вас есть uncommitted changes
💡 Решение:
   - git commit или git stash ваши изменения
   - Затем повторите команду
```

### Outdated Master:
```
❌ Ошибка: Master branch устарел
💡 Решение:
   - git checkout master
   - git pull origin master
   - Затем повторите команду
```

### Branch Already Exists:
```
❌ Ошибка: Ветка 'feature/name' уже существует
💡 Решение:
   - Выберите другое имя ветки
   - Или удалите существующую: git branch -D feature/name
```

## 📋 Naming Conventions

### Рекомендуемые префиксы:
- `experiment/` - экспериментальные изменения
- `hotfix/` - критические исправления
- `docs/` - обновления документации
- `refactor/` - рефакторинг кода
- `config/` - изменения конфигураций
- `research/` - исследовательские ветки

### Примеры хороших имен:
```bash
✅ experiment/async-ui-rendering
✅ hotfix/memory-leak-pathfinder
✅ docs/api-reference-update
✅ refactor/extract-async-manager
✅ config/improve-docker-performance
```

### Избегайте:
```bash
❌ test                    # Слишком общее
❌ my-branch              # Не информативное
❌ feature/new-feature    # Избыточность (feature/feature)
❌ fix-bug                # Какой именно баг?
```

## 🔗 Интеграция с другими командами

### После создания ветки:
```bash
# Для выполнения задачи development:
@execute-task [task-name]

# Для тестирования:
@run-tests

# Для анализа:
@analyze-logs
```

## 💡 Best Practices

### 1. **Используйте описательные имена**
```bash
✅ experiment/thread-safe-collections
❌ experiment/test1
```

### 2. **Документируйте цель ветки**
После создания добавьте комментарий в первый commit:
```bash
git commit -m "Create experimental branch for testing new async architecture

Goal: Evaluate performance impact of async collections
Expected outcome: 15-20% performance improvement
Risk: Potential compatibility issues with existing mods"
```

### 3. **Регулярно sync с master**
```bash
git checkout master
git pull origin master
git checkout your-branch
git rebase master
```

## 🎯 Типичный пример использования

```bash
# Хотите протестировать новую идею architecture
@create-branch experiment/event-driven-async

# Ветка создана, теперь работаете над экспериментом
# ... делаете изменения ...

# Когда готово к merge:
git checkout master
git merge experiment/event-driven-async
git branch -d experiment/event-driven-async
```

## 🚀 Напоминание

**Для обычной разработки используйте `@execute-task`** - она автоматически:
- Создает feature ветку от master
- Выполняет задачу из Development Plan
- Форматирует код и запускает тесты
- Коммитит изменения

`@create-branch` - только для специальных случаев! 🎯
