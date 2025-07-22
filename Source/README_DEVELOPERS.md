# RimAsync - Developer Documentation

Техническая документация для разработчиков RimAsync.

## 🏗️ Архитектура проекта

### Основные компоненты

```
Source/RimAsync/
├── Core/                    # Основные системы
│   ├── RimAsyncMod.cs      # Главный класс мода, Harmony инициализация
│   └── RimAsyncCore.cs     # Координатор систем, ExecutionMode
├── Settings/                # Настройки
│   └── RimAsyncSettings.cs # UI настроек с полными контролями
├── Threading/               # Асинхронные операции
│   └── AsyncManager.cs     # Управление async с AsyncTime поддержкой
├── Utils/                   # Утилиты
│   ├── MultiplayerCompat.cs     # Обнаружение Multiplayer через reflection
│   └── PerformanceMonitor.cs    # TPS мониторинг и метрики
├── Components/              # Игровые компоненты
│   └── RimAsyncGameComponent.cs # GameComponent для lifecycle
└── Patches/                 # Harmony патчи
    ├── RW_Patches/         # Vanilla RimWorld патчи
    ├── Multiplayer_Patches/ # Multiplayer совместимость
    ├── Performance_Patches/ # Оптимизация производительности
    └── Mod_Patches/        # Совместимость с другими модами
```

## 🎯 Execution Modes

RimAsync работает в трех режимах:

### 1. FullAsync (Single Player)
- Полная асинхронная обработка
- Максимальная производительность
- Используется когда `!MultiplayerCompat.IsInMultiplayer`

### 2. AsyncTimeEnabled (Multiplayer + AsyncTime)
- Ограниченные асинхронные операции
- Безопасно для мультиплеера
- Используется когда `MultiplayerCompat.AsyncTimeEnabled == true`

### 3. FullSync (Multiplayer без AsyncTime)
- Полностью синхронное выполнение
- Максимальная безопасность для MP
- Fallback режим

## 🔄 AsyncTime Integration

### Ключевые принципы:

```csharp
// Всегда проверяй режим выполнения
var mode = RimAsyncCore.GetExecutionMode();

// Используй AsyncManager для adaptive execution
await AsyncManager.ExecuteAdaptive(
    async (ct) => { /* async operation */ },
    () => { /* sync fallback */ },
    "OperationName");

// Проверяй доступность async операций
if (AsyncManager.CanExecuteAsync())
{
    // Безопасно использовать async
}
```

### AsyncTime-Safe операции:
- Background pathfinding calculations
- Performance monitoring
- Cache warming
- Non-game-state UI updates

### Должны остаться sync:
- Game state modifications
- RNG operations
- Save/load operations
- Critical game logic

## 🛠️ Разработка патчей

### Структура Harmony патча:

```csharp
[HarmonyPatch(typeof(TargetClass), nameof(TargetClass.Method))]
public static class TargetClass_Method_Patch
{
    [HarmonyPrefix]
    public static bool Prefix(TargetClass __instance, ref ReturnType __result)
    {
        // Проверь настройки
        if (!RimAsyncMod.Settings.enableOptimization)
            return true; // Use original

        // Проверь критичность операции
        if (IsCriticalOperation(__instance))
            return true; // Use original

        try
        {
            // Примени оптимизацию
            var result = TryOptimization(__instance);
            if (result != null)
            {
                __result = result;
                return false; // Skip original
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[RimAsync] Error in optimization: {ex}");
        }

        return true; // Fallback to original
    }
}
```

### Performance Monitoring:

```csharp
// Измерение производительности
using (PerformanceMonitor.StartMeasuring("OperationName"))
{
    // Your code here
}

// Запись метрик
PerformanceMonitor.RecordMetric("PathfindingTime", duration);
```

## 🔧 Multiplayer Compatibility

### Detection паттерн:

```csharp
// В MultiplayerCompat.cs через reflection
var multiplayerAssembly = GetMultiplayerAssembly();
var apiType = multiplayerAssembly.GetType("Multiplayer.Client.MultiplayerAPI");
var isInMultiplayerProp = apiType.GetProperty("IsInMultiplayer");
```

### Безопасные операции:

```csharp
// Wrap операции для MP безопасности
MultiplayerCompat_Patch.WrapMultiplayerSafeOperation(() =>
{
    // Your operation
}, "OperationName");
```

## 📊 Performance Guidelines

### TPS мониторинг:
- `PerformanceMonitor.CurrentTPS` - текущий TPS
- `PerformanceMonitor.AverageTPS` - средний TPS
- `PerformanceMonitor.IsPerformanceGood` - TPS > 50

### Автоматическая настройка:
- При TPS < 30 - уменьшить оптимизации
- При TPS > 55 - можно увеличить оптимизации

## 🧪 Testing

### Обязательные тесты:
1. **Single Player** - полная функциональность
2. **Multiplayer + AsyncTime** - ограниченный async
3. **Multiplayer без AsyncTime** - полный sync
4. **Mod Compatibility** - с популярными модами

### Debug режим:
- `enableDebugLogging = true` в настройках
- Показывает performance overlay
- Логирует все операции

## 🔗 Dependencies

### Required:
- **Harmony** - для патчинга
- **RimWorld 1.5+** - базовая игра

### Optional:
- **RimWorld Multiplayer** - для MP функций

### References в .csproj:
- `0Harmony.dll`
- `Assembly-CSharp.dll` (RimWorld)
- `UnityEngine.CoreModule.dll`

## 📝 Coding Standards

- **Английские комментарии** - обязательно для всего кода
- **Namespace:** `RimAsync.*`
- **Harmony ID:** `rimasync.mod`
- **Fallback обязателен** - всегда возврат к оригинальному коду
- **Exception handling** - безопасная обработка ошибок

## 🚀 Build Process

```bash
# Visual Studio / Rider
1. Открыть RimAsync.csproj
2. Set RimWorldInstallDir environment variable
3. Build -> Output в 1.5/Assemblies/

# Результат:
- RimAsync.dll в 1.5/Assemblies/
```

---

**Developed by [@iillyyaa1997](https://github.com/iillyyaa1997) for RimWorld community** 🚀 