# RimAsync - Real In-Game Testing Guide

**Дата:** 2 ноября 2025  
**Статус:** ✅ Готов к реальному тестированию  
**Версия:** 1.0.0 (supports RimWorld 1.5 & 1.6)

## 🎯 Цель

Провести реальное тестирование мода в RimWorld для проверки:
- Загрузки мода без ошибок
- Работы базового функционала
- Performance improvements
- Multiplayer compatibility (with AsyncTime)

---

## 📋 Pre-Testing Checklist

### 1. Компиляция мода

```bash
cd /Users/ilyavolkov/Workspace/RimAsync
make build
```

**Проверить:**
- ✅ Build successful (0 errors)
- ✅ DLL созданы в `1.5/Assemblies/RimAsync.dll`
- ✅ DLL созданы в `1.6/Assemblies/RimAsync.dll`

### 2. Установка в RimWorld

**Путь к папке модов RimWorld (macOS):**
```bash
~/Library/Application Support/Steam/steamapps/common/RimWorld/Mods/
```

**Установка:**
```bash
# Опция 1: Symbolic link (рекомендуется для разработки)
ln -s /Users/ilyavolkov/Workspace/RimAsync \
  ~/Library/Application\ Support/Steam/steamapps/common/RimWorld/Mods/RimAsync

# Опция 2: Копирование (для stable тестирования)
cp -r /Users/ilyavolkov/Workspace/RimAsync \
  ~/Library/Application\ Support/Steam/steamapps/common/RimWorld/Mods/RimAsync
```

**Проверить структуру:**
```
RimWorld/Mods/RimAsync/
  ├── About/
  │   ├── About.xml
  │   └── Preview.png
  ├── 1.5/
  │   └── Assemblies/
  │       └── RimAsync.dll
  └── 1.6/
      └── Assemblies/
          └── RimAsync.dll
```

---

## 🧪 Testing Phases

### Phase 1: Basic Loading (5 minutes)

**Цель:** Убедиться, что мод загружается без ошибок

**Шаги:**
1. Запустить RimWorld
2. Открыть **Mods** menu
3. Найти **RimAsync** в списке
4. Включить мод (Enable)
5. Перезапустить игру

**Проверить:**
- [ ] Мод появляется в списке модов
- [ ] Мод включается без ошибок
- [ ] Игра загружается после перезапуска
- [ ] Нет красных ошибок в логах

**Логи проверить:**
```bash
# RimWorld Player.log location (macOS)
~/Library/Logs/Unity/Player.log

# Поиск ошибок RimAsync
grep -i "rimasync" ~/Library/Logs/Unity/Player.log
grep -i "error" ~/Library/Logs/Unity/Player.log | grep -i "rimasync"
```

---

### Phase 2: Settings UI (5 minutes)

**Цель:** Проверить UI настроек мода

**Шаги:**
1. В главном меню: **Options → Mod Settings**
2. Выбрать **RimAsync**
3. Проверить все настройки

**Проверить:**
- [ ] Settings UI открывается
- [ ] Toggle switches работают (Enable/Disable)
- [ ] Thread limit slider работает (1-64)
- [ ] Auto thread limits toggle работает
- [ ] Enable debug mode checkbox работает
- [ ] Все описания (tooltips) отображаются

**Настройки для тестирования:**
```
✅ Enable RimAsync: ON
✅ Enable debug mode: ON
✅ Auto thread limits: ON
✅ Max async threads: 8 (auto)
```

---

### Phase 3: Debug Overlay (5 minutes)

**Цель:** Проверить debug overlay и метрики

**Шаги:**
1. Загрузить любую карту (или создать новую)
2. Нажать **F11** для toggle debug overlay
3. Наблюдать метрики

**Проверить:**
- [ ] F11 toggle работает
- [ ] Overlay отображается в углу экрана
- [ ] TPS (Ticks Per Second) показывается
- [ ] Cache stats показываются
- [ ] Async operations counter показывается
- [ ] Thread status показывается
- [ ] Settings summary показывается

**Ожидаемые метрики:**
```
=== RimAsync Debug ===
TPS: 60.00 (normal: 60.00)
Cache: X hits, Y misses (Z% hit rate)
Async: N operations active
Threads: 8/8 available
Mode: AsyncSinglePlayer
Settings: Enabled, Debug: ON
======================
```

---

### Phase 4: Basic Gameplay (15 minutes)

**Цель:** Проверить базовый функционал в игре

**Шаги:**
1. Создать новую колонию (3-5 pawns)
2. Играть 10-15 минут
3. Выполнить различные действия

**Действия для теста:**
- [ ] **Pathfinding:** Приказать pawn'ам перемещаться по карте
- [ ] **Jobs:** Дать разные приказы (haul, construct, etc.)
- [ ] **Building:** Построить 2-3 здания
- [ ] **AI thinking:** Наблюдать за AI решениями
- [ ] **Performance:** Проверить TPS в debug overlay

**Проверить:**
- [ ] Pawns перемещаются нормально
- [ ] Pathfinding работает корректно (нет зависаний)
- [ ] Jobs выполняются без проблем
- [ ] Buildings строятся нормально
- [ ] TPS остается стабильным (55-60)
- [ ] Нет красных ошибок в логах

**Performance baseline:**
- TPS без нагрузки: 60
- TPS с 5 pawns: 58-60
- TPS при pathfinding (5 pawns): 55-60

---

### Phase 5: Performance Testing (20 minutes)

**Цель:** Измерить реальное улучшение производительности

**Шаги:**
1. Создать колонию с 10+ pawns
2. Провести stress test
3. Сравнить TPS с/без RimAsync

**Test scenarios:**

#### Scenario 1: Large pathfinding
```
- 10 pawns одновременно
- Приказать всем переместиться на противоположный конец карты
- Измерить TPS
```

#### Scenario 2: Mass building
```
- Заложить фундамент большого здания (20x20)
- 10 pawns строят одновременно
- Измерить TPS
```

#### Scenario 3: Complex AI
```
- 10+ pawns в колонии
- Raid или event с много enemies
- Измерить TPS во время боя
```

**Ожидаемые результаты:**
```
Без RimAsync: 45-55 TPS (large colony)
С RimAsync: 50-60 TPS (15-30% improvement)
```

**Записать результаты:**
```bash
# Создать файл с результатами
echo "Performance Test Results - $(date)" > ~/Desktop/RimAsync_Performance.txt
echo "Scenario 1: TPS = XX.XX" >> ~/Desktop/RimAsync_Performance.txt
echo "Scenario 2: TPS = XX.XX" >> ~/Desktop/RimAsync_Performance.txt
echo "Scenario 3: TPS = XX.XX" >> ~/Desktop/RimAsync_Performance.txt
```

---

### Phase 6: Multiplayer Testing (30 minutes)

**⚠️ Требует RimWorld Multiplayer mod!**

**Цель:** Проверить совместимость с Multiplayer и AsyncTime

**Prerequisite:**
- Установить [RimWorld Multiplayer](https://steamcommunity.com/sharedfiles/filedetails/?id=1752864297)
- Load order: `Harmony → Multiplayer → RimAsync`

**Шаги:**

#### 6.1 Multiplayer Detection
1. Запустить игру с Multiplayer mod
2. Проверить detection в логах

**Проверить:**
```bash
grep "Multiplayer detected" ~/Library/Logs/Unity/Player.log
```

#### 6.2 Solo Multiplayer Session
1. Создать multiplayer сессию (solo)
2. Играть 10 минут
3. Проверить TPS и стабильность

**Проверить:**
- [ ] Multiplayer session создается
- [ ] RimAsync работает в multiplayer режиме
- [ ] TPS стабилен
- [ ] Нет desyncs

#### 6.3 AsyncTime Detection (если доступно)
```bash
# Проверить логи на AsyncTime detection
grep "AsyncTime" ~/Library/Logs/Unity/Player.log
```

**Проверить:**
- [ ] AsyncTime detection работает
- [ ] Execution mode переключается на AsyncWithTime
- [ ] Нет desyncs

**Desyncs проверить:**
```bash
ls -lh ~/Library/Application\ Support/RimWorld/MpDesyncs/
```

**Должно быть:** 0 desyncs во время теста

---

## 🐛 Bug Reporting Template

Если найдешь баг, создай файл:
```bash
touch ~/Desktop/RimAsync_Bug_Report.txt
```

**Template:**
```
BUG REPORT: [Short Description]
Date: [Date]
RimWorld Version: [1.5 or 1.6]
RimAsync Version: 1.0.0

STEPS TO REPRODUCE:
1. 
2. 
3. 

EXPECTED BEHAVIOR:


ACTUAL BEHAVIOR:


LOGS:
[Paste relevant log lines from Player.log]

SCREENSHOTS:
[Attach if possible]

MODS LOADED:
[List other active mods]
```

---

## 📊 Performance Metrics Template

**Создать файл с метриками:**
```bash
cat > ~/Desktop/RimAsync_Metrics.txt << 'EOF'
RIMASYNC PERFORMANCE METRICS
============================
Test Date: [Date]
RimWorld Version: [1.5/1.6]

BASELINE (Without RimAsync):
- Empty colony TPS: ___
- 5 pawns TPS: ___
- 10 pawns TPS: ___
- Large pathfinding TPS: ___
- Mass building TPS: ___

WITH RIMASYNC:
- Empty colony TPS: ___
- 5 pawns TPS: ___
- 10 pawns TPS: ___
- Large pathfinding TPS: ___
- Mass building TPS: ___

IMPROVEMENT:
- Empty: +___%
- 5 pawns: +___%
- 10 pawns: +___%
- Pathfinding: +___%
- Building: +___%

MULTIPLAYER (if tested):
- Solo session TPS: ___
- AsyncTime detected: [YES/NO]
- Desyncs: [count]

NOTES:
[Any observations, issues, or interesting findings]
EOF
```

---

## ✅ Success Criteria

**Minimum for "PASS":**
- [ ] Mod loads without errors
- [ ] Settings UI works
- [ ] Debug overlay works (F11)
- [ ] Basic gameplay functional (pathfinding, jobs, building)
- [ ] No critical errors in logs
- [ ] TPS >= baseline (no performance regression)

**Ideal for "EXCELLENT":**
- [ ] All above + 
- [ ] TPS improvement 15-30%
- [ ] Multiplayer detection works
- [ ] 0 desyncs in multiplayer
- [ ] AsyncTime integration works

---

## 🚨 Emergency Rollback

Если что-то пойдет не так:

```bash
# 1. Disable mod in game
# 2. Remove symbolic link
rm ~/Library/Application\ Support/Steam/steamapps/common/RimWorld/Mods/RimAsync

# 3. Restart RimWorld
```

---

## 📝 Post-Testing Checklist

После завершения тестирования:

- [ ] Заполнить Performance Metrics template
- [ ] Создать Bug Reports (если есть)
- [ ] Сохранить логи
- [ ] Сделать скриншоты debug overlay
- [ ] Записать общие впечатления

**Отправить результаты:**
```bash
# Собрать все результаты в одну папку
mkdir -p ~/Desktop/RimAsync_Test_Results
cp ~/Desktop/RimAsync_*.txt ~/Desktop/RimAsync_Test_Results/
cp ~/Library/Logs/Unity/Player.log ~/Desktop/RimAsync_Test_Results/Player.log

echo "✅ Test results collected in ~/Desktop/RimAsync_Test_Results/"
```

---

## 🎉 Next Steps

После успешного тестирования:

1. **Update Development Plan** с результатами
2. **Fix найденные баги** (priority)
3. **Optimize** на основе метрик
4. **Prepare for release** если все OK

---

**Good luck with testing! 🚀**

