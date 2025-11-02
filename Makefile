# RimAsync Makefile
# Convenient commands for Docker-based development

.PHONY: help build test test-unit test-integration test-performance test-run test-quick test-find t coverage coverage-html coverage-quick test-unit-coverage test-integration-coverage test-performance-coverage test-run-coverage test-quick-coverage test-find-coverage t-coverage dev quick-build release clean logs shell format lint setup install package deploy-local

# Default target
.DEFAULT_GOAL := help

# Colors for output
CYAN=\033[0;36m
GREEN=\033[0;32m
YELLOW=\033[1;33m
RED=\033[0;31m
NC=\033[0m # No Color

## 🆘 Show this help message
help:
	@echo ""
	@echo "$(CYAN)RimAsync Development Commands$(NC)"
	@echo ""
	@echo "$(GREEN)⚡ Quick Start:$(NC)"
	@echo "  $(YELLOW)make deploy$(NC)       - Build + Install in one command (recommended)"
	@echo ""
	@echo "$(GREEN)🏗️  Build Commands:$(NC)"
	@echo "  $(YELLOW)make build$(NC)        - Full build in Docker container"
	@echo "  $(YELLOW)make quick-build$(NC)  - Quick incremental build"
	@echo "  $(YELLOW)make release$(NC)      - Production release build"
	@echo ""
	@echo "$(GREEN)🧪 Test Commands:$(NC)"
	@echo "  $(YELLOW)make test$(NC)         - Run all tests in Docker"
	@echo "  $(YELLOW)make test-unit$(NC)    - Run unit tests only"
	@echo "  $(YELLOW)make test-integration$(NC) - Run integration tests only"
	@echo "  $(YELLOW)make test-performance$(NC) - Run performance tests only"

	@echo "  $(YELLOW)make test-run [TARGET=\"...\"] [OPTS=\"...\"]$(NC) - Universal test runner (interactive)"
	@echo "  $(YELLOW)make test-quick$(NC) - Quick test menu (4 most common options)"
	@echo "  $(YELLOW)make test-find NAME=\"SearchTerm\"$(NC) - Smart search for tests by name"
	@echo "  $(YELLOW)make t$(NC) - Super quick test (unit tests only)"
	@echo ""
	@echo "$(GREEN)📊 Test Commands with Built-in Coverage:$(NC)"
	@echo "  $(YELLOW)make test-unit-coverage$(NC) - Unit tests with coverage"
	@echo "  $(YELLOW)make test-integration-coverage$(NC) - Integration tests with coverage"
	@echo "  $(YELLOW)make test-performance-coverage$(NC) - Performance tests with coverage"
	@echo "  $(YELLOW)make test-run-coverage [TARGET=\"...\"]$(NC) - Universal runner with coverage"
	@echo "  $(YELLOW)make test-quick-coverage$(NC) - Quick menu with coverage options"
	@echo "  $(YELLOW)make test-find-coverage NAME=\"SearchTerm\"$(NC) - Smart search with coverage"
	@echo "  $(YELLOW)make t-coverage$(NC) - Super quick test with coverage"
	@echo ""
	@echo "$(GREEN)📊 Coverage Commands:$(NC)"
	@echo "  $(YELLOW)make coverage-skip$(NC)   - Run tests without coverage (FASTEST)"
	@echo "  $(YELLOW)make coverage-basic$(NC)  - Unit test coverage only (FAST & RELIABLE)"
	@echo "  $(YELLOW)make coverage-quick$(NC)  - Quick coverage for unit tests only (legacy)"
	@echo "  $(YELLOW)make coverage$(NC)        - Full coverage report (SLOW, may hang)"
	@echo "  $(YELLOW)make coverage-html$(NC)   - Generate HTML coverage report"
	@echo "    - Env: COVERAGE_TIMEOUT (sec, default 900) — container timeout"
	@echo "    - Env: TEST_TIMEOUT (sec, default 60) — inner \"dotnet test\" timeout"
	@echo ""
	@echo "$(GREEN)🚀 Development Commands:$(NC)"
	@echo "  $(YELLOW)make dev$(NC)          - Start development environment"
	@echo "  $(YELLOW)make shell$(NC)        - Enter Docker container shell"
	@echo "  $(YELLOW)make logs$(NC)         - Show Docker container logs"
	@echo "  $(YELLOW)make install$(NC)      - Install/Update mod in RimWorld"
	@echo ""
	@echo "$(GREEN)🧹 Maintenance Commands:$(NC)"
	@echo "  $(YELLOW)make clean$(NC)        - Clean Docker images and containers"
	@echo "  $(YELLOW)make clean-all$(NC)    - Deep clean (images, volumes, cache)"
	@echo ""
	@echo "$(GREEN)🎨 Code Quality Commands:$(NC)"
	@echo "  $(YELLOW)make format$(NC)       - Format code (dotnet format)"
	@echo "  $(YELLOW)make format-fix$(NC)   - Auto-fix code style issues"
	@echo "  $(YELLOW)make format-check$(NC) - Check code style without fixing"
	@echo "  $(YELLOW)make lint$(NC)         - Run code analysis"
	@echo "  $(YELLOW)make lint-report$(NC)  - Generate detailed analysis report"
	@echo "  $(YELLOW)make security-lint$(NC) - Run security vulnerability scan"
	@echo ""
	@echo "$(GREEN)📋 Setup Commands:$(NC)"
	@echo "  $(YELLOW)make setup$(NC)        - Initial project setup"
	@echo "  $(YELLOW)make docker-info$(NC)  - Show Docker environment info"
	@echo ""

## 🏗️ Build the project in Docker
build:
	@echo "$(CYAN)🏗️ Building RimAsync in Docker...$(NC)"
	docker-compose up build --remove-orphans
	@echo "$(GREEN)✅ Build completed!$(NC)"

## ⚡ Quick incremental build
quick-build:
	@echo "$(CYAN)⚡ Quick build...$(NC)"
	docker-compose up quick-build --remove-orphans
	@echo "$(GREEN)✅ Quick build completed!$(NC)"

## 🚀 Production release build
release:
	@echo "$(CYAN)🚀 Creating release build...$(NC)"
	docker-compose up release --remove-orphans
	@echo "$(GREEN)✅ Release build completed!$(NC)"

## 🧪 Run all tests
test:
	@echo "$(CYAN)🧪 Running all tests...$(NC)"
	docker-compose up test --remove-orphans
	@echo "$(GREEN)✅ Tests completed!$(NC)"

## 🔬 Run unit tests only
test-unit:
	@echo "$(CYAN)🔬 Running unit tests...$(NC)"
	docker-compose exec test dotnet test --filter Category=Unit
	@echo "$(GREEN)✅ Unit tests completed!$(NC)"

## 🔗 Run integration tests only
test-integration:
	@echo "$(CYAN)🔗 Running integration tests...$(NC)"
	docker-compose exec test dotnet test --filter Category=Integration
	@echo "$(GREEN)✅ Integration tests completed!$(NC)"

## ⚡ Run performance tests only
test-performance:
	@echo "$(CYAN)⚡ Running performance tests...$(NC)"
	docker-compose exec test dotnet test --filter Category=Performance
	@echo "$(GREEN)✅ Performance tests completed!$(NC)"









## 🎯 Universal test runner - one command for everything
test-run:
	@if [ -n "$(TARGET)" ]; then \
		echo "$(CYAN)🎯 Running tests with target: $(TARGET)$(NC)"; \
		if [ -n "$(OPTS)" ]; then \
			echo "$(YELLOW)📋 Using options: $(OPTS)$(NC)"; \
			docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$(TARGET)\" $(OPTS)"; \
		else \
			docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$(TARGET)\" --logger \"console;verbosity=normal\""; \
		fi; \
		echo "$(GREEN)✅ Tests completed!$(NC)"; \
	else \
		echo "$(CYAN)🚀 Universal Test Runner$(NC)"; \
		echo "$(YELLOW)Choose test target:$(NC)"; \
		echo "  1) All tests"; \
		echo "  2) Unit tests only"; \
		echo "  3) Integration tests only"; \
		echo "  4) Performance tests only"; \
		echo "  5) Multiplayer tests"; \
		echo "  6) Settings UI tests"; \
		echo "  7) Specific test class"; \
		echo "  8) Specific test method"; \
		echo "  9) Custom filter"; \
		echo ""; \
		echo "$(YELLOW)💡 Quick examples:$(NC)"; \
		echo "  make test-run TARGET=\"Category=Unit\""; \
		echo "  make test-run TARGET=\"MultiplayerDetectionTests\""; \
		echo "  make test-run TARGET=\"Name~Initialize\" OPTS=\"--logger html;LogFileName=report.html\""; \
		echo ""; \
		read -p "Enter choice (1-9) or press Enter for all tests: " choice; \
		case "$$choice" in \
			1|"") echo "$(CYAN)🧪 Running all tests...$(NC)"; \
				docker-compose run test bash -c "cd /app/Tests && dotnet test --logger \"console;verbosity=normal\"";; \
			2) echo "$(CYAN)🔬 Running unit tests...$(NC)"; \
				docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Unit\" --logger \"console;verbosity=normal\"";; \
			3) echo "$(CYAN)🔗 Running integration tests...$(NC)"; \
				docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Integration\" --logger \"console;verbosity=normal\"";; \
			4) echo "$(CYAN)⚡ Running performance tests...$(NC)"; \
				docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Performance\" --logger \"console;verbosity=normal\"";; \
			5) echo "$(CYAN)🌐 Running multiplayer tests...$(NC)"; \
				docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"MultiplayerDetectionTests\" --logger \"console;verbosity=normal\"";; \
			6) echo "$(CYAN)🎨 Running settings UI tests...$(NC)"; \
				docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"SettingsUITests\" --logger \"console;verbosity=normal\"";; \
			7) echo "$(CYAN)📋 Running specific test class...$(NC)"; \
				read -p "Enter test class name: " classname; \
				if [ -n "$$classname" ]; then \
					docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$$classname\" --logger \"console;verbosity=normal\""; \
				else \
					echo "$(RED)❌ Class name required$(NC)"; \
				fi;; \
			8) echo "$(CYAN)🔍 Running specific test method...$(NC)"; \
				read -p "Enter test method name (or part of it): " methodname; \
				if [ -n "$$methodname" ]; then \
					docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Name~$$methodname\" --logger \"console;verbosity=detailed\""; \
				else \
					echo "$(RED)❌ Method name required$(NC)"; \
				fi;; \
			9) echo "$(CYAN)🎛️ Custom filter...$(NC)"; \
				read -p "Enter filter expression: " filter; \
				if [ -n "$$filter" ]; then \
					read -p "Enter verbosity (normal/detailed/diagnostic) [normal]: " verbosity; \
					verbosity=$${verbosity:-normal}; \
					docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$$filter\" --logger \"console;verbosity=$$verbosity\""; \
				else \
					echo "$(RED)❌ Filter expression required$(NC)"; \
				fi;; \
			*) echo "$(RED)❌ Invalid choice$(NC)";; \
		esac; \
		echo "$(GREEN)✅ Test run completed!$(NC)"; \
	fi

## 🚀 Quick test shortcuts - most common test scenarios
test-quick:
	@echo "$(CYAN)⚡ Quick Test Menu$(NC)"
	@echo "$(YELLOW)What do you want to test?$(NC)"
	@echo "  1) Unit tests (fast)"
	@echo "  2) Multiplayer detection"
	@echo "  3) Settings UI"
	@echo "  4) Everything"
	@echo ""
	@read -p "Choice (1-4): " choice; \
	case "$$choice" in \
		1) make test-run TARGET="Category=Unit";; \
		2) make test-run TARGET="MultiplayerDetectionTests";; \
		3) make test-run TARGET="SettingsUITests";; \
		4) make test-run;; \
		*) echo "$(RED)❌ Invalid choice$(NC)";; \
	esac

## 📋 Test specific component by name (smart search)
test-find:
	@if [ -z "$(NAME)" ]; then \
		echo "$(RED)❌ NAME parameter required$(NC)"; \
		echo "$(YELLOW)💡 Usage: make test-find NAME=\"SearchTerm\"$(NC)"; \
		echo "$(YELLOW)📋 Examples:$(NC)"; \
		echo "  make test-find NAME=\"Multiplayer\"    # Find tests with 'Multiplayer'"; \
		echo "  make test-find NAME=\"Settings\"      # Find tests with 'Settings'"; \
		echo "  make test-find NAME=\"Initialize\"    # Find tests with 'Initialize'"; \
	else \
		echo "$(CYAN)🔍 Searching for tests containing: $(NAME)$(NC)"; \
		docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Name~$(NAME)\" --logger \"console;verbosity=normal\""; \
		echo "$(GREEN)✅ Search completed!$(NC)"; \
	fi

## ⚡ Super quick test - fastest option for development
t:
	@echo "$(CYAN)⚡ Running unit tests (fastest)...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Unit\" --logger \"console;verbosity=minimal\""
	@echo "$(GREEN)✅ Quick tests completed!$(NC)"

## 📊 Test commands with built-in coverage

## 🧪 Unit tests with quick coverage
test-unit-coverage:
	@echo "$(CYAN)🧪 Running unit tests with coverage...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Unit\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Unit/ --logger \"console;verbosity=normal\""
	@echo "$(GREEN)✅ Unit tests with coverage completed!$(NC)"
	@echo "$(YELLOW)📊 Coverage report: ./TestResults/Coverage/Unit/$(NC)"

## 🔗 Integration tests with coverage
test-integration-coverage:
	@echo "$(CYAN)🔗 Running integration tests with coverage...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Integration\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Integration/ --logger \"console;verbosity=normal\""
	@echo "$(GREEN)✅ Integration tests with coverage completed!$(NC)"
	@echo "$(YELLOW)📊 Coverage report: ./TestResults/Coverage/Integration/$(NC)"

## ⚡ Performance tests with coverage
test-performance-coverage:
	@echo "$(CYAN)⚡ Running performance tests with coverage...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Performance\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Performance/ --logger \"console;verbosity=normal\""
	@echo "$(GREEN)✅ Performance tests with coverage completed!$(NC)"
	@echo "$(YELLOW)📊 Coverage report: ./TestResults/Coverage/Performance/$(NC)"

## ⚡ Super quick test with coverage
t-coverage:
	@echo "$(CYAN)⚡ Running unit tests with coverage (fastest)...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Unit\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Quick/ --logger \"console;verbosity=minimal\""
	@echo "$(GREEN)✅ Quick tests with coverage completed!$(NC)"
	@echo "$(YELLOW)📊 Coverage report: ./TestResults/Coverage/Quick/$(NC)"

## 🎯 Universal test runner with coverage
test-run-coverage:
	@if [ -z "$(TARGET)" ]; then \
		echo "$(CYAN)🎯 Universal Test Runner with Coverage$(NC)"; \
		echo "$(YELLOW)Choose test category:$(NC)"; \
		echo "  1) All tests"; \
		echo "  2) Unit tests"; \
		echo "  3) Integration tests"; \
		echo "  4) Performance tests"; \
		echo "  5) Multiplayer tests"; \
		echo "  6) Settings UI tests"; \
		echo "  7) Specific test class"; \
		echo "  8) Specific test method"; \
		echo "  9) Custom filter"; \
		echo ""; \
		read -p "Choice (1-9): " choice; \
		case "$$choice" in \
			1) docker-compose run test bash -c "cd /app/Tests && dotnet test --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/All/ --logger \"console;verbosity=normal\"";; \
			2) docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Unit\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Unit/ --logger \"console;verbosity=normal\"";; \
			3) docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Integration\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Integration/ --logger \"console;verbosity=normal\"";; \
			4) docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Performance\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Performance/ --logger \"console;verbosity=normal\"";; \
			5) docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"MultiplayerDetectionTests\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Multiplayer/ --logger \"console;verbosity=normal\"";; \
			6) docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"SettingsUITests\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Settings/ --logger \"console;verbosity=normal\"";; \
			7) read -p "Enter test class name: " class; \
			   if [ -n "$$class" ]; then \
				   docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$$class\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Class/ --logger \"console;verbosity=normal\""; \
			   else \
				   echo "$(RED)❌ Class name required$(NC)"; \
			   fi;; \
			8) read -p "Enter test method name: " method; \
			   if [ -n "$$method" ]; then \
				   docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Name~$$method\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Method/ --logger \"console;verbosity=normal\""; \
			   else \
				   echo "$(RED)❌ Method name required$(NC)"; \
			   fi;; \
			9) read -p "Enter filter expression: " filter; \
			   if [ -n "$$filter" ]; then \
				   docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$$filter\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Custom/ --logger \"console;verbosity=normal\""; \
			   else \
				   echo "$(RED)❌ Filter expression required$(NC)"; \
			   fi;; \
			*) echo "$(RED)❌ Invalid choice$(NC)";; \
		esac; \
		echo "$(GREEN)✅ Test run with coverage completed!$(NC)"; \
		echo "$(YELLOW)📊 Coverage reports available in ./TestResults/Coverage/$(NC)"; \
	else \
		echo "$(CYAN)🎯 Running tests with coverage: $(TARGET)$(NC)"; \
		docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"$(TARGET)\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Target/ $(OPTS) --logger \"console;verbosity=normal\""; \
		echo "$(GREEN)✅ Tests with coverage completed!$(NC)"; \
		echo "$(YELLOW)📊 Coverage report: ./TestResults/Coverage/Target/$(NC)"; \
	fi

## 🔍 Smart search with coverage
test-find-coverage:
	@if [ -z "$(NAME)" ]; then \
		echo "$(RED)❌ NAME parameter required$(NC)"; \
		echo "$(YELLOW)💡 Usage: make test-find-coverage NAME=\"SearchTerm\"$(NC)"; \
		echo "$(YELLOW)📋 Examples:$(NC)"; \
		echo "  make test-find-coverage NAME=\"Multiplayer\"  # Find and test with coverage"; \
		echo "  make test-find-coverage NAME=\"Settings\"    # Find and test with coverage"; \
	else \
		echo "$(CYAN)🔍 Searching and testing with coverage: $(NAME)$(NC)"; \
		docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Name~$(NAME)\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Search/ --logger \"console;verbosity=normal\""; \
		echo "$(GREEN)✅ Search with coverage completed!$(NC)"; \
		echo "$(YELLOW)📊 Coverage report: ./TestResults/Coverage/Search/$(NC)"; \
	fi

## 🚀 Quick test menu with coverage options
test-quick-coverage:
	@echo "$(CYAN)⚡ Quick Test Menu with Coverage$(NC)"
	@echo "$(YELLOW)What do you want to test?$(NC)"
	@echo "  1) Unit tests with coverage (fast)"
	@echo "  2) Multiplayer detection with coverage"
	@echo "  3) Settings UI with coverage"
	@echo "  4) Everything with coverage"
	@echo ""
	@read -p "Choice (1-4): " choice; \
	case "$$choice" in \
		1) make test-run-coverage TARGET="Category=Unit";; \
		2) make test-run-coverage TARGET="MultiplayerDetectionTests";; \
		3) make test-run-coverage TARGET="SettingsUITests";; \
		4) make test-run-coverage;; \
		*) echo "$(RED)❌ Invalid choice$(NC)";; \
	esac

## 📊 Generate code coverage report (unit tests only - fast)
coverage-basic:
	@echo "$(CYAN)📊 Generating basic coverage for unit tests only...$(NC)"
	@docker-compose run --rm test bash -lc "cd /app/Tests && dotnet restore && dotnet test --filter Category=Unit --collect:'XPlat Code Coverage' --results-directory ./TestResults/Coverage/ --logger 'console;verbosity=minimal'"
	@echo "$(GREEN)✅ Unit test coverage generated in ./Tests/TestResults/Coverage/$(NC)"

## 🚫 Skip coverage entirely (fastest)
coverage-skip:
	@echo "$(CYAN)🚫 Skipping coverage collection for speed...$(NC)"
	@docker-compose run --rm test bash -lc "cd /app && dotnet test Tests/ --logger 'console;verbosity=normal' --no-restore"
	@echo "$(GREEN)✅ Tests completed without coverage collection$(NC)"

## 📊 Generate code coverage report (legacy - may hang)
coverage:
	@echo "$(CYAN)📊 Generating code coverage report...$(NC)"
	@echo "$(YELLOW)⚠️  Warning: This command may hang. Use 'make coverage-basic' for reliable coverage or 'make coverage-skip' for fastest testing.$(NC)"
	@docker-compose run --rm test bash -lc "BLAME=$${COVERAGE_BLAME:-0} VERB=$${COVERAGE_VERBOSITY:-normal} SIMPLE=$${COVERAGE_SIMPLE:-0} DIAG=$${COVERAGE_DIAG:-0} timeout --preserve-status --signal=TERM --kill-after=20s $${COVERAGE_TIMEOUT:-900} bash -lc 'export COVERAGE_ENABLE_BLAME='\\''$$BLAME'\\''; export COVERAGE_VERBOSITY='\\''$$VERB'\\''; export COVERAGE_SIMPLE='\\''$$SIMPLE'\\''; export COVERAGE_DIAG='\\''$$DIAG'\\''; bash /app/Tests/run_coverage.sh'"
	@echo "$(GREEN)✅ Coverage report generated in ./Tests/TestResults/Coverage/$(NC)"
	@echo "$(YELLOW)💡 Find coverage.cobertura.xml in ./Tests/TestResults/Coverage/$(NC)"
	@echo "$(YELLOW)📋 Use 'make coverage-html' for human-readable HTML report$(NC)"

## 🌐 Generate HTML coverage report
coverage-html:
	@echo "$(CYAN)🌐 Generating HTML coverage report...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/ --logger \"console;verbosity=normal\" && if command -v reportgenerator >/dev/null 2>&1; then reportgenerator \"-reports:./TestResults/Coverage/*/coverage.cobertura.xml\" \"-targetdir:./TestResults/Coverage/Html\" \"-reporttypes:Html\"; else echo 'Installing ReportGenerator...'; dotnet tool install --global dotnet-reportgenerator-globaltool && reportgenerator \"-reports:./TestResults/Coverage/*/coverage.cobertura.xml\" \"-targetdir:./TestResults/Coverage/Html\" \"-reporttypes:Html\"; fi"
	@echo "$(GREEN)✅ HTML coverage report generated!$(NC)"
	@echo "$(YELLOW)💡 Open ./TestResults/Coverage/Html/index.html in browser$(NC)"
	@echo "$(YELLOW)📋 If ReportGenerator fails, install it: dotnet tool install --global dotnet-reportgenerator-globaltool$(NC)"

## 📈 Quick coverage check (unit tests only)
coverage-quick:
	@echo "$(CYAN)📈 Quick coverage check (unit tests)...$(NC)"
	@docker-compose run test bash -c "cd /app/Tests && dotnet test --filter \"Category=Unit\" --collect:\"XPlat Code Coverage\" --results-directory ./TestResults/Coverage/Quick/ --logger \"console;verbosity=minimal\""
	@echo "$(GREEN)✅ Quick coverage completed!$(NC)"
	@echo "$(YELLOW)💡 Check ./TestResults/Coverage/Quick/ for results$(NC)"

## 💻 Start development environment
dev:
	@echo "$(CYAN)💻 Starting development environment...$(NC)"
	docker-compose up dev

## 🐚 Enter Docker container shell for debugging
shell:
	@echo "$(CYAN)🐚 Entering Docker container shell...$(NC)"
	docker-compose exec dev /bin/bash

## 📋 Show Docker container logs
logs:
	@echo "$(CYAN)📋 Showing Docker logs...$(NC)"
	docker-compose logs -f

## 🧹 Clean Docker images and containers
clean:
	@echo "$(CYAN)🧹 Cleaning Docker environment...$(NC)"
	docker-compose down --volumes --remove-orphans
	docker system prune -f
	@echo "$(GREEN)✅ Cleanup completed!$(NC)"

## 💥 Deep clean - remove everything
clean-all:
	@echo "$(RED)💥 Deep cleaning Docker environment...$(NC)"
	@echo "$(YELLOW)⚠️  This will remove ALL Docker containers, images, and volumes!$(NC)"
	@read -p "Are you sure? (y/N): " confirm && [ "$$confirm" = "y" ] || exit 1
	docker-compose down --volumes --remove-orphans
	docker system prune -af --volumes
	docker volume prune -f
	@echo "$(GREEN)✅ Deep cleanup completed!$(NC)"

## ✨ Format code using dotnet format
format:
	@echo "$(CYAN)✨ Formatting code...$(NC)"
	docker-compose exec dev dotnet format Source/RimAsync/RimAsync.csproj --verbosity normal
	@echo "$(GREEN)✅ Code formatting completed!$(NC)"

## 🔍 Run code analysis and linting
lint:
	@echo "$(CYAN)🔍 Running code analysis...$(NC)"
	docker-compose exec dev dotnet build Source/RimAsync/RimAsync.csproj --verbosity normal --configuration Debug
	@echo "$(GREEN)✅ Code analysis completed!$(NC)"

## 🧹 Format code and fix style issues automatically
format-fix:
	@echo "$(CYAN)🧹 Auto-fixing code style issues...$(NC)"
	# Run whitespace-only formatting to avoid SDK analyzer crashes
	docker-compose exec dev sh -lc "dotnet format whitespace Source/RimAsync/RimAsync.csproj --verbosity normal"
	@echo "$(GREEN)✅ Auto-fix completed!$(NC)"

## 🔎 Check code style without fixing (whitespace only)
format-check:
	@echo "$(CYAN)🔎 Checking code style...$(NC)"
	docker-compose exec dev sh -lc "dotnet format whitespace Source/RimAsync/RimAsync.csproj --verify-no-changes --verbosity normal"
	@echo "$(GREEN)✅ Code style check completed!$(NC)"

## 📊 Run detailed code analysis report
lint-report:
	@echo "$(CYAN)📊 Generating detailed analysis report...$(NC)"
	docker-compose exec dev dotnet build Source/RimAsync/RimAsync.csproj --verbosity diagnostic --configuration Release > ./CodeAnalysis.log 2>&1 || true
	@echo "$(GREEN)✅ Analysis report generated in CodeAnalysis.log$(NC)"

## 🛡️ Run security analysis
security-lint:
	@echo "$(CYAN)🛡️ Running security analysis...$(NC)"
	docker-compose exec dev dotnet list Source/RimAsync/RimAsync.csproj package --vulnerable --include-transitive --highest-minor
	@echo "$(GREEN)✅ Security analysis completed!$(NC)"

## 📦 Initial project setup
setup: docker-info
	@echo "$(CYAN)📦 Setting up RimAsync development environment...$(NC)"
	@echo "$(YELLOW)📋 Checking prerequisites...$(NC)"
	@command -v docker >/dev/null 2>&1 || { echo "$(RED)❌ Docker is required but not installed.$(NC)" >&2; exit 1; }
	@command -v docker-compose >/dev/null 2>&1 || { echo "$(RED)❌ Docker Compose is required but not installed.$(NC)" >&2; exit 1; }
	@echo "$(GREEN)✅ Docker and Docker Compose are available$(NC)"
	@echo "$(YELLOW)🏗️ Building initial Docker images...$(NC)"
	docker-compose build
	@echo "$(GREEN)✅ Setup completed! Use 'make help' to see available commands.$(NC)"

## 🐳 Show Docker environment info
docker-info:
	@echo "$(CYAN)🐳 Docker Environment Information:$(NC)"
	@echo "$(YELLOW)Docker version:$(NC)"
	@docker --version 2>/dev/null || echo "$(RED)❌ Docker not found$(NC)"
	@echo "$(YELLOW)Docker Compose version:$(NC)"
	@docker-compose --version 2>/dev/null || echo "$(RED)❌ Docker Compose not found$(NC)"
	@echo "$(YELLOW)Docker status:$(NC)"
	@docker info --format "{{.ServerVersion}}" 2>/dev/null && echo "$(GREEN)✅ Docker daemon running$(NC)" || echo "$(RED)❌ Docker daemon not running$(NC)"

# Advanced targets for CI/CD and automation



## 🔒 Security scan
security-scan:
	@echo "$(CYAN)🔒 Running security scan...$(NC)"
	docker-compose exec dev dotnet list package --vulnerable --include-transitive
	@echo "$(GREEN)✅ Security scan completed!$(NC)"

## 📦 Create distribution package (RimWorld 1.6 compatible)
package: build
	@echo "$(CYAN)📦 Creating distribution package...$(NC)"
	@mkdir -p dist/RimAsync/About dist/RimAsync/Assemblies
	@cp -r About/* dist/RimAsync/About/ 2>/dev/null || echo "$(YELLOW)⚠️  About folder not found$(NC)"
	@if [ -f 1.6/Assemblies/RimAsync.dll ]; then \
		cp 1.6/Assemblies/RimAsync.dll dist/RimAsync/Assemblies/; \
		echo "$(GREEN)✅ Copied RimWorld 1.6 compatible DLL$(NC)"; \
	else \
		echo "$(RED)❌ 1.6/Assemblies/RimAsync.dll not found, run 'make build' first$(NC)"; \
		exit 1; \
	fi
	@cp README.md LICENSE dist/RimAsync/ 2>/dev/null || echo "$(YELLOW)⚠️  README.md or LICENSE not found$(NC)"
	@echo "$(GREEN)✅ Package created in dist/RimAsync/ (RimWorld 1.6 compatible)$(NC)"

## 🚀 Install/Update mod in RimWorld mods directory
install: package
	@echo "$(CYAN)🚀 Installing/Updating mod in RimWorld...$(NC)"
	@if [ -f .env ]; then \
		RIMWORLD_MODS=$$(grep '^RIMWORLD_MODS_PATH=' .env | cut -d '=' -f2- | sed 's/^"//;s/"$$//'); \
		RIMWORLD_MODS=$${RIMWORLD_MODS:-$(HOME)/Library/Application Support/RimWorld/Mods}; \
	else \
		echo "$(YELLOW)⚠️  .env file not found, using default path$(NC)"; \
		RIMWORLD_MODS="$(HOME)/Library/Application Support/RimWorld/Mods"; \
	fi; \
	RIMWORLD_MODS=$$(eval echo "$$RIMWORLD_MODS"); \
	if [ ! -d "$$RIMWORLD_MODS" ]; then \
		echo "$(RED)❌ RimWorld mods directory not found: $$RIMWORLD_MODS$(NC)"; \
		echo "$(YELLOW)💡 Please create .env file with RIMWORLD_MODS_PATH variable$(NC)"; \
		echo "$(YELLOW)💡 See .env.example for reference$(NC)"; \
		exit 1; \
	fi; \
	if [ -d "$$RIMWORLD_MODS/RimAsync" ]; then \
		echo "$(YELLOW)📦 Mod found, updating...$(NC)"; \
		rm -rf "$$RIMWORLD_MODS/RimAsync"; \
	else \
		echo "$(YELLOW)📦 Mod not found, installing...$(NC)"; \
	fi; \
	cp -r dist/RimAsync/ "$$RIMWORLD_MODS/RimAsync/"; \
	echo "$(GREEN)✅ Mod installed/updated successfully!$(NC)"; \
	echo "$(CYAN)📍 Location: $$RIMWORLD_MODS/RimAsync/$(NC)"

## 🚀 Deploy to local RimWorld mods directory (macOS) - DEPRECATED, use 'make deploy'
deploy-local: deploy
	@echo "$(YELLOW)⚠️  'deploy-local' is deprecated, use 'make deploy' instead$(NC)"

## ⚡ Quick deploy: Build + Install in one command
deploy: build
	@echo "$(CYAN)⚡ Quick Deploy: Building and installing...$(NC)"
	@$(MAKE) --no-print-directory package-internal
	@$(MAKE) --no-print-directory install-internal
	@echo "$(GREEN)🎉 Deploy complete! Mod is ready to use in RimWorld.$(NC)"

# Internal targets (no logging spam)
package-internal:
	@mkdir -p dist/RimAsync/About dist/RimAsync/Assemblies
	@cp -r About/* dist/RimAsync/About/ 2>/dev/null || true
	@if [ -f 1.6/Assemblies/RimAsync.dll ]; then \
		cp 1.6/Assemblies/RimAsync.dll dist/RimAsync/Assemblies/; \
	else \
		echo "$(RED)❌ Build failed$(NC)"; exit 1; \
	fi
	@cp README.md LICENSE dist/RimAsync/ 2>/dev/null || true

install-internal:
	@if [ -f .env ]; then \
		RIMWORLD_MODS=$$(grep '^RIMWORLD_MODS_PATH=' .env | cut -d '=' -f2- | sed 's/^"//;s/"$$//'); \
		RIMWORLD_MODS=$${RIMWORLD_MODS:-$(HOME)/Library/Application Support/RimWorld/Mods}; \
	else \
		RIMWORLD_MODS="$(HOME)/Library/Application Support/RimWorld/Mods"; \
	fi; \
	RIMWORLD_MODS=$$(eval echo "$$RIMWORLD_MODS"); \
	if [ ! -d "$$RIMWORLD_MODS" ]; then \
		echo "$(RED)❌ RimWorld mods directory not found$(NC)"; exit 1; \
	fi; \
	rm -rf "$$RIMWORLD_MODS/RimAsync" 2>/dev/null || true; \
	cp -r dist/RimAsync/ "$$RIMWORLD_MODS/RimAsync/"; \
	echo "$(GREEN)✅ Installed to: $$RIMWORLD_MODS/RimAsync/$(NC)"

# Development workflow shortcuts

## 🔄 Full development cycle: clean -> build -> test
cycle: clean build test
	@echo "$(GREEN)🎉 Full development cycle completed!$(NC)"

## ⚡ Quick development cycle: build -> test
quick-cycle: quick-build test
	@echo "$(GREEN)🎉 Quick development cycle completed!$(NC)"

## 🛠️ Development mode with file watching
watch:
	@echo "$(CYAN)🛠️ Starting file watcher for automatic rebuild...$(NC)"
	@echo "$(YELLOW)💡 This will rebuild on file changes. Press Ctrl+C to stop.$(NC)"
	docker-compose up dev --watch

# Git integration (works with @create-branch command)

## 🌿 Git status with Docker build info
status:
	@echo "$(CYAN)🌿 Git and Docker Status:$(NC)"
	@echo "$(YELLOW)📋 Git status:$(NC)"
	@git status --short 2>/dev/null || echo "$(RED)❌ Not a git repository$(NC)"
	@echo "$(YELLOW)🏗️ Last build status:$(NC)"
	@[ -f Build/Assemblies/RimAsync.dll ] && echo "$(GREEN)✅ Build exists$(NC)" || echo "$(RED)❌ No build found$(NC)"
	@echo "$(YELLOW)🐳 Docker containers:$(NC)"
	@docker-compose ps
