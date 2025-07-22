# RimAsync Makefile
# Convenient commands for Docker-based development

.PHONY: help build test dev quick-build release clean logs shell format lint setup

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
	@echo ""
	@echo "$(GREEN)🚀 Development Commands:$(NC)"
	@echo "  $(YELLOW)make dev$(NC)          - Start development environment"
	@echo "  $(YELLOW)make shell$(NC)        - Enter Docker container shell"
	@echo "  $(YELLOW)make logs$(NC)         - Show Docker container logs"
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
	docker-compose exec dev dotnet format Source/RimAsync/RimAsync.csproj --fix-style --fix-analyzers --verbosity normal
	@echo "$(GREEN)✅ Auto-fix completed!$(NC)"

## 🔎 Check code style without fixing
format-check:
	@echo "$(CYAN)🔎 Checking code style...$(NC)"
	docker-compose exec dev dotnet format Source/RimAsync/RimAsync.csproj --verify-no-changes --verbosity normal
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

## 🚨 Run tests with coverage report
test-coverage:
	@echo "$(CYAN)🚨 Running tests with coverage...$(NC)"
	docker-compose exec test dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults/Coverage/
	@echo "$(GREEN)✅ Coverage report generated in ./TestResults/Coverage/$(NC)"

## 📊 Generate test report
test-report:
	@echo "$(CYAN)📊 Generating test report...$(NC)"
	docker-compose exec test dotnet test --logger "html;LogFileName=TestReport.html" --results-directory ./TestResults/Reports/
	@echo "$(GREEN)✅ Test report generated in ./TestResults/Reports/$(NC)"

## 🔒 Security scan
security-scan:
	@echo "$(CYAN)🔒 Running security scan...$(NC)"
	docker-compose exec dev dotnet list package --vulnerable --include-transitive
	@echo "$(GREEN)✅ Security scan completed!$(NC)"

## 📦 Create distribution package
package: release
	@echo "$(CYAN)📦 Creating distribution package...$(NC)"
	@mkdir -p dist/
	@cp -r Build/ dist/RimAsync/
	@cp About/ dist/RimAsync/About/
	@cp README.md LICENSE dist/RimAsync/
	@echo "$(GREEN)✅ Package created in dist/RimAsync/$(NC)"

## 🚀 Deploy to local RimWorld mods directory (macOS)
deploy-local: package
	@echo "$(CYAN)🚀 Deploying to local RimWorld mods...$(NC)"
	@RIMWORLD_MODS="$(HOME)/Library/Application Support/RimWorld/Mods"; \
	if [ -d "$$RIMWORLD_MODS" ]; then \
		cp -r dist/RimAsync/ "$$RIMWORLD_MODS/RimAsync/"; \
		echo "$(GREEN)✅ Deployed to $$RIMWORLD_MODS/RimAsync/$(NC)"; \
	else \
		echo "$(RED)❌ RimWorld mods directory not found: $$RIMWORLD_MODS$(NC)"; \
		echo "$(YELLOW)💡 Please install RimWorld or create the mods directory manually$(NC)"; \
	fi

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