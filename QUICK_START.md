# RimAsync - Quick Start for Testing

**🎯 Цель:** Быстро запустить мод в игре для тестирования

## ⚡ Быстрый старт (5 минут)

### 1. Компиляция
```bash
cd /Users/ilyavolkov/Workspace/RimAsync
make build
```

### 2. Установка (Symbolic Link)
```bash
ln -s /Users/ilyavolkov/Workspace/RimAsync \
  ~/Library/Application\ Support/Steam/steamapps/common/RimWorld/Mods/RimAsync
```

### 3. Запуск RimWorld
1. Открыть RimWorld через Steam
2. Mods → Включить **RimAsync**
3. Restart игра

### 4. Базовая проверка
- **Settings:** Options → Mod Settings → RimAsync
- **Debug:** Нажми **F11** в игре для overlay
- **Play:** Создай колонию и играй!

## 🧪 Минимальный тест (10 минут)

1. Создать колонию (3-5 pawns)
2. Нажать **F11** (debug overlay должен появиться)
3. Дать pawn'ам приказы на перемещение
4. Построить 1-2 здания
5. Проверить TPS в overlay (должен быть 55-60)

**Если всё работает - мод готов! ✅**

## 📋 Детальное тестирование

Смотри: **REAL_TESTING_GUIDE.md** (30+ минут)

---

**Логи:** `~/Library/Logs/Unity/Player.log`  
**Desyncs:** `~/Library/Application Support/RimWorld/MpDesyncs/`
