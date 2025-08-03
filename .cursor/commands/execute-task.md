# Execute Task Command

**Command:** `@execute-task`
**Description:** Execute the next priority task from RimAsync development plan with mandatory task breakdown

## 🎯 Usage

In Cursor IDE chat, type:
```
@execute-task
```

## 🔧 What the command does

1. **Creates new git branch** from latest master for the task
2. **Analyzes current project status** from Planning/Development_Plan.md
3. **Selects highest priority task**
4. **MANDATORY breaks down task** into 2-5 specific subtasks
5. **Creates TODO list** with clear success criteria
6. **Creates tests** for the task (if required)
7. **Implements functionality** according to technical plan
8. **Compiles project in Docker** container for safety
9. **Runs tests in Docker** to verify implementation
10. **Commits changes** to feature branch
11. **Merges to master** and deletes feature branch
12. **Updates task status** in the plan

## 🐳 Docker Environment

**ALL compilation and testing operations are performed in Docker containers!**

### 📋 Makefile Commands (Recommended):
- `make build` - compile project (with colored output)
- `make test` - run tests (with progress)
- `make test-run TARGET="..." [OPTS="..."]` - universal test runner (interactive)
- `make test-quick` - quick test menu (4 most common options)
- `make t` - super quick unit tests (fastest)
- `make coverage-quick` - quick coverage for unit tests (MANDATORY for individual testing)
- `make coverage` - full coverage report for all tests
- `make coverage-html` - HTML coverage report with visual analysis
- `make dev` - development mode
- `make quick-build` - quick compilation for debugging
- `make help` - show all available commands

### 🔧 Raw Docker Commands (alternative):
- `docker-compose up build` - compile project
- `docker-compose up test` - run tests
- `docker-compose up dev` - development mode
- `docker-compose up quick-build` - quick compilation for debugging

**💡 Use Makefile commands for better UX and automatic cleanup!**

## 📋 Priority Task Queue

### 🔴 CRITICAL priority (executed first):
1. **Project compilation** - fix compilation errors
2. **AsyncTime integration** - detection logic testing
3. **Basic async pathfinding** - functionality verification
4. **Performance monitoring** - real-time TPS metrics

### 🟠 HIGH priority:
1. **AI processing async** - extend async functionality
2. **Building updates async** - async building processing
3. **Mod compatibility testing** - testing with top-5 mods
4. **Memory optimization** - SmartCache improvements

### 🟡 MEDIUM priority:
1. **Advanced settings UI** - additional options
2. **Debug overlay** - extended metrics
3. **Documentation polish** - documentation improvements
4. **Community feedback integration** - feedback integration

## 🔧 Task Breakdown Guidelines

### 📝 Task breakdown rules:

**MANDATORY:** Every task must be broken down into 2-5 specific subtasks

#### ✅ Good subtasks:
- **Specific and measurable** - "Add Initialize() method to AsyncManager"
- **Completable in 1-2 hours** - not too big or small
- **Independent** - can be completed in any order
- **Testable** - has clear completion criteria

#### ❌ Bad subtasks:
- **Too general** - "Improve performance"
- **Too large** - "Rewrite entire system"
- **Dependent** - cannot start without previous one
- **Non-testable** - unclear when completed

### 🎯 Breakdown examples:

**Task:** "AsyncTime integration - detection logic testing"

**Subtasks:**
1. 🔧 Create mock for MultiplayerCompat.IsInMultiplayer
2. 🧪 Write tests for single player mode
3. 🧪 Write tests for multiplayer without AsyncTime
4. 🧪 Write tests for multiplayer with AsyncTime
5. 📊 Add performance tests for mode switching

**Task:** "Performance monitoring - real-time TPS metrics"

**Subtasks:**
1. 🔧 Create PerformanceMonitor class with TPS tracking
2. 🔧 Add StartMeasuring/StopMeasuring methods
3. 🧪 Write unit tests for performance measurement
4. 🎮 Integrate with RimAsyncCore for automatic monitoring

## 🧪 Testing Strategy

Each task **MANDATORY** includes:

### 1. Unit Tests
```csharp
[Test]
public void TaskName_Scenario_ExpectedResult()
{
    // Arrange
    // Act
    // Assert
}
```

### 2. Integration Tests
```csharp
[Test]
public void TaskName_Integration_WorksWithSystem()
{
    // Test integration with existing systems
}
```

### 3. Performance Tests
```csharp
[Test]
public void TaskName_Performance_MeetsTargets()
{
    // Benchmark performance improvements
}
```

### 4. Multiplayer Tests
```csharp
[Test]
public void TaskName_Multiplayer_NoDesync()
{
    // Test multiplayer compatibility
}
```

### 🚨 5. MANDATORY Coverage Testing

**After implementing any task, ALWAYS verify code coverage:**

```bash
# For unit tests specifically for your task:
make test-run TARGET="YourTaskTestClass" && make coverage-quick

# For full task verification:
make test && make coverage

# For detailed analysis:
make coverage-html
```

**Coverage Requirements for Tasks:**
- **New functionality:** Minimum 85% coverage
- **Critical systems (Core, AsyncManager):** Minimum 90% coverage
- **Bug fixes:** Must include tests reproducing the bug
- **Performance improvements:** Must include benchmark tests

**Coverage Verification Checklist:**
- [ ] Unit tests cover all new methods
- [ ] Error handling paths are tested
- [ ] Edge cases are covered
- [ ] Integration points are tested
- [ ] Performance requirements are verified

## 🔄 Workflow

### 🌿 Git Branch Management
1. **git checkout master** - switch to main branch
2. **git pull origin master** - get latest changes
3. **git checkout -b feature/task-name** - create new feature branch

### 📋 Task Planning & Breakdown
4. **Analyzes Planning/Development_Plan.md**
5. **Selects next critical task**
6. **MANDATORY breaks down task into subtasks** (2-5 subtasks maximum)
7. **Creates TODO list** with concrete execution steps
8. **Defines success criteria** for each subtask

### 🎯 Task Execution
9. **Creates test file:** `Tests/[TaskName]Tests.cs`
10. **Implements functionality** in corresponding file
11. **make format-fix** - code auto-formatting (MANDATORY)
12. **make build** - compilation in Docker (convenient Makefile command)
13. **make lint** - code quality check (MANDATORY)
14. **make test** - run tests (convenient Makefile command)
15. **Checks results** and fixes errors if needed

### 📝 Git Commit & Update
16. **git add .** - adds changes
17. **git commit -m "feat(task): descriptive message"** - commit with conventional commits
18. **Updates status** in plan: ⏳ → 🔄 → ✅
19. **git push origin feature/task-name** - pushes branch (optional)

### 🔄 Branch Merge & Cleanup (FINAL STAGE)
20. **git checkout master** - switches to master
21. **git merge feature/task-name** - merges feature branch to master
22. **git push origin master** - pushes updated master
23. **git branch -d feature/task-name** - deletes local feature branch

## 🎯 Task Completion Criteria

- ✅ **All subtasks completed** - each subtask from TODO list executed
- ✅ **Functionality implemented** according to technical plan
- ✅ **All tests pass** (Unit + Integration + Performance)
- ✅ **Code style complies** with project standards
- ✅ **Documentation updated** (if required)
- ✅ **Performance targets achieved** (if applicable)
- ✅ **Multiplayer compatibility** verified (if applicable)
- ✅ **Status updated** in Planning/Development_Plan.md
- ✅ **Feature branch merged** to master and deleted

## 🚀 Execution Example

```
User: @execute-task

AI: 🌿 Creating new git branch...
git checkout master
git pull origin master
git checkout -b feature/fix-compilation-errors

🎯 Analyzing current tasks...

📋 Selected task: "Project compilation - fix compilation errors"
🔴 Priority: CRITICAL
📁 Files: Source/RimAsync/Core/*.cs

🔧 Breaking down task into subtasks:
1. 🔍 Analyze compilation errors in Docker
2. 🔧 Add missing using statements
3. 🔧 Fix namespace conflicts
4. 🔧 Update mock references
5. 🧪 Create comprehensive compilation tests

📝 TODO List created - 5 subtasks defined!

🧪 Creating tests:
- Tests/Unit/Core/CompilationTests.cs
- Tests/Integration/ProjectStructureTests.cs

🔧 Fixing compilation errors:
- Add missing using statements for RimWorld/Verse namespaces
- Fix namespace conflicts
- Update references

🎨 Auto-formatting code:
make format-fix ✅ Code style fixed

🐳 Docker compilation:
make build ✅

🔍 Code quality check:
make lint ✅ Analysis passed, 0 errors

🧪 Docker testing:
make test ✅ 8/8 tests passed

📝 Committing changes:
git add .
git commit -m "feat(compilation): fix compilation errors and add tests

- Add missing using statements for RimWorld/Verse namespaces
- Fix .csproj structure for .NET Standard 2.1
- Add mock references for Docker compilation
- Add comprehensive compilation tests

Resolves: Compilation errors in Docker environment"

✅ Status updated: ⏳ → ✅

🔄 Merging to master and cleaning up branches:
git checkout master
git merge feature/fix-compilation-errors ✅ Fast-forward merge
git push origin master ✅ Master updated
git branch -d feature/fix-compilation-errors ✅ Branch deleted

🎉 Task completed! Next task: "AsyncTime integration testing"
```

---

**Note:** This command automatically selects the highest priority task. To select a specific task, use `@execute-task [task-name]`
