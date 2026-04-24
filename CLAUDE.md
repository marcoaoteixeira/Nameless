# CLAUDE.md

## AI Behavior Rules

**Always:**
- Provide clear, concise, technically accurate responses
- Follow required output schemas and formats
- Ask for clarification only when essential
- Provide actionable and specific insights when asked

**Never:**
- Invent system behavior or hallucinate design details not provided
- Produce speculative or fictional logs
- Output code invalid in C# / .NET contexts (unless explicitly asked)
- Provide harmful instructions or security-sensitive details

## Project Overview

**Name**: NAMELESS
**Default Branch**: main

A collection of reusable, implementation-agnostic .NET libraries targeting **net10.0**.

### Solution Structure

```
src/
  Nameless.Core/           # Abstractions, interfaces, extension methods, helpers
  Nameless.Impl/           # Concrete implementations
  Nameless.Testing.Tools/  # Reusable classes, mockers, and utilities for testing
  Nameless.Web/            # ASP.NET Core extensions/utilities
  Nameless.WPF/            # WPF library for Windows-based applications
tests/
  Nameless.Core.Tests/     # Tests for abstractions, extension methods, helpers
```

## Common Commands

| Purpose | Command |
|---|---|
| Build | `dotnet build` |
| Test | `dotnet test` |
| EF Core Migrations | `dotnet ef migrations` |
| EF Core Update DB | `dotnet ef database update` |

## Build & Code Analysis

- `EnableNETAnalyzers=True`, `EnforceCodeStyleInBuild=True` — code style is enforced at **build time**; fix analyzer warnings, don't suppress them
- Package versions are centrally managed in `Directory.Packages.props` — never add version attributes to individual `<PackageReference>` elements

## Coding Conventions

These take precedence over general conventions.

- **Async/Await:** Use `SkipContextSync()` (equivalent to `ConfigureAwait(false)`) in all library/helper code; omit in app entry points and UI code.
- **Null Checks:** Use `Throws.When.Null(param)` for objects; use `Throws.When.NullOrWhitespace(param)` for strings. Guard at the top of the method. Avoid the null-forgiving operator (`!`).

## Testing Conventions

- Categorize every test with the appropriate attribute:
  - `[UnitTest]` — isolated unit tests
  - `[IntegrationTest]` — tests that cross component/process boundaries
  - `[E2E]` — end-to-end tests
- Do **not** enable `#nullable` in test projects — tests must be able to pass `null` without compiler warnings.

## Feature Development Workflow

**MUST** be followed always when developing a `new feature`.
Follow these steps **in order**. Do not skip or reorder. Do not write code until step 7.

1. **Cleanup** — Switch to default branch and pull. If there is uncommited work, ask user for directions.
2. **Clarify** — Understand the task; ask questions when any requirement is unclear.
3. **Brainstorm** — Propose clean, organized solutions. Cover security, performance, and edge cases. Explain reasoning.
4. **Spec** — Create `.claude/specs/features/FEAT-XXX-<slug>.md` capturing agreed-upon requirements and decisions.
5. **Plan** — Create `.claude/specs/plans/FEAT-XXX-<slug>.md` with an ordered implementation plan and edge cases.
6. **Explain** — Describe the approach in plain language: what changes, why, in what order, and what edge cases are covered.
7. **Branch** — Propose branch name (format: `feature/NAMELESS-FEAT_NUMBER-<slug>`), confirm with user, then create from `main`.
8. **Code** — Implement using Test-Driven Development (TDD): write failing tests first, then make them pass.
9. **Review** — Run `/code-reviewer` skill. Address every **🔴 Critical Issue** before proceeding.
10. **User Check** — Ask the user to review the code; do not continue until approved.
11. **Commit** — Use conventional commit format. Message **must** be ≤ 120 chars and include project name and feature number:
    ```
    feat(NAMELESS-FEAT_NUMBER): concise description of what changed
    ```
12. **Close** — Update the feature status to `Done` in `.claude/specs/README.md`.
13. **Push** — Push changes.
