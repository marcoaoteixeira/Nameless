#!/usr/bin/env bash
# =============================================================================
# build.sh — Nameless build pipeline
#
# Usage:
#   ./build.sh [OPTIONS]
#
# Options:
#   --configuration <name>   Build configuration (default: Release)
#   --version <semver>       Assembly and package version (default: 1.0.0)
#   --output-dir <path>      NuGet package output directory (default: ./artifacts)
#   --pack                   Create NuGet packages after a successful test run
#   --help                   Print this help and exit
#
# Examples:
#   ./build.sh
#   ./build.sh --configuration Debug
#   ./build.sh --version 2.3.1 --pack
#   ./build.sh --version 2.3.1 --pack --output-dir ./nupkgs
# =============================================================================

set -euo pipefail

# -----------------------------------------------------------------------------
# GitHub Actions log helpers
# When running outside CI the annotations are still harmless plain text.
# -----------------------------------------------------------------------------
step_start() { echo "::group::  $*"; }
step_end()   { echo "::endgroup::"; }
log_info()   { echo "  [info]  $*"; }
log_error()  { echo "::error::$*" >&2; }

# -----------------------------------------------------------------------------
# Defaults
# -----------------------------------------------------------------------------
CONFIGURATION="Release"
VERSION="1.0.0"
OUTPUT_DIR="./artifacts"
PACK=false

# Resolve the repo root so the script can be called from any working directory.
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Source projects listed in dependency order:
#   Nameless.Core  <-  Nameless.Impl  <-  Nameless.Web  <-  Nameless.Testing.Tools
#
# Building in this order ensures each project's references are already compiled
# before the dependent project starts.
SRC_PROJECTS=(
    "src/Nameless.Core/Nameless.Core.csproj"
    "src/Nameless.Impl/Nameless.Impl.csproj"
    "src/Nameless.Web/Nameless.Web.csproj"
    "src/Nameless.Testing.Tools/Nameless.Testing.Tools.csproj"
)

# -----------------------------------------------------------------------------
# Argument parsing
# -----------------------------------------------------------------------------
usage() {
    sed -n '/^# Usage:/,/^# ===\+/{ /^# ===/d; s/^# \{0,1\}//; p }' "$0"
    exit 0
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        --configuration)
            CONFIGURATION="${2:?'--configuration requires a value'}"
            shift 2 ;;
        --version)
            VERSION="${2:?'--version requires a value'}"
            shift 2 ;;
        --output-dir)
            OUTPUT_DIR="${2:?'--output-dir requires a value'}"
            shift 2 ;;
        --pack)
            PACK=true
            shift ;;
        --help|-h)
            usage ;;
        *)
            log_error "Unknown argument: $1"
            usage ;;
    esac
done

# -----------------------------------------------------------------------------
# Preconditions
# -----------------------------------------------------------------------------
if ! command -v dotnet &>/dev/null; then
    log_error "dotnet CLI not found. Install it from https://dotnet.microsoft.com/download"
    exit 1
fi

cd "$REPO_ROOT"

log_info "Configuration : $CONFIGURATION"
log_info "Version       : $VERSION"
log_info "Pack          : $PACK"
[[ "$PACK" == true ]] && log_info "Output dir    : $OUTPUT_DIR"
echo

# Discover test projects once so both the Restore and Test steps share the list.
mapfile -t TEST_PROJECTS < <(find tests -name "*.csproj" 2>/dev/null | sort || true)

# =============================================================================
# Step 1 — Restore
# =============================================================================
step_start "Restore"

log_info "Restoring source projects..."
for project in "${SRC_PROJECTS[@]}"; do
    log_info "dotnet restore $project"
    dotnet restore "$project" \
        --verbosity minimal
done

if [[ ${#TEST_PROJECTS[@]} -gt 0 ]]; then
    log_info "Restoring test projects..."
    for project in "${TEST_PROJECTS[@]}"; do
        log_info "dotnet restore $project"
        dotnet restore "$project" \
            --verbosity minimal
    done
fi

step_end

# =============================================================================
# Step 2 — Build
# =============================================================================
step_start "Build"

log_info "Building source projects..."
for project in "${SRC_PROJECTS[@]}"; do
    log_info "dotnet build $project"
    dotnet build "$project" \
        --configuration "$CONFIGURATION" \
        --no-restore \
        --nologo \
        -p:Version="$VERSION" \
        -p:AssemblyVersion="$VERSION" \
        -p:FileVersion="$VERSION" \
        --verbosity minimal
done

step_end

# =============================================================================
# Step 3 — Test
# =============================================================================
step_start "Test"

if [[ ${#TEST_PROJECTS[@]} -eq 0 ]]; then
    log_info "No test projects found under tests/. Skipping."
else
    log_info "Running tests..."
    for project in "${TEST_PROJECTS[@]}"; do
        log_info "dotnet test $project"
        # Test projects are built here (--no-build is intentionally omitted).
        # Source project outputs from Step 2 are reused via incremental build;
        # only the test assembly itself is compiled.
        dotnet test "$project" \
            --configuration "$CONFIGURATION" \
            --no-restore \
            --nologo \
            --verbosity normal \
            --logger "console;verbosity=normal" \
            --collect "XPlat Code Coverage"
    done
fi

step_end

# =============================================================================
# Step 4 — Pack  (opt-in via --pack)
# =============================================================================
if [[ "$PACK" == true ]]; then

    mkdir -p "$OUTPUT_DIR"

    step_start "Pack"

    log_info "Creating NuGet packages -> $OUTPUT_DIR"
    for project in "${SRC_PROJECTS[@]}"; do
        log_info "dotnet pack $project"
        dotnet pack "$project" \
            --configuration "$CONFIGURATION" \
            --no-build \
            --no-restore \
            --nologo \
            --output "$OUTPUT_DIR" \
            -p:PackageVersion="$VERSION" \
            --verbosity minimal
    done

    step_end

fi

# -----------------------------------------------------------------------------
echo
log_info "Build completed successfully."
