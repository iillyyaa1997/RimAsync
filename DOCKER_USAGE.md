# Использование Docker в RimAsync

## 🐳 Обзор

**RimAsync полностью использует Docker для всех операций разработки.** Это обеспечивает:
- Изолированную среду разработки
- Воспроизводимые сборки
- Отсутствие зависимостей от локальной установки .NET SDK
- Консистентность между разработчиками

## 🚀 Быстрый старт

### Предварительные требования
- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Проверка установки
```bash
docker --version
docker-compose --version
```

## 📋 Основные команды

### 🔨 Компиляция

#### Полная сборка (Release)
```bash
docker-compose up build
```
- Создает оптимизированные assemblies
- Сохраняет результат в `./Build/Assemblies/`
- Копирует About файлы

#### Быстрая сборка для разработки (Debug)
```bash
docker-compose up quick-build
```
- Быстрая компиляция без оптимизаций
- Результат в `./Build/Debug/`

### 🧪 Тестирование

#### Запуск всех тестов
```bash
docker-compose up test
```
- Выполняет все Unit/Integration/Performance/Multiplayer тесты
- Создает отчеты в `./TestResults/`

#### Запуск конкретных категорий тестов
```bash
# Только Unit тесты
docker-compose run test dotnet test Tests/Unit/ --logger "console;verbosity=detailed"

# Только Integration тесты  
docker-compose run test dotnet test Tests/Integration/

# Только Performance тесты
docker-compose run test dotnet test Tests/Performance/

# Тесты с конкретным фильтром
docker-compose run test dotnet test Tests/ --filter "Category=CriticalPriority"
```

### 🛠️ Разработка

#### Интерактивная среда разработки
```bash
docker-compose up dev
```
- Запускает контейнер с live reload
- Подключает volumes для синхронизации кода
- Доступ через: `docker-compose exec dev bash`

#### Интерактивная работа в контейнере
```bash
# Получить доступ к shell в dev контейнере
docker-compose exec dev bash

# В контейнере можно выполнять:
dotnet build --project Source/RimAsync
dotnet test Tests/
dotnet watch --project Source/RimAsync
```

### 📦 Релиз

#### Создание production сборки
```bash
docker-compose up release
```
- Создает оптимизированную сборку
- Готовую для загрузки в Steam Workshop
- Результат в `./Release/`

## 📁 Структура output директорий

```
RimAsync/
├── Build/                    # Development builds
│   ├── Debug/               # Quick debug builds  
│   │   └── RimAsync.dll
│   ├── Assemblies/          # Release builds
│   │   ├── RimAsync.dll
│   │   └── RimAsync.pdb
│   └── About/               # Copied About files
│       ├── About.xml
│       └── Preview.png
├── TestResults/             # Test outputs
│   ├── TestResults.trx      # MSTest results
│   └── coverage/            # Coverage reports
└── Release/                 # Production ready
    ├── About/
    │   ├── About.xml
    │   └── Preview.png
    └── Assemblies/
        └── RimAsync.dll
```

## 🎯 Интеграция с Cursor IDE

### Команды автоматически используют Docker:

#### @execute-task
```
@execute-task
```
1. Анализирует план разработки
2. Выполняет `docker-compose up build`
3. Запускает `docker-compose up test`
4. Обновляет статус задач

#### @create-tests
```
@create-tests AsyncManager
```
1. Создает тестовые файлы
2. Запускает `docker-compose up test` для проверки
3. Сохраняет результаты

#### @run-tests
```
@run-tests --unit
```
1. Выполняет `docker-compose run test dotnet test Tests/Unit/`
2. Анализирует результаты из `./TestResults/`

## 🔧 Отладка и решение проблем

### Проблемы с сборкой

#### Очистка Docker кэша
```bash
# Остановить все контейнеры
docker-compose down

# Очистить system cache
docker system prune -f

# Пересобрать образы без кэша
docker-compose build --no-cache

# Запустить заново
docker-compose up build
```

#### Проблемы с volumes
```bash
# Пересоздать volumes
docker-compose down -v
docker-compose up build
```

### Отладка тестов

#### Запуск тестов с подробными логами
```bash
docker-compose run test dotnet test Tests/ --verbosity detailed --logger "console;verbosity=detailed"
```

#### Интерактивная отладка
```bash
# Запустить test контейнер интерактивно
docker-compose run --rm test bash

# В контейнере выполнить отладку
cd /app
dotnet test Tests/Unit/Core/CompilationTests.cs --verbosity detailed
```

### Логирование и мониторинг

#### Просмотр логов контейнера
```bash
# Логи build процесса
docker-compose logs build

# Логи тестов
docker-compose logs test

# Follow real-time logs
docker-compose logs -f dev
```

## 🚀 Продвинутое использование

### Кастомные Docker команды

#### Запуск только компиляции без тестов
```bash
docker-compose run --rm build sh -c "
  cd /app/Source/RimAsync &&
  dotnet restore &&
  dotnet build --configuration Release --output /app/Build/Custom
"
```

#### Создание собственного dev образа
```bash
# Создать образ для экспериментов
docker build -t rimasync-custom --target build .

# Запустить интерактивно
docker run -it --rm -v $(pwd):/app rimasync-custom bash
```

### Performance оптимизация

#### Многоэтапная сборка для скорости
```bash
# Использовать buildkit для параллельных builds
DOCKER_BUILDKIT=1 docker-compose build

# Кэширование зависимостей
docker-compose build --build-arg BUILDKIT_INLINE_CACHE=1
```

## 🔄 CI/CD интеграция

Все Docker команды готовы для использования в CI/CD пайплайнах:

```yaml
# Пример GitHub Actions
- name: Build RimAsync
  run: docker-compose up build

- name: Run Tests  
  run: docker-compose up test

- name: Create Release
  run: docker-compose up release
```

## ⚠️ Важные замечания

1. **НЕ ИСПОЛЬЗУЙ** локальные `dotnet` команды - только Docker
2. **ВСЕ volumes** настроены для синхронизации с host системой
3. **TestResults** сохраняются для анализа после завершения контейнеров
4. **Release сборки** готовы для загрузки в Steam Workshop
5. **Dev контейнер** поддерживает live reload для быстрой разработки

## 📞 Поддержка

При проблемах с Docker:
1. Проверь версии Docker и Docker Compose
2. Очисти кэш: `docker system prune -f`
3. Пересобери образы: `docker-compose build --no-cache`
4. Проверь логи: `docker-compose logs [service]` 