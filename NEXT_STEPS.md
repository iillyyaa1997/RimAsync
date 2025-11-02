# Next Steps - RimAsync Development

**Date:** 2 November 2025
**Current Status:** ✅ RimWorld 1.6 Compatible (Core functionality) - 0 errors on launch
**Target Version:** RimWorld 1.6.4630+ ONLY

> ⚠️ **Development Focus:** RimAsync is now developed EXCLUSIVELY for RimWorld 1.6. RimWorld 1.5 support discontinued.

---

## 🎯 Priority Queue

### 🔴 **CRITICAL - Do This Week**

#### 1. Research RimWorld 1.6 API Changes (2-4 hours)
**Goal:** Find replacements for changed APIs

**Tasks:**
- [ ] Research `GameComponent` replacement in RimWorld 1.6
  - Check if it's `Verse.GameComponent` instead of `RimWorld.GameComponent`
  - Or if there's a new base class for mod components
  - Document findings in `docs/RimWorld_1.6_API_Changes.md`
- [ ] Research `Building.Tick()` method signature
  - Find new method signature or alternative hook
  - Test with simple patch
- [ ] Create test implementation
  - Minimal GameComponent test
  - Minimal Building_Patch test

**Success Criteria:**
- ✅ GameComponent replacement identified and tested
- ✅ Building.Tick() alternative found
- ✅ Test patches compile and run without errors

**Command to execute:** `@execute-task Research RimWorld 1.6 API changes`

---

#### 2. Rewrite Pawn Patches (Main-Thread Safe) (4-6 hours)
**Goal:** Remove `Task.Run()` and make patches thread-safe

**Current Problem:**
```csharp
// ❌ UNSAFE - calls Unity from background thread
Task.Run(() => {
    pawn.jobs.jobTracker.curDriver.DriverTick(); // CRASH!
});
```

**Solution:**
```csharp
// ✅ SAFE - schedule on main thread
TickManager.QueueMainThreadAction(() => {
    pawn.jobs.jobTracker.curDriver.DriverTick(); // OK
});
```

**Tasks:**
- [ ] Remove all `Task.Run()` from Pawn_AI_Patch.cs
- [ ] Remove all `Task.Run()` from Pawn_PathFollower_Patch.cs
- [ ] Implement main-thread scheduling system
- [ ] Add comprehensive tests
- [ ] Verify no threading violations

**Success Criteria:**
- ✅ No `Task.Run()` calling Unity methods
- ✅ All tests pass
- ✅ No crashes during 1+ hour gameplay

**Command to execute:** `@execute-task Rewrite Pawn patches main-thread safe`

---

### 🟠 **IMPORTANT - Do Next Week**

#### 3. Real In-Game Performance Testing (2-3 hours)
**Goal:** Measure actual TPS improvements in real gameplay

**Test Scenarios:**
- [ ] Small colony (10 pawns) - baseline
- [ ] Medium colony (30 pawns) - standard game
- [ ] Large colony (50+ pawns) - performance test
- [ ] Mega colony (100+ pawns) - stress test

**Measurements:**
- [ ] Average TPS (target: +15-30% improvement)
- [ ] Min TPS (no drops below 30 TPS)
- [ ] Stability (2+ hours without crashes)

**Success Criteria:**
- ✅ +15% TPS improvement minimum
- ✅ No crashes in 2+ hour session
- ✅ Smooth gameplay feel

---

#### 4. Mod Compatibility Testing (3-4 hours)
**Goal:** Verify compatibility with popular mods

**Test Mods:**
- [ ] Combat Extended - combat mechanics
- [ ] Vanilla Expanded (any series) - content mods
- [ ] HugsLib - mod framework
- [ ] Rocketman - performance mod
- [ ] Performance Optimizer - another perf mod

**Success Criteria:**
- ✅ No crashes with tested mods
- ✅ No obvious conflicts
- ✅ Update COMPATIBILITY.md with findings

---

#### 5. Multiplayer + AsyncTime Testing (2-3 hours)
**Goal:** Verify multiplayer safety

**Test Scenarios:**
- [ ] 2-player session (1+ hour)
- [ ] Desync monitoring
- [ ] AsyncTime stress test
- [ ] Performance in multiplayer

**Success Criteria:**
- ✅ No desyncs
- ✅ Performance improvements in multiplayer
- ✅ AsyncTime working correctly

---

### 🟡 **NICE TO HAVE - Following Week**

#### 6. Documentation Updates (2-3 hours)
**Tasks:**
- [ ] Update README.md with RimWorld 1.6 notes
- [ ] Update COMPATIBILITY.md
- [ ] Create docs/RimWorld_1.6_API_Changes.md
- [ ] Update quickstart guide

---

#### 7. Alpha Release Preparation (2-3 hours)
**Tasks:**
- [ ] Create GitHub release v0.5.0-alpha
- [ ] Write release notes
- [ ] Prepare Steam Workshop submission
- [ ] Setup community beta testing

---

## 📅 Timeline

### Week 1 (2-9 Nov 2025):
- ✅ Day 1: RimWorld 1.6 compatibility fixed (DONE)
- Day 2-3: Research RimWorld 1.6 API changes
- Day 4-5: Rewrite Pawn patches (main-thread safe)
- Day 6-7: Initial testing

### Week 2 (10-16 Nov 2025):
- Day 1-2: In-game performance testing
- Day 3-4: Mod compatibility testing
- Day 5: Multiplayer testing
- Day 6-7: Bug fixes

### Week 3 (17-23 Nov 2025):
- Day 1-2: Documentation updates
- Day 3-4: Alpha release preparation
- Day 5-7: Community beta testing

### Week 4 (24-30 Nov 2025):
- Beta feedback integration
- Final polishing
- Public release preparation

---

## 🚀 Quick Commands

```bash
# Development
make deploy              # Build + Install in one command
make build              # Only build
make test               # Run all tests

# Testing
make coverage           # Test coverage report
make coverage-html      # HTML coverage report

# Cleanup
make clean              # Clean build artifacts
make clean-all          # Deep clean
```

---

## 📊 Current Metrics

**Code:**
- 54 KB DLL (RimWorld 1.6 compatible)
- ~10,000 lines of code
- 255 unit tests (100% passing)

**Functionality:**
- ✅ Core async system (100%)
- ✅ Settings UI (100%)
- ✅ TickManager optimization (100%)
- ⏳ Pawn AI optimization (0% - needs rewrite)
- ⏳ Building optimization (0% - needs API research)
- ⏳ GameComponent (0% - needs API research)

**Performance:**
- Current: ~5-10% TPS improvement
- Target: 20-35% TPS improvement
- Gap: Need to restore Pawn AI and Building patches

---

## 📝 Notes

### What's Working:
- ✅ Mod loads without errors
- ✅ Core async infrastructure
- ✅ Settings and configuration
- ✅ Debug overlay (F11)
- ✅ Thread limit calculator
- ✅ Smart caching

### What Needs Work:
- ⏳ RimWorld 1.6 API compatibility (research needed)
- ⏳ Pawn patches (rewrite needed)
- ⏳ Real-world testing (not yet done)
- ⏳ Community feedback (not yet available)

---

**See also:**
- [Development_Plan.md](Planning/Development_Plan.md) - Full roadmap
- [KNOWN_ISSUES.md](KNOWN_ISSUES.md) - Known limitations
- [RIMWORLD_1.6_COMPATIBILITY_FIX.md](RIMWORLD_1.6_COMPATIBILITY_FIX.md) - What was fixed
