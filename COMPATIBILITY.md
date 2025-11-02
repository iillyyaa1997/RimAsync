# RimAsync - Mod Compatibility Guide

**Last Updated:** November 2, 2025
**Version:** 0.1.0 (Development)

## Overview

RimAsync is designed to be compatible with most RimWorld mods. This document provides information about tested mods, known incompatibilities, and recommended load order.

## Compatibility Status

### ✅ Fully Compatible

These mods have been tested and work perfectly with RimAsync:

#### Core Frameworks
- **Harmony** (Required) - Core patching framework
- **RimWorld Multiplayer** - Full support with AsyncTime integration
- **HugsLib** - Mod settings and logging framework

#### UI & QoL Mods
- **Dubs Performance Analyzer** - Works alongside RimAsync for performance monitoring
- **Camera+** - No conflicts with async operations
- **EdB Prepare Carefully** - Colony preparation mod

#### Content Expansion
- **Vanilla Expanded - Core** - Framework for Vanilla Expanded series
- **Vanilla Expanded Framework** - Full compatibility with all VE mods
- **Vanilla Factions Expanded** - All faction mods compatible
- **Vanilla Animals Expanded** - All animal expansion mods compatible
- **Vanilla Plants Expanded** - All plant expansion mods compatible

### ⚠️ Requires Special Configuration

These mods work with RimAsync but may require specific settings or load order:

- **Combat Extended** - Load after RimAsync, ensure async AI is enabled
  - Pathfinding compatibility: ✅
  - Combat calculations: ✅ (runs synchronously when needed)
  - Recommended setting: Enable async pathfinding, keep async AI enabled

### ❌ Known Incompatibilities

These mods cannot be used with RimAsync:

- **RimThreaded** - Direct conflict (both modify threading)
- **MultiThreading (Legacy)** - Direct conflict (both modify threading)

> **Note:** You must choose between RimAsync and other threading mods.

### 🔬 Experimental / Untested

These mods have not been fully tested:

- **RocketMan** - Another performance mod, potential conflicts
- **Performance Optimizer** - May have overlapping optimizations
- **RuntimeGC** - Memory management mod, compatibility unknown

## Load Order

### Recommended Load Order

For optimal compatibility, use this load order:

```
1. Harmony (Required)
2. HugsLib (Recommended)
3. RimWorld Multiplayer (Optional)
4. RimAsync
5. Vanilla Expanded - Core
6. Vanilla Expanded Framework
7. Other Vanilla Expanded mods
8. Combat Extended (if used)
9. Content mods
10. Gameplay mods
11. UI mods
```

### Load Order Rules

- **Always load Harmony first** (after Core)
- **Load RimAsync after core frameworks** (Harmony, HugsLib, Multiplayer)
- **Load RimAsync before content mods** (to ensure patches apply correctly)
- **Load Combat Extended after RimAsync** (CE has special compatibility handling)

## Multiplayer Compatibility

### AsyncTime Integration

RimAsync integrates with RimWorld Multiplayer's **AsyncTime** feature:

- ✅ **Enabled AsyncTime:** Full async performance benefits
- ✅ **Disabled AsyncTime:** Automatic sync mode, no desyncs
- ✅ **Deterministic execution:** All async operations properly synchronized

### Multiplayer Setup

1. Install RimWorld Multiplayer
2. Install RimAsync
3. In Multiplayer settings, enable **AsyncTime**
4. RimAsync will automatically detect and use AsyncTime

### Multiplayer Safety

RimAsync ensures multiplayer safety through:
- Automatic detection of multiplayer mode
- Deterministic execution with AsyncTime
- Fallback to synchronous mode when needed
- No desyncs reported in testing

## Performance Mod Compatibility

### Works With:
- **Dubs Performance Analyzer** - Use together for best results
- **RuntimeGC** - Should work (needs more testing)

### May Conflict:
- **RocketMan** - Both optimize performance, may overlap
- **Performance Optimizer** - Similar optimizations

### Definitely Conflicts:
- **RimThreaded** - Choose one or the other
- **MultiThreading** - Choose one or the other

## Testing Your Mod List

### Automatic Compatibility Check

RimAsync includes a built-in compatibility checker:

1. Load your mod list
2. Start RimWorld
3. Check the log for RimAsync compatibility report
4. Look for warnings about incompatible mods

### Manual Testing Checklist

When testing RimAsync with new mods:

- [ ] Game loads without errors
- [ ] Pawns pathfind correctly
- [ ] AI decisions work as expected
- [ ] Buildings construct properly
- [ ] No errors in Hugslib log (Ctrl+F12)
- [ ] In multiplayer: No desyncs after 10+ minutes

### Reporting Compatibility Issues

Found a compatibility issue? Please report it with:

1. **Mod list** (use RimPy mod manager export)
2. **Load order** (screenshot of mod list)
3. **Error logs** (Hugslib: Ctrl+F12)
4. **Multiplayer status** (AsyncTime on/off)
5. **Steps to reproduce** the issue

## Combat Extended Specific Guide

Combat Extended is one of the most popular overhaul mods. Here's how to use it with RimAsync:

### Load Order
```
1. Harmony
2. HugsLib
3. RimAsync
4. Combat Extended
5. Combat Extended patches
```

### Recommended Settings
- ✅ Enable async pathfinding (default)
- ✅ Enable async AI (default)
- ⚠️ Disable async job execution if issues occur
- ✅ Enable smart caching (default)

### Known CE Compatibility
- **Pathfinding:** ✅ Fully async compatible
- **Combat calculations:** ✅ Runs sync when needed
- **Ammunition tracking:** ✅ No conflicts
- **Armor system:** ✅ No conflicts
- **Medical system:** ✅ No conflicts

## FAQ

### Q: Can I use RimAsync with RimThreaded?
**A:** No, they are incompatible. Choose one or the other.

### Q: Do I need Multiplayer mod to use RimAsync?
**A:** No, RimAsync works in single-player. Multiplayer is optional.

### Q: Will RimAsync break my save?
**A:** No, RimAsync is safe to add/remove. However, removing it may affect performance.

### Q: My favorite mod isn't listed. Is it compatible?
**A:** Most likely yes! If a mod isn't listed as incompatible, it should work fine.

### Q: RimAsync and [other mod] both patch the same method. What happens?
**A:** Harmony handles this automatically. In most cases, both patches will work together.

### Q: How do I know if RimAsync is working?
**A:** Press F11 in-game to see the debug overlay with performance metrics.

## Contributing

Help improve compatibility documentation:

1. Test RimAsync with your favorite mods
2. Report results (compatible/incompatible)
3. Share recommended settings
4. Submit pull requests with updates

## Version History

### 0.1.0 (Development)
- Initial compatibility testing framework
- Tested with top 20 most popular mods
- Multiplayer integration complete
- Combat Extended compatibility verified

---

**Note:** This is a living document. Compatibility may change with updates to RimWorld, RimAsync, or other mods.
