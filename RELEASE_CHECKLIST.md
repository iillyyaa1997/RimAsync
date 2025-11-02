# RimAsync - Release Checklist

**Version:** 0.1.0 (Development → Alpha)
**Target Date:** TBD
**Release Type:** Alpha / Beta / Stable

## Pre-Release Checklist

### 🔧 Code Quality

- [ ] **All tests passing** (255/255 ✅)
  ```bash
  make test
  ```

- [ ] **No linter errors**
  ```bash
  make lint
  ```

- [ ] **Code formatted**
  ```bash
  make format-check
  ```

- [ ] **Coverage requirements met** (>85%)
  ```bash
  make coverage
  ```

### 🏗️ Build & Compilation

- [ ] **Clean build successful**
  ```bash
  make clean && make build
  ```

- [ ] **Release build successful**
  ```bash
  docker-compose up release
  ```

- [ ] **DLL size reasonable** (< 5MB)

- [ ] **No debug symbols in release** (except .pdb)

### 📝 Documentation

- [ ] **README.md updated** with latest features
- [ ] **CHANGELOG.md created** with version notes
- [ ] **COMPATIBILITY.md updated** with tested mods
- [ ] **Version number updated** in About.xml
- [ ] **All documentation in English** (README, COMPATIBILITY)
- [ ] **Russian docs updated** (STATUS.md, Development_Plan.md)

### 🧪 Testing

#### Unit & Integration Tests
- [ ] **All unit tests pass** (180+ tests)
- [ ] **All integration tests pass** (30+ tests)
- [ ] **No flaky tests** (run 5x to verify)
  ```bash
  for i in {1..5}; do make test; done
  ```

#### Performance Tests
- [ ] **Performance benchmarks pass** (15+ tests)
- [ ] **No regression detected** (<2x baseline)
- [ ] **Memory leak tests pass** (3 tests)
- [ ] **Large colony tests pass** (100+ pawns)

#### Compatibility Tests
- [ ] **Mod compatibility tests pass** (20+ tests)
- [ ] **No known incompatibilities** documented
- [ ] **Load order verified**

#### Stress Tests
- [ ] **Short stress tests pass** (9 tests)
- [ ] **Long-running test completed** (10-hour simulation)
  ```bash
  make test-run TARGET="Category=LongRunning"
  ```

### 🎮 In-Game Testing

#### Single Player
- [ ] **Mod loads without errors**
- [ ] **Settings UI works**
- [ ] **Debug overlay works** (F11)
- [ ] **Pathfinding works correctly**
- [ ] **AI behavior normal**
- [ ] **No crashes in 1+ hour gameplay**
- [ ] **Performance improvement verified** (TPS increase)

#### Test Scenarios
- [ ] **New colony** (start to finish)
- [ ] **Existing save** (load compatibility)
- [ ] **Large colony** (100+ pawns, 50+ buildings)
- [ ] **Complex pathfinding** (mountain base)
- [ ] **Combat scenario** (raid defense)

#### Multiplayer
- [ ] **Multiplayer mod detected correctly**
- [ ] **AsyncTime integration works**
- [ ] **No desyncs in 30+ min session**
- [ ] **Host and client both working**
- [ ] **Performance maintained**

### 🔌 Mod Compatibility

#### Core Mods (Must Test)
- [ ] **Harmony** - Required dependency
- [ ] **HugsLib** - Common framework
- [ ] **Multiplayer** - AsyncTime integration

#### Popular Mods (Should Test)
- [ ] **Combat Extended** - Major overhaul
- [ ] **Vanilla Expanded Core** - Content expansion
- [ ] **Vanilla Expanded Framework** - VE framework
- [ ] **Dubs Performance Analyzer** - Performance tool

#### Known Conflicts
- [ ] **RimThreaded** - Documented as incompatible
- [ ] **MultiThreading** - Documented as incompatible

### 🚨 Error Handling

- [ ] **Graceful degradation** on errors
- [ ] **Fallback to sync mode** works
- [ ] **Error messages clear** and actionable
- [ ] **Logs useful** for debugging
- [ ] **HugsLib integration** (Ctrl+F12)

### 📊 Performance Metrics

- [ ] **TPS improvement** measured (target: +15-30%)
- [ ] **Memory usage** acceptable (<20% increase)
- [ ] **Startup time** not significantly increased
- [ ] **Save/load time** not impacted

### 🔒 Safety & Stability

- [ ] **No data corruption** in saves
- [ ] **Safe to add/remove** mid-game
- [ ] **No memory leaks** detected
- [ ] **Thread limits** work correctly
- [ ] **Auto thread limit** works on various CPUs

## Release Preparation

### Version Management

1. **Update version numbers:**
   - `About/About.xml` - mod version
   - `Source/RimAsync/RimAsyncMod.cs` - code version
   - `README.md` - documentation
   - `CHANGELOG.md` - version entry

2. **Create git tag:**
   ```bash
   git tag -a v0.1.0-alpha -m "Alpha release v0.1.0"
   git push origin v0.1.0-alpha
   ```

### Build Artifacts

1. **Create release build:**
   ```bash
   docker-compose up release
   ```

2. **Package structure:**
   ```
   RimAsync/
   ├── About/
   │   ├── About.xml
   │   └── Preview.png
   ├── Assemblies/
   │   ├── RimAsync.dll
   │   └── RimAsync.deps.json
   ├── LICENSE
   └── README.txt
   ```

3. **Verify DLL:**
   ```bash
   # Check file size
   ls -lh Release/Assemblies/RimAsync.dll

   # Verify dependencies
   dotnet --list-deps Release/Assemblies/RimAsync.dll
   ```

### Documentation Preparation

1. **Create CHANGELOG.md:**
   ```markdown
   # Changelog

   ## [0.1.0-alpha] - 2025-11-XX

   ### Added
   - Async pathfinding system
   - AsyncTime integration for Multiplayer
   - Performance monitoring and TPS display
   - Debug overlay (F11)
   - Auto thread limit calculation
   - Smart caching system

   ### Performance
   - +15-30% TPS improvement in large colonies
   - <20% memory overhead
   - Efficient cache utilization (90%+ hit rate)
   ```

2. **Create README.txt** (for mod folder):
   ```
   RimAsync v0.1.0-alpha

   Performance mod with full multiplayer support.

   Features:
   - Async pathfinding
   - Multiplayer compatible (AsyncTime)
   - Auto thread optimization
   - Debug overlay (F11)

   Requirements:
   - RimWorld 1.6.4630+
   - Harmony (required)

   See full README at: [GitHub URL]
   ```

### Quality Assurance

- [ ] **Clean install test** (remove old version first)
- [ ] **Upgrade test** (install over previous version)
- [ ] **Uninstall test** (remove cleanly)
- [ ] **Multiple save test** (different scenarios)

## Steam Workshop Release

### Workshop Setup

1. **Create Preview.png** (640x360px recommended)
2. **Write Workshop description** (from README.md)
3. **Set tags:**
   - Performance
   - Optimization
   - Multiplayer
   - Utility

4. **Upload using SteamCMD:**
   ```bash
   # Prepare upload directory
   cp -r Release/ RimAsync_Workshop/

   # Use RimWorld Workshop Uploader
   # or SteamCMD
   ```

### Workshop Description Template

```markdown
[h1]RimAsync - Performance Optimization[/h1]

The first performance mod with full multiplayer support!

[h2]Features[/h2]
[list]
[*] Async pathfinding - Non-blocking path calculation
[*] Multiplayer compatible - Works with RimWorld Multiplayer + AsyncTime
[*] Auto optimization - Smart CPU-based thread limits
[*] Debug overlay - Real-time performance monitoring (F11)
[*] Safe & stable - Automatic fallbacks and extensive testing
[/list]

[h2]Requirements[/h2]
[list]
[*] RimWorld 1.6.4630+
[*] Harmony (required)
[*] RimWorld Multiplayer (optional, for multiplayer)
[/list]

[h2]Compatibility[/h2]
✅ Combat Extended
✅ Vanilla Expanded (all)
✅ HugsLib
✅ Multiplayer (with AsyncTime)
❌ RimThreaded (incompatible)

[h2]Load Order[/h2]
1. Harmony
2. HugsLib (if used)
3. Multiplayer (if used)
4. RimAsync
5. Other mods

[h2]Support[/h2]
[url=GITHUB_URL]GitHub[/url] | [url=DISCORD_URL]Discord[/url]

[h2]Performance[/h2]
Tested with 100+ pawns: +15-30% TPS improvement
```

## GitHub Release

### Release Artifacts

1. **Source code** (automatic from tag)
2. **RimAsync.zip** (packaged mod)
3. **RimAsync.dll** (standalone DLL)
4. **CHANGELOG.md**

### Release Notes Template

```markdown
## RimAsync v0.1.0-alpha

**First alpha release!** 🎉

### Highlights
- Async pathfinding for smooth gameplay
- Full multiplayer support with AsyncTime
- 255 passing tests (100% ✅)
- Comprehensive mod compatibility

### Installation
1. Download `RimAsync.zip`
2. Extract to `RimWorld/Mods/`
3. Enable in mod list after Harmony

### Known Issues
- [List any known issues]

### Testing
- 255/255 tests passing
- Tested with Combat Extended, Vanilla Expanded
- Multiplayer tested (no desyncs)
- 100+ pawns stress tested

### Changelog
[Full changelog](CHANGELOG.md)

### Compatibility
See [COMPATIBILITY.md](COMPATIBILITY.md) for full list.
```

## Post-Release

### Immediate Actions

- [ ] **Monitor bug reports** (first 24 hours critical)
- [ ] **Test user-reported issues**
- [ ] **Respond to questions**
- [ ] **Track download statistics**

### Week 1

- [ ] **Collect feedback** from community
- [ ] **Fix critical bugs** (hotfix if needed)
- [ ] **Update compatibility list** based on reports
- [ ] **Improve documentation** based on questions

### Week 2-4

- [ ] **Analyze performance reports** from users
- [ ] **Plan next version** features
- [ ] **Create roadmap** based on feedback
- [ ] **Consider beta promotion** if stable

## Hotfix Process

If critical bug found after release:

1. **Create hotfix branch:**
   ```bash
   git checkout -b hotfix/v0.1.1 v0.1.0-alpha
   ```

2. **Fix bug + add test**

3. **Verify fix:**
   ```bash
   make test
   ```

4. **Create hotfix release:**
   ```bash
   git tag -a v0.1.1-alpha -m "Hotfix: [description]"
   git push origin v0.1.1-alpha
   ```

5. **Update Workshop immediately**

6. **Notify users** in description/changelog

## Version Progression

### Alpha (v0.1.x)
- **Purpose:** Initial testing, core features
- **Audience:** Willing testers, developers
- **Updates:** Frequent, may break saves

### Beta (v0.2.x)
- **Purpose:** Stability testing, polish
- **Audience:** Early adopters
- **Updates:** Less frequent, save-compatible

### Stable (v1.0.0)
- **Purpose:** Production ready
- **Audience:** General users
- **Updates:** Stable, well-tested

## Success Metrics

### Technical
- [ ] **Crash rate** < 1%
- [ ] **Performance improvement** > 15%
- [ ] **Test coverage** > 85%
- [ ] **Bug reports** < 10 critical in month 1

### Community
- [ ] **Downloads** > 1000 in week 1
- [ ] **Rating** > 4.5/5 stars
- [ ] **Positive feedback** > 80%
- [ ] **Active users** > 500 monthly

## Rollback Plan

If major issues found:

1. **Remove from Workshop** (hide, don't delete)
2. **Add warning** to GitHub release
3. **Fix issues** in develop branch
4. **Re-test thoroughly**
5. **Re-release** when ready

---

**Remember:** Quality > Speed. Better to delay than release broken mod.

**Questions?** Review this checklist with the team before each release.
