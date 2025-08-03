# Analyze Logs Command

**Command:** `@analyze-logs`
**Description:** Comprehensive analysis of RimWorld logs, Docker containers, and multiplayer desync files

## 🎯 Usage

### Basic Usage
```
@analyze-logs                    # Analyze all available logs
@analyze-logs --desyncs         # Focus on multiplayer desync analysis
@analyze-logs --performance     # Focus on performance issues
@analyze-logs --errors          # Focus on error patterns
```

### Advanced Usage
```
@analyze-logs --docker          # Analyze Docker container logs
@analyze-logs --rimworld        # Analyze RimWorld game logs
@analyze-logs --last 24h        # Analyze logs from last 24 hours
@analyze-logs --export          # Export analysis to file
```

## 🔧 What the command does

1. **Scans log directories** for RimWorld, Docker, and system logs
2. **Identifies error patterns** and performance bottlenecks
3. **Analyzes multiplayer desyncs** for AsyncTime compatibility
4. **Generates actionable recommendations** for fixing issues
5. **Exports detailed reports** for debugging and optimization
6. **Tracks trends** in errors and performance over time

## 📁 Log Sources

### 🎮 RimWorld Logs
```
~/Library/Application Support/RimWorld/
├── Player.log (main game log)
├── Player-prev.log (previous session)
├── Config/ (settings and preferences)
└── MpDesyncs/ (multiplayer desync files)
```

### 🐳 Docker Container Logs
```
Docker Containers:
├── rimasync_build (compilation logs)
├── rimasync_test (test execution logs)
├── rimasync_dev (development logs)
└── rimasync_performance (benchmark logs)
```

### 🖥️ System Logs
```
macOS System Logs:
├── /var/log/system.log
├── ~/Library/Logs/
└── Console app integration
```

## 🔍 Analysis Categories

### ❌ Error Analysis (`--errors`)

#### RimAsync Specific Errors
- **Harmony patch failures**
- **AsyncManager initialization issues**
- **Thread synchronization problems**
- **Multiplayer compatibility violations**

#### RimWorld Game Errors
- **Mod loading failures**
- **Save game corruption**
- **Memory allocation issues**
- **Performance degradation**

### 🎮 Multiplayer Desync Analysis (`--desyncs`)

#### Desync Pattern Detection
```
🛡️ Multiplayer Desync Analysis:

📊 Desync Statistics:
├── Total Desyncs: 3 files
├── AsyncTime Related: 0 (0%)
├── Other Mods: 2 (66.7%)
└── Unknown: 1 (33.3%)

🔍 AsyncTime Safety Check:
├── Safe Operations: ✅ 847/847 (100%)
├── Unsafe Operations: ❌ 0/847 (0%)
├── Fallback Triggers: 🔄 23 times
└── Deterministic Execution: ✅ Verified
```

#### Desync File Analysis
```
📁 Recent Desync Files:

1. MpDesync_2025-08-03_14-32-45.log
   ├── Size: 234 KB
   ├── Cause: Mod conflict (not RimAsync)
   ├── Players: 4
   └── Duration: 2h 15m before desync

2. MpDesync_2025-08-03_16-21-12.log
   ├── Size: 89 KB
   ├── Cause: Save game issue
   ├── Players: 2
   └── RimAsync Status: ✅ Safe mode active
```

### ⚡ Performance Analysis (`--performance`)

#### TPS Monitoring
```
📈 TPS Performance Trends:

🎯 Colony Performance:
├── Small Colony (5-10 pawns): 60 TPS (Target: 60)
├── Medium Colony (11-50 pawns): 48 TPS (Target: 45)
├── Large Colony (51-100 pawns): 32 TPS (Target: 30)
└── Mega Colony (100+ pawns): 18 TPS (Target: 15)

🧵 Thread Utilization:
├── Main Thread: 78% avg usage
├── Async Threads: 6/8 active (75%)
├── Memory Usage: 2.3 GB (stable)
└── GC Collections: 12/hour (normal)
```

#### Performance Bottlenecks
```
⚠️ Performance Issues Detected:

🐌 Slow Operations:
1. Pathfinding (large maps): 145ms avg (target: <100ms)
   ├── Cause: Complex terrain calculation
   ├── Impact: Medium TPS reduction
   └── Recommendation: Enable async pathfinding

2. Job scheduling: 23ms avg (target: <15ms)
   ├── Cause: Large pawn count
   ├── Impact: Low TPS reduction
   └── Recommendation: Optimize job priority queue
```

### 🐳 Docker Analysis (`--docker`)

#### Container Health
```
🐳 Docker Container Analysis:

📊 Container Status:
├── rimasync_build: ✅ Healthy (0 errors)
├── rimasync_test: ⚠️ Warning (2 timeout issues)
├── rimasync_dev: ✅ Healthy (0 errors)
└── rimasync_performance: ✅ Healthy (0 errors)

🔍 Resource Usage:
├── CPU: 45% avg (8 cores)
├── Memory: 6.2 GB / 16 GB (38%)
├── Disk I/O: 234 MB/s read, 89 MB/s write
└── Network: 12 MB/s (Docker internal)
```

#### Build Issues
```
❌ Docker Build Issues:

1. NuGet package restore timeout
   ├── Frequency: 3 times in last 24h
   ├── Duration: 5-10 minutes each
   ├── Cause: Network connectivity
   └── Solution: Update Docker registry mirror

2. Test environment initialization slow
   ├── Frequency: Consistent
   ├── Duration: 45 seconds (target: <20s)
   ├── Cause: Mock assembly loading
   └── Solution: Optimize test setup
```

## 📊 Report Generation

### 🗂️ Analysis Report Structure

#### Summary Report
```
🔍 RimAsync Log Analysis Report
📅 Period: 2025-08-03 00:00 - 2025-08-03 23:59

📊 Executive Summary:
├── ✅ Overall Health: Good (87/100)
├── ❌ Critical Issues: 0
├── ⚠️ Warnings: 3
└── 📈 Performance: Above target

🎯 Key Findings:
1. AsyncTime integration working correctly (0 desyncs)
2. Performance targets met for all colony sizes
3. Minor Docker timeout issues (non-critical)
4. No RimAsync-related errors detected

📋 Recommendations:
1. Update Docker registry configuration
2. Optimize test environment initialization
3. Monitor large colony performance trends
```

#### Detailed Technical Report
```
🔧 Technical Analysis Details:

📊 Error Breakdown:
├── Total Errors: 12
├── Critical: 0 (0%)
├── High: 1 (8.3%)
├── Medium: 4 (33.3%)
├── Low: 7 (58.4%)

🎮 RimWorld Integration:
├── Mod Load Success: ✅ 100%
├── Harmony Patches: ✅ 34/34 applied
├── Settings Loading: ✅ Success
├── Game Component: ✅ Initialized

🧵 Threading Analysis:
├── Thread Safety: ✅ No violations detected
├── Deadlock Detection: ✅ No deadlocks
├── Resource Leaks: ✅ No leaks detected
├── Performance Impact: +23.4% TPS improvement
```

### 📈 Trend Analysis

#### Historical Performance
```
📊 Performance Trends (7 days):

TPS Improvements:
Day 1: +18.2% (baseline established)
Day 2: +19.7% (stable improvement)
Day 3: +21.1% (optimization effects)
Day 4: +22.8% (peak performance)
Day 5: +23.4% (current stable state)
Day 6: +23.1% (minor regression)
Day 7: +23.4% (recovered)

Trend: ↗️ Steadily improving (+5.2% over week)
```

#### Error Patterns
```
❌ Error Frequency (7 days):

Critical Errors: [0,0,0,0,0,0,0] ✅ Stable
High Priority: [2,1,0,1,0,0,1] ✅ Decreasing
Medium Priority: [8,6,4,3,4,2,4] 📈 Improving
Low Priority: [15,12,9,8,7,6,7] 📈 Improving

Overall Trend: ↘️ Errors decreasing (-53% over week)
```

## 🔧 Command Options

### Filter Options
- `--errors` - Focus on error analysis
- `--desyncs` - Multiplayer desync analysis
- `--performance` - Performance bottleneck analysis
- `--docker` - Docker container analysis

### Time Range Options
- `--last 1h` - Last hour
- `--last 24h` - Last 24 hours (default)
- `--last 7d` - Last 7 days
- `--since "2025-08-03"` - Since specific date

### Output Options
- `--verbose` - Detailed technical output
- `--summary` - Executive summary only
- `--export` - Export to file
- `--format json|html|text` - Output format

## 🚀 Usage Examples

### Quick Health Check
```
@analyze-logs --summary

📊 RimAsync Health Check:
├── ✅ Overall Status: Healthy
├── ❌ Critical Issues: 0
├── ⚠️ Warnings: 2 (minor)
└── 📈 Performance: +23.4% TPS
```

### Desync Investigation
```
@analyze-logs --desyncs --verbose

🔍 Detailed Desync Analysis:
├── 📁 3 desync files found
├── 🛡️ 0 AsyncTime related (100% safe)
├── 📊 Analysis complete
└── 📋 Recommendations generated
```

### Performance Troubleshooting
```
@analyze-logs --performance --last 7d

📈 7-Day Performance Analysis:
├── 📊 Trend analysis complete
├── 🎯 All targets met
├── ⚠️ 2 minor bottlenecks identified
└── 📋 Optimization suggestions provided
```

---

**Note:** This command automatically detects log locations on macOS and provides actionable insights for debugging and optimization.
