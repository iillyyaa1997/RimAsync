# Create Branch Command

**Command:** `@create-branch`
**Description:** Create git branches for special scenarios (NOT for regular development tasks)

## ⚠️ Important Note

**Use `@execute-task` for regular development!** This command is only for special cases where manual branch creation is needed.

## 🎯 Usage

### Basic Usage
```
@create-branch feature/feature-name     # Create feature branch
@create-branch bugfix/issue-description # Create bugfix branch
@create-branch hotfix/critical-fix      # Create hotfix branch
```

### Advanced Usage
```
@create-branch experimental/new-idea --no-tests    # Skip test creation
@create-branch docs/update-readme --docs-only      # Documentation only
@create-branch refactor/cleanup --from develop     # From specific branch
```

## 🔧 What the command does

1. **Validates branch name** according to project conventions
2. **Creates git branch** from master (or specified base)
3. **Sets up branch tracking** for remote repository
4. **Creates basic test structure** (if applicable)
5. **Updates local documentation** with branch info
6. **Prepares development environment** for the new branch

## 📋 Branch Naming Conventions

### 🌿 Branch Types

#### Feature Branches (`feature/`)
```
feature/async-pathfinding
feature/performance-monitor
feature/multiplayer-compat
feature/settings-ui
```

#### Bugfix Branches (`bugfix/`)
```
bugfix/null-reference-asyncmanager
bugfix/docker-build-timeout
bugfix/memory-leak-smartcache
bugfix/harmony-patch-conflict
```

#### Hotfix Branches (`hotfix/`)
```
hotfix/critical-desync-fix
hotfix/game-crash-startup
hotfix/multiplayer-incompatible
```

#### Experimental Branches (`experimental/`)
```
experimental/new-threading-model
experimental/alternative-caching
experimental/ui-redesign
```

#### Documentation Branches (`docs/`)
```
docs/api-documentation
docs/user-guide-update
docs/developer-readme
```

### 📝 Naming Rules

#### ✅ Good Branch Names
- **Descriptive:** `feature/async-pathfinding-optimization`
- **Kebab-case:** `bugfix/memory-leak-detection`
- **Concise:** `hotfix/startup-crash`
- **Clear purpose:** `docs/installation-guide`

#### ❌ Bad Branch Names
- **Too generic:** `feature/improvement`
- **CamelCase:** `feature/AsyncPathfinding`
- **Spaces:** `feature/async pathfinding`
- **Too long:** `feature/implement-async-pathfinding-with-multiplayer-compatibility-and-performance-optimization`

## 🔄 Workflow

### 🌿 Git Operations
1. **git checkout master** - switches to main branch
2. **git pull origin master** - gets latest changes
3. **git checkout -b [branch-name]** - creates new branch
4. **git push -u origin [branch-name]** - sets up remote tracking

### 📁 Project Setup
5. **Creates branch directory** in `Planning/Branches/`
6. **Generates branch documentation** with objectives
7. **Creates test placeholders** (if applicable)
8. **Updates branch tracking** in project metadata

### 📋 Documentation Updates
9. **Updates CONTRIBUTORS.md** with branch info
10. **Creates branch TODO list** for objectives
11. **Sets up merge requirements** for the branch

## 🎯 Special Branch Types

### 🧪 Experimental Branches
```
@create-branch experimental/new-threading --no-tests

Features:
- No automatic test creation
- Isolated from main development
- Can break existing functionality
- Used for proof-of-concept work
```

### 📚 Documentation Branches
```
@create-branch docs/api-update --docs-only

Features:
- Only documentation changes
- No code compilation required
- Fast merge to master
- Automatic README updates
```

### 🚨 Hotfix Branches
```
@create-branch hotfix/critical-bug --urgent

Features:
- Created from master
- Immediate priority
- Minimal testing requirements
- Fast-track merge process
```

### 🔧 Refactor Branches
```
@create-branch refactor/code-cleanup --full-tests

Features:
- Comprehensive test coverage required
- No functionality changes
- Code quality improvements only
- Performance validation needed
```

## 🔍 Branch Validation

### Pre-Creation Checks
```
✅ Validation Results:

🌿 Git Status:
├── Current branch: master
├── Working directory: Clean
├── Remote sync: Up to date
└── Uncommitted changes: None

📝 Branch Name:
├── Format: ✅ Valid (feature/async-pathfinding)
├── Length: ✅ Appropriate (25 chars)
├── Conventions: ✅ Follows project standards
└── Uniqueness: ✅ No conflicts

🎯 Project State:
├── Build status: ✅ Passing
├── Tests status: ✅ All passing
├── Dependencies: ✅ Up to date
└── Documentation: ✅ Current
```

### Post-Creation Verification
```
🎉 Branch Created Successfully:

🌿 Git Information:
├── Branch: feature/async-pathfinding
├── Base: master (commit: abc123def)
├── Remote tracking: ✅ Set up
└── Local checkout: ✅ Active

📁 Project Structure:
├── Branch docs: Planning/Branches/feature-async-pathfinding.md
├── Test placeholders: Tests/Unit/AsyncPathfinding/
├── TODO list: Created with 5 objectives
└── Merge requirements: Defined

📋 Next Steps:
1. Review objectives in branch documentation
2. Start development with @execute-task
3. Regular commits with descriptive messages
4. Create PR when ready for review
```

## 🔧 Command Options

### Branch Type Options
- `--feature` - Feature branch (default)
- `--bugfix` - Bugfix branch
- `--hotfix` - Hotfix branch
- `--experimental` - Experimental branch
- `--docs` - Documentation branch

### Setup Options
- `--no-tests` - Skip test structure creation
- `--docs-only` - Documentation changes only
- `--full-tests` - Comprehensive test coverage
- `--urgent` - Fast-track urgent changes

### Base Branch Options
- `--from master` - Create from master (default)
- `--from develop` - Create from develop branch
- `--from [branch]` - Create from specific branch

## 🚫 When NOT to Use This Command

### Use `@execute-task` Instead For:
- **Regular development tasks** from Development_Plan.md
- **Planned feature implementation**
- **Scheduled bug fixes**
- **Standard workflow tasks**

### Only Use `@create-branch` For:
- **Experimental work** not in the plan
- **Emergency hotfixes** requiring immediate attention
- **Documentation updates** separate from development
- **Code refactoring** projects
- **Special investigation** branches

## 🚀 Usage Examples

### Feature Development
```
@create-branch feature/ui-improvements

🌿 Creating feature branch for UI improvements...
├── ✅ Branch: feature/ui-improvements
├── 📁 Documentation: Planning/Branches/feature-ui-improvements.md
├── 🧪 Tests: Tests/Unit/UI/
└── 📋 TODO: 3 objectives created

Ready for development! Use @execute-task to begin implementation.
```

### Emergency Hotfix
```
@create-branch hotfix/startup-crash --urgent

🚨 Creating urgent hotfix branch...
├── ✅ Branch: hotfix/startup-crash
├── 🔥 Priority: CRITICAL
├── 📋 Fast-track: Enabled
└── ⏰ Timeline: Immediate

Emergency branch ready! Address critical issue immediately.
```

### Documentation Update
```
@create-branch docs/api-reference --docs-only

📚 Creating documentation branch...
├── ✅ Branch: docs/api-reference
├── 📖 Type: Documentation only
├── 🚫 No code compilation
└── 📝 Focus: Documentation updates

Documentation branch ready! Update docs without code changes.
```

## 📋 Branch Management

### Active Branch Tracking
The project maintains a list of active branches in `Planning/Branches/`:

```
Planning/Branches/
├── active-branches.md (branch registry)
├── feature-async-pathfinding.md
├── bugfix-memory-leak.md
├── docs-api-update.md
└── experimental-threading.md
```

### Branch Lifecycle
1. **Created** - Branch created and documented
2. **Active** - Development in progress
3. **Testing** - Under review and testing
4. **Ready** - Ready for merge to master
5. **Merged** - Successfully merged and deleted

---

**Remember:** Use `@execute-task` for regular development. This command is for special cases only!
