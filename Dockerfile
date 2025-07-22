# RimAsync Docker Build Environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install additional tools for RimWorld modding
RUN apt-get update && apt-get install -y \
    unzip \
    wget \
    git \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy project files
COPY Source/ ./Source/
COPY Tests/ ./Tests/
COPY About/ ./About/
COPY Planning/ ./Planning/
COPY .cursor/ ./.cursor/

# Copy project configuration files
COPY *.md ./
COPY .gitignore ./

# Set build configuration
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1

# Create output directories
RUN mkdir -p /app/Build/Assemblies
RUN mkdir -p /app/Build/About

# Build the project
WORKDIR /app/Source/RimAsync
RUN dotnet restore
RUN dotnet build --configuration Release --output /app/Build/Assemblies

# Copy About files to build output
RUN cp -r /app/About/* /app/Build/About/

# Test stage
FROM build AS test
WORKDIR /app

# Install NUnit console runner
RUN dotnet tool install --global NUnit.ConsoleRunner --version 3.16.3
ENV PATH="$PATH:/root/.dotnet/tools"

# Run tests
RUN dotnet test Tests/ --configuration Release --logger "console;verbosity=detailed"

# Production stage - minimal image with just the compiled mod
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS runtime

WORKDIR /mod

# Copy built assemblies and About files
COPY --from=build /app/Build/ ./

# Create version info
RUN echo "Built: $(date)" > ./Build-Info.txt
RUN echo "Docker: $(dotnet --version)" >> ./Build-Info.txt

# Volume for mod output
VOLUME ["/mod/output"]

# Default command copies mod to output volume
CMD ["sh", "-c", "cp -r /mod/* /mod/output/ && echo 'RimAsync mod copied to output volume'"] 