# RimAsync

**Performance optimization mod with full multiplayer support**

[![RimWorld Version](https://img.shields.io/badge/RimWorld-1.5+-blue.svg)](https://rimworldgame.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Multiplayer Compatible](https://img.shields.io/badge/Multiplayer-Compatible-brightgreen.svg)](https://github.com/rwmt/Multiplayer)

## What is RimAsync?

RimAsync brings **asynchronous performance improvements** to RimWorld while maintaining full compatibility with multiplayer. The first performance mod that works seamlessly with [RimWorld Multiplayer](https://github.com/rwmt/Multiplayer).

## Key Features

- **🎯 Multiplayer Compatible** - Works with RimWorld Multiplayer using AsyncTime
- **⚡ Performance Boost** - Significant TPS improvements on large colonies
- **🛡️ Safe & Stable** - Automatic fallbacks and extensive testing
- **📊 Debug Overlay** - Real-time performance monitoring (F11 to toggle)
- **🔧 Auto Thread Limits** - Smart CPU-based thread optimization
- **🤝 Mod Friendly** - Designed for broad compatibility

## Performance Improvements

- **Asynchronous pathfinding** - Pawns find paths without blocking gameplay
- **Background job processing** - Work continues while you plan
- **AI optimization** - Asynchronous thinking and decision-making
- **Building operations** - Non-blocking construction updates
- **Smart caching** - Intelligent optimization of frequent calculations with LRU eviction
- **Memory optimization** - Reduced stuttering and smoother performance
- **Automatic thread tuning** - Adapts to your CPU (1-256 cores supported)

## Installation

### Requirements
- **RimWorld 1.5+**
- **[Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)** (required)
- **[RimWorld Multiplayer](https://github.com/rwmt/Multiplayer)** (optional, for multiplayer)

### Steps
1. Subscribe on Steam Workshop *(coming soon)*
2. Enable in mod list (load after Harmony)
3. **For multiplayer:** Enable AsyncTime in Multiplayer settings
4. Enjoy improved performance!

## Development with Docker

**RimAsync uses Docker for all compilation and testing operations.**

### Prerequisites
- **Docker** and **Docker Compose** installed
- No need for local .NET SDK installation!

### 📋 Quick Commands (Makefile)
```bash
# Build the mod
make build

# Run tests
make test

# Development environment
make dev

# Quick development build
make quick-build

# Show all available commands
make help
```

### 🔧 Alternative (Raw Docker)
```bash
# Build the mod
docker-compose up build

# Run tests
docker-compose up test

# Development environment
docker-compose up dev

# Quick development build
docker-compose up quick-build

# Production release build
docker-compose up release
```

### Output Directories
- `./Build/` - Development builds
- `./Release/` - Production builds ready for Steam Workshop
- `./TestResults/` - Test reports and logs

### Testing

RimAsync has comprehensive test coverage:

```bash
# Run all tests
make test

# Run specific test categories
docker-compose run --rm test dotnet test --filter Category=Unit
docker-compose run --rm test dotnet test --filter Category=Integration
docker-compose run --rm test dotnet test --filter Category=Performance
```

**Test Statistics:**
- 225+ tests with 99.6% pass rate
- Unit tests for all core components
- Integration tests for patch interactions
- Performance benchmarks for async operations
- Mock RimWorld environment for isolated testing

## Multiplayer Support

RimAsync uses the **AsyncTime setting** in RimWorld Multiplayer to enable safe background processing without affecting synchronization. This allows performance improvements while maintaining perfect multiplayer stability.

**Setup for multiplayer:**
- Install RimWorld Multiplayer
- Enable AsyncTime in Multiplayer settings
- RimAsync automatically detects and adapts

## Debug Overlay

Press **F11** in-game to toggle the debug overlay, which displays:

- **TPS (Ticks Per Second)** - Current game performance
- **Cache Statistics** - Hit rate, misses, evictions
- **Async Operations** - Active background tasks
- **Thread Status** - Current thread utilization
- **Settings** - Active configuration

The overlay helps monitor performance and diagnose issues in real-time.

## Settings

RimAsync provides extensive configuration options:

### Performance Settings
- **Async Pathfinding** - Enable background path calculation
- **Async AI** - Enable background AI processing
- **Async Job Execution** - Enable background job processing
- **Async Building** - Enable background building operations
- **Smart Caching** - Enable intelligent result caching

### Thread Management
- **Auto Thread Limits** *(Recommended)* - Automatically calculates optimal thread count based on your CPU
- **Manual Thread Limit** - Set custom thread count (1-16)

### Safety Settings
- **Fallback Mechanisms** - Automatic sync fallback on errors
- **Performance Monitoring** - Track operation metrics

## Compatibility

### ✅ Compatible
- RimWorld Multiplayer
- Most performance and UI mods
- Vanilla-friendly mods

### ❌ Incompatible
- Other threading/performance mods
- Mods that heavily modify tick loops

## Support

**Found an issue?** Please report with:
- RimWorld version
- Mod list (use [RimPy](https://github.com/rimpy-custom/RimPy))
- Error logs (HugsLib: Ctrl+F12)
- Multiplayer status (AsyncTime on/off)

## License

This project is licensed under the MIT License - see [LICENSE](LICENSE) for details.

## Acknowledgments

- **RimWorld Multiplayer Team** - For the multiplayer framework and AsyncTime
- **Ludeon Studios** - For RimWorld
- **Community** - For testing and feedback

---

**Made for the RimWorld community** ❤️ 