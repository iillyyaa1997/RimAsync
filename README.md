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
- **🤝 Mod Friendly** - Designed for broad compatibility

## Performance Improvements

- **Asynchronous pathfinding** - Pawns find paths without blocking gameplay
- **Background job processing** - Work continues while you plan
- **Smart caching** - Intelligent optimization of frequent calculations
- **Memory optimization** - Reduced stuttering and smoother performance

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

## Multiplayer Support

RimAsync uses the **AsyncTime setting** in RimWorld Multiplayer to enable safe background processing without affecting synchronization. This allows performance improvements while maintaining perfect multiplayer stability.

**Setup for multiplayer:**
- Install RimWorld Multiplayer
- Enable AsyncTime in Multiplayer settings
- RimAsync automatically detects and adapts

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