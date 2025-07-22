# Analyze Logs Command

**Команда:** `@analyze-logs`  
**Описание:** Анализировать логи ошибок RimAsync и RimWorld для диагностики проблем

## 🎯 Использование

```
@analyze-logs [options]
```

**Примеры:**
```
@analyze-logs                      # Анализ всех доступных логов
@analyze-logs --docker             # Только Docker логи
@analyze-logs --rimworld           # Только RimWorld логи
@analyze-logs --desyncs            # Только desync логи Multiplayer
@analyze-logs --compilation        # Только ошибки компиляции
@analyze-logs --recent             # Только недавние логи (последние 24 часа)
@analyze-logs --errors-only        # Только ошибки, без warnings
```

## 📁 Анализируемые источники логов

### 🔴 RimWorld Multiplayer Desyncs
**Путь:** `/Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs`
- Анализирует desync логи
- Выявляет проблемы синхронизации  
- Проверяет совместимость с AsyncTime
- Идентифицирует проблемные методы

### 🐳 Docker Container Logs
```bash
docker-compose logs build    # Логи компиляции
docker-compose logs test     # Логи тестирования
docker-compose logs dev      # Логи разработки
```

### 🎮 RimWorld Player.log
**Пути:**
- macOS: `~/Library/Logs/RimWorld by Ludeon Studios/Player.log`
- Windows: `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`

### 📊 RimAsync Specific Logs
- `./TestResults/` - Результаты тестов
- `./Build/` - Логи компиляции
- Performance metrics из PerformanceMonitor

## 🔧 Типы анализа

### 1. 🚨 Critical Error Analysis
```
[CRITICAL] NullReferenceException in AsyncManager
[CRITICAL] Harmony patch conflict detected
[CRITICAL] Multiplayer desync detected
```

### 2. ⚠️ Performance Issues
```
[PERFORMANCE] TPS drop detected: 45 → 12
[PERFORMANCE] Memory leak in SmartCache
[PERFORMANCE] Thread contention detected
```

### 3. 🔄 Multiplayer Compatibility
```
[MULTIPLAYER] AsyncTime not enabled
[MULTIPLAYER] Desync in Pawn_PathFollower_Patch
[MULTIPLAYER] Determinism violation detected
```

### 4. 🐳 Docker Build Issues
```
[DOCKER] Compilation failed: missing reference
[DOCKER] Test timeout: AsyncManagerTests
[DOCKER] Container volume mount issue
```

## 📋 Output Format

### Структурированный отчет:
```
🔍 RimAsync Log Analysis Report
===============================
📅 Analysis Date: 2025-07-20 15:30:45
🕐 Time Range: Last 24 hours
📊 Total Logs Analyzed: 156 files

🚨 CRITICAL ISSUES (3):
┌─────────────────────────────────────────────────
│ [15:25:32] NullReferenceException in AsyncManager.StartAsyncOperation()
│ File: AsyncManager.cs:247
│ Stack: AsyncManager.StartAsyncOperation() → Job.StartJob()
│ Impact: HIGH - Async operations failing
│ Solution: Add null check before operation
├─────────────────────────────────────────────────
│ [15:20:15] Multiplayer Desync detected
│ File: /Users/ilyavolkov/Library/Application Support/RimWorld/MpDesyncs/desync_2025-07-20_15-20-15.log
│ Method: Pawn_PathFollower.PatherTick()  
│ Impact: CRITICAL - Multiplayer incompatible
│ Solution: Review AsyncTime integration
└─────────────────────────────────────────────────

⚠️ PERFORMANCE WARNINGS (5):
┌─────────────────────────────────────────────────
│ [15:18:45] TPS Performance Drop
│ Before: 60 TPS → After: 25 TPS
│ Cause: SmartCache lock contention
│ Impact: MEDIUM - Noticeable stuttering
│ Solution: Optimize cache locking strategy
└─────────────────────────────────────────────────

🐳 DOCKER ISSUES (2):
┌─────────────────────────────────────────────────
│ [15:30:12] Docker Build Failed
│ Service: rimasync-build
│ Error: CS0246: Type 'Verse.Pawn' not found
│ Impact: LOW - Missing RimWorld references
│ Solution: Update Dockerfile with RimWorld libs
└─────────────────────────────────────────────────

✅ SUCCESSFUL OPERATIONS (12):
- AsyncManager initialization: OK
- Harmony patches applied: 8/8 OK
- Performance monitor: Active
- Multiplayer detection: Working

📈 TRENDS:
- Error frequency: ↑ 25% vs yesterday
- Docker build success rate: 85%
- Test pass rate: 92% (Unit), 78% (Integration)
- Performance: TPS average 45 (target: 60)

🎯 RECOMMENDED ACTIONS:
1. URGENT: Fix AsyncManager null reference (AsyncManager.cs:247)
2. HIGH: Review Pawn_PathFollower_Patch for multiplayer compatibility  
3. MEDIUM: Optimize SmartCache locking to reduce TPS drops
4. LOW: Update Docker references for RimWorld assemblies

📊 STATISTICS:
- Total Errors: 3 critical, 5 warnings
- Most Frequent Error: NullReferenceException (40%)
- Peak Error Time: 15:20-15:30 (8 errors)
- Clean Period: 14:00-15:00 (0 errors)
```

## 🛠️ Advanced Options

### Custom Time Range
```
@analyze-logs --from "2025-07-20 10:00" --to "2025-07-20 16:00"
```

### Specific Error Types
```
@analyze-logs --filter "NullReferenceException,AsyncManager"
@analyze-logs --filter "Multiplayer,Desync"
@analyze-logs --filter "Performance,TPS"
```

### Export Options
```
@analyze-logs --export-json ./logs-analysis.json
@analyze-logs --export-csv ./logs-analysis.csv  
@analyze-logs --export-html ./logs-report.html
```

## 🔄 Automatic Analysis

### Scheduled Analysis
Команда может быть настроена для автоматического запуска:
- После каждой компиляции (`@execute-task`)
- После тестирования (`@run-tests`)
- При обнаружении новых логов

### Integration with Development Workflow
```
@execute-task → build → test → @analyze-logs --auto
```

## 📍 Log File Locations Reference

### RimWorld Core Logs
```
macOS:
- Player.log: ~/Library/Logs/RimWorld by Ludeon Studios/
- Multiplayer Desyncs: ~/Library/Application Support/RimWorld/MpDesyncs/
- Mod Configs: ~/Library/Application Support/RimWorld/Config/

Windows:  
- Player.log: %USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\
- Multiplayer Desyncs: %USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\MpDesyncs\
```

### RimAsync Project Logs
```
./TestResults/TestResults.trx    # NUnit test results
./Build/build.log               # Compilation logs
./docker-compose.logs           # Docker container logs
```

## 🚀 Quick Diagnostic Commands

### Emergency Analysis (when game crashes)
```
@analyze-logs --emergency --last-10-minutes
```

### Pre-Release Validation
```
@analyze-logs --validate-release --full-scan
```

### Performance Regression Detection
```
@analyze-logs --performance-compare --baseline yesterday
```

## 🎯 Error Categories

### 🔴 Critical (Requires Immediate Action)
- NullReferenceException in core systems
- Multiplayer desyncs
- Complete build failures
- Game crashes

### 🟠 High Priority
- Performance degradation > 25%
- Failed integration tests
- Harmony patch conflicts
- Memory leaks

### 🟡 Medium Priority  
- Minor performance issues
- Non-critical test failures
- Docker build warnings
- Deprecated API usage

### 🟢 Low Priority
- Code style warnings
- Optimization opportunities
- Documentation issues
- Non-blocking suggestions

---

**Примечание:** Команда автоматически создает backup анализов и отслеживает тренды ошибок для выявления регрессий. 