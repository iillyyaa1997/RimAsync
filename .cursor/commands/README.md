# Cursor IDE Commands for RimAsync

**Version:** 2.0
**Language:** English Only (as per project requirements)

## 📋 Command Overview

This directory contains all Cursor IDE commands for the RimAsync project. All commands are documented in English to ensure international collaboration and compatibility.

### 🌍 Language Policy

**IMPORTANT:** All Cursor commands must be written in English. See [Language Requirements](./../rules/cursor-commands.mdc) for detailed guidelines.

## 🚀 Available Commands

### 🎯 Primary Development Command

#### `@execute-task`
- **Purpose:** Execute the next priority task from development plan
- **Features:** Automatic task breakdown, testing, and branch management
- **Usage:** `@execute-task` (automatic task selection)
- **File:** `execute-task.md`

### 🧪 Testing Commands

#### `@run-tests`
- **Purpose:** Execute comprehensive test suite in Docker environment
- **Features:** Multiple test categories, performance benchmarks, coverage reports
- **Usage:** `@run-tests [category] [options]`
- **File:** `run-tests.md`

#### `@create-tests`
- **Purpose:** Generate comprehensive test suites for components
- **Features:** Unit, integration, and performance test generation
- **Usage:** `@create-tests [component] [options]`
- **File:** `create-tests.md`

### 🔍 Analysis Commands

#### `@analyze-logs`
- **Purpose:** Comprehensive analysis of RimWorld logs and Docker containers
- **Features:** Error detection, desync analysis, performance monitoring
- **Usage:** `@analyze-logs [options]`
- **File:** `analyze-logs.md`

### 🌿 Git Management Commands

#### `@create-branch`
- **Purpose:** Create git branches for special scenarios (NOT regular development)
- **Features:** Branch validation, documentation generation, tracking setup
- **Usage:** `@create-branch [branch-name] [options]`
- **File:** `create-branch.md`

## 🔄 Command Workflow

### Primary Development Workflow
```
1. @execute-task              # Select and execute next priority task
2. [Automatic branching]      # Creates feature branch automatically
3. [Automatic testing]       # Runs tests in Docker environment
4. [Automatic merging]       # Merges to master when complete
```

### Supporting Workflow
```
1. @analyze-logs             # Analyze issues if problems occur
2. @create-tests Component   # Generate additional tests if needed
3. @run-tests --specific     # Run targeted test categories
4. @create-branch hotfix/... # Emergency fixes (special cases only)
```

## 📁 Command Structure

### File Organization
```
.cursor/commands/
├── README.md              # This overview document
├── execute-task.md        # Primary development command
├── run-tests.md          # Test execution command
├── create-tests.md       # Test generation command
├── analyze-logs.md       # Log analysis command
└── create-branch.md      # Branch management command
```

### Documentation Format
Each command file follows this structure:
```markdown
# [Command Name] Command

**Command:** `@command-name`
**Description:** Brief description in English

## 🎯 Usage
[Usage examples and syntax]

## 🔧 What the command does
[Step-by-step breakdown]

## [Additional sections as needed]
```

## 🎯 Command Categories

### 🔴 Critical Priority Commands
- `@execute-task` - Primary development workflow
- `@run-tests` - Essential testing validation
- `@analyze-logs` - Critical issue debugging

### 🟠 High Priority Commands
- `@create-tests` - Test infrastructure development
- `@create-branch` - Emergency branch management

### 🟡 Medium Priority Commands
- Future commands will be added as needed

## 🔧 Command Development Guidelines

### Creating New Commands

1. **Follow English-only policy** - All text must be in English
2. **Use standard structure** - Follow existing command documentation format
3. **Include comprehensive examples** - Provide clear usage scenarios
4. **Add appropriate categories** - Use established priority system
5. **Test thoroughly** - Validate command functionality before committing

### Command Naming Conventions

#### ✅ Good Command Names
- `@execute-task` - Clear action verb + target
- `@run-tests` - Simple verb + noun
- `@analyze-logs` - Descriptive action + object
- `@create-branch` - Action verb + target object

#### ❌ Bad Command Names
- `@non-english-command` - Non-English language (forbidden)
- `@do-stuff` - Too generic
- `@executeTaskAndRunTestsAndAnalyzeLogs` - Too long
- `@et` - Too abbreviated

### Documentation Standards

#### Required Sections
- **Command name and description** (English only)
- **Usage section** with examples
- **What the command does** step-by-step breakdown
- **Options/parameters** if applicable

#### Optional Sections
- **Examples** for complex commands
- **Best practices** for proper usage
- **Integration** with other commands
- **Troubleshooting** for common issues

## 🌐 International Collaboration

### Why English?

1. **Global Accessibility** - Enables international contributors
2. **Tool Integration** - Better compatibility with development tools
3. **Community Sharing** - Facilitates sharing with RimWorld modding community
4. **Long-term Maintenance** - Ensures future maintainability
5. **Industry Standards** - Aligns with software development practices

### Migration from Russian

The project is transitioning from Russian to English commands:

#### Migration Status
- ✅ **Language policy established** - Rules documented
- ✅ **English command examples created** - Templates available
- ⏳ **Command migration in progress** - Gradual transition
- 📋 **Documentation updates ongoing** - Comprehensive coverage

#### Backward Compatibility
- Existing Russian commands continue to work during transition
- New commands created only in English
- Gradual deprecation of Russian commands over time

## 🔍 Command Testing

### Validation Process

Before adding new commands:

1. **Syntax validation** - Ensure proper markdown format
2. **Language compliance** - Verify English-only content
3. **Functionality testing** - Test command execution
4. **Documentation review** - Check completeness and clarity
5. **Integration testing** - Verify compatibility with existing commands

### Quality Assurance

Commands must meet these standards:

- **Clear documentation** - Easy to understand and follow
- **Comprehensive examples** - Cover common use cases
- **Error handling** - Graceful failure modes
- **Performance** - Efficient execution
- **Reliability** - Consistent results

## 📊 Command Usage Statistics

### Most Used Commands (Expected)
1. `@execute-task` - Primary development workflow
2. `@run-tests` - Testing and validation
3. `@analyze-logs` - Debugging and troubleshooting
4. `@create-tests` - Test development
5. `@create-branch` - Special case branch management

## 🔄 Future Development

### Planned Commands
- `@check-compatibility` - RimWorld/Multiplayer compatibility validation
- `@generate-docs` - Automatic documentation generation
- `@optimize-performance` - Performance analysis and optimization
- `@validate-release` - Pre-release validation checks

### Command Enhancement
- Enhanced error reporting
- Better integration between commands
- Automated workflow suggestions
- Performance optimizations

## 🎯 Getting Started

### For New Contributors

1. **Read language requirements** - Understand English-only policy
2. **Study existing commands** - Learn established patterns
3. **Follow documentation standards** - Use consistent format
4. **Test thoroughly** - Validate before submitting
5. **Seek review** - Get feedback from team members

### For Command Users

1. **Start with `@execute-task`** - Primary development command
2. **Use `@run-tests`** for validation
3. **Check `@analyze-logs`** for debugging
4. **Avoid `@create-branch`** for regular development
5. **Follow command examples** - Use provided syntax

---

**Remember:** Use `@execute-task` for regular development. It handles branching, testing, and merging automatically!
