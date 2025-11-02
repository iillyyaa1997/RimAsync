# Known Issues - RimAsync

**Last Updated:** 2 November 2025
**Target Version:** RimWorld 1.6.4630+ ONLY

> ⚠️ **IMPORTANT:** RimAsync is developed exclusively for RimWorld 1.6. RimWorld 1.5 support has been discontinued.

## 🔴 Critical Issues

### RimWorld 1.6 API Compatibility

**Status:** ⏳ In Progress - Research needed

**Issue:** Several components are temporarily disabled due to RimWorld 1.6 API changes.

**Affected Components:**
1. **GameComponent System** - `RimWorld.GameComponent` not found in RimWorld 1.6
   - Impact: No save/load persistence, no GameComponent hooks
   - Workaround: Core functionality works without it
   - Fix ETA: Research in progress

2. **Building_Patch** - `Building.Tick()` method signature changed
   - Impact: Building-specific optimizations disabled
   - Workaround: TickManager optimization still active
   - Fix ETA: API research needed

3. **Pawn AI Patches** - Unsafe threading violations
   - Impact: Pawn AI optimizations disabled
   - Reason: `Task.Run()` calls Unity methods from background threads (causes crashes)
   - Fix ETA: Complete rewrite needed (main-thread safe implementation)

4. **Multiplayer Patches** - Depends on GameComponent
   - Impact: Some multiplayer-specific optimizations disabled
   - Workaround: Core multiplayer compatibility maintained
   - Fix ETA: After GameComponent restoration

**Current Status:**
- ✅ Mod loads successfully (0 errors)
- ✅ Core async functionality works (54 KB DLL)
- ✅ Performance optimizations active (TickManager)
- ⏳ Advanced features pending API research

**References:**
- [RIMWORLD_1.6_COMPATIBILITY_FIX.md](RIMWORLD_1.6_COMPATIBILITY_FIX.md) - Full diagnostic report
- [Development_Plan.md](Planning/Development_Plan.md) - Roadmap for restoration

---

## 🟡 Known Limitations

### 1. Performance Improvements

**Issue:** Current performance improvements are limited to TickManager optimization only.

**Reason:**
- Building, Pawn AI, and Pathfinding patches are disabled (see above)
- Main performance boost comes from TickManager optimization

**Expected Performance:**
- ✅ TickManager: +5-10% TPS improvement (active)
- ⏳ Pawn AI: +10-15% TPS improvement (disabled)
- ⏳ Building: +5-10% TPS improvement (disabled)
- **Current Total:** ~5-10% TPS (vs. planned 20-35%)

**Fix ETA:** After RimWorld 1.6 API research complete

### 2. Debug Overlay Limitations

**Issue:** F11 debug overlay shows limited data without GameComponent.

**Current Display:**
- ✅ TPS metrics
- ✅ Settings values
- ⏳ Cache statistics (limited without GameComponent hooks)
- ⏳ Async operations count (limited)

**Workaround:** Basic metrics still functional

**Fix ETA:** After GameComponent restoration

### 3. Multiplayer AsyncTime Integration

**Issue:** Some AsyncTime features limited without multiplayer patches.

**Current Status:**
- ✅ AsyncTime detection works
- ✅ Basic multiplayer compatibility maintained
- ⏳ Advanced async operations in multiplayer limited

**Workaround:** Mod safe to use in multiplayer (no desyncs)

**Fix ETA:** After GameComponent and multiplayer patches restoration

---

## 🟢 Non-Issues (Working as Intended)

### 1. .NET 4.7.2 Compatibility

**Status:** ✅ RESOLVED

**Previous Issue:** Code used .NET 5+ APIs incompatible with RimWorld.

**Solution Applied:**
- `Math.Clamp` → `Math.Max(min, Math.Min(value, max))`
- `String.Contains(StringComparison)` → `IndexOf(...) >= 0`

**Result:** Fully compatible with RimWorld's .NET 4.7.2 runtime

### 2. Build Pipeline

**Status:** ✅ WORKING

**Command:** `make deploy` - builds and installs in one step

**Components:**
- Docker build with real RimWorld libraries ✅
- Automatic installation to game mods folder ✅
- Version management (1.5 & 1.6) ✅

---

## 📋 Workarounds

### For Missing Pawn AI Optimizations

**Use these alternative performance mods:**
- [Rocketman](https://steamcommunity.com/sharedfiles/filedetails/?id=2479389928) - Pawn AI caching
- [Performance Optimizer](https://steamcommunity.com/sharedfiles/filedetails/?id=2664723367) - General optimizations

**Note:** RimAsync is compatible with these mods.

### For Missing Building Optimizations

**Recommendations:**
- Limit active construction projects to 5-10 per colony
- Use "Priority" mod to manage work orders efficiently
- RimAsync's TickManager optimization still helps overall performance

---

## 🔍 Reporting New Issues

Found a new issue? Please report it with:

1. **RimWorld Version** (1.5 or 1.6)
2. **Mod List** (full list of active mods)
3. **Player.log** location:
   - Windows: `%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`
   - Mac: `~/Library/Logs/Unity/Player.log`
   - Linux: `~/.config/unity3d/Ludeon Studios/RimWorld by Ludeon Studios/Player.log`
4. **Steps to reproduce**
5. **Expected vs actual behavior**

**Where to report:**
- GitHub Issues: [Create Issue](https://github.com/yourusername/RimAsync/issues/new)
- Steam Workshop: Comments section (after release)
- Discord: RimWorld Multiplayer server (after release)

---

## 📅 Fix Timeline

### Immediate (This Week):
1. Research RimWorld 1.6 GameComponent API (2-4 hours)
2. Research Building.Tick() changes (1-2 hours)
3. Create test implementation

### Short-term (1-2 Weeks):
1. Restore GameComponent functionality
2. Restore Building_Patch with new API
3. Rewrite Pawn patches (main-thread safe)
4. Full in-game testing

### Medium-term (3-4 Weeks):
1. Restore all advanced features
2. Performance testing (target: +20-35% TPS)
3. Multiplayer stress testing
4. Community beta testing

### Long-term (1-2 Months):
1. Steam Workshop release
2. Community feedback integration
3. Additional optimizations
4. RimWorld 1.7 preparation (if released)

---

**See also:**
- [Development Plan](Planning/Development_Plan.md) - Full roadmap
- [RIMWORLD_1.6_COMPATIBILITY_FIX.md](RIMWORLD_1.6_COMPATIBILITY_FIX.md) - Technical details
- [COMPATIBILITY.md](COMPATIBILITY.md) - Mod compatibility information
