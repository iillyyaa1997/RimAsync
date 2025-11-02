# 🐛 Critical Bug Fixes - Quick Reference

## What Was Wrong?

### Problem 1: Creating JobDefs at runtime ❌
```csharp
// WRONG - causes NullReferenceException
var job = new JobDef { defName = "Ingest" };
```
**Why:** JobDefs MUST be loaded from XML via DefDatabase, not created programmatically.

### Problem 2: Calling Unity methods from background threads ❌
```csharp
// WRONG - causes threading violations
Task.Run(() => {
    jobTracker.curDriver.DriverTick(); // Unity object!
});
```
**Why:** Unity objects can ONLY be accessed from the main thread.

---

## What Was Fixed? ✅

### Files Changed:
1. **Pawn_AI_Patch.cs**
   - Disabled `GetQuickThought()` - was creating JobDefs
   - Disabled `TryIssueJobPackage_Prefix` - always returns true now

2. **Pawn_PathFollower_Patch.cs**
   - Disabled `GetQuickJob()`, `CreateFoodJob()`, `CreateRestJob()` - were creating JobDefs
   - Disabled `DetermineNextJob_Prefix` - always returns true now
   - Disabled `JobTrackerTick_Prefix` - always returns true now
   - Disabled `ScheduleAsyncJobTick()` - was calling DriverTick() from background thread

### New Files:
- **BUG_ANALYSIS.md** - Detailed technical analysis (English)
- **BUG_FIX_SUMMARY_RU.md** - Summary in Russian
- **BUG_FIX_QUICK.md** - This file (quick reference)

---

## Safe Patterns for RimWorld Mods ✅

### DO:
```csharp
// ✅ Background computation (no Unity objects)
Task.Run(() => {
    var result = ComputeSomething(); // Pure computation
    // Store result for main thread to use later
});

// ✅ Postfix patches (measure, don't interfere)
[HarmonyPostfix]
public static void MyMethod_Postfix() {
    PerformanceMonitor.RecordMetric("Something", 1.0f);
}

// ✅ Get JobDefs from DefDatabase
var jobDef = DefDatabase<JobDef>.GetNamed("Ingest");
```

### DON'T:
```csharp
// ❌ Create Defs programmatically
var def = new JobDef { defName = "Something" };

// ❌ Access Unity objects from background threads
Task.Run(() => pawn.Position); // Will crash!

// ❌ Skip original methods that do initialization
[HarmonyPrefix]
public static bool SpawnSetup_Prefix() {
    return false; // Breaks texture loading!
}
```

---

## Log Evidence

**Before fix:** 617 texture errors + NullReferenceException
```
Could not load Texture2D at 'Terrain/Surfaces/CarpetFine'
MatFrom with null sourceTex.
NullReferenceException: Object reference not set to an instance of an object
  at Verse.KeyBindingDef.get_JustPressed()
```

**After fix:** Should be clean (needs testing)

---

## Next Steps

1. **Test:** Launch RimWorld and verify no crashes
2. **Verify:** Check that texture errors are gone
3. **Refactor:** Use safer async patterns (caching, prediction, postfix optimization)

---

## Core Lesson

RimWorld is Unity-based with a single-threaded game loop. You can't just "make everything async":
- ✅ Async for **computation** (no Unity objects)
- ✅ Main thread for **all game state**
- ✅ Cache results, apply on main thread
- ❌ Don't call game methods from background threads
