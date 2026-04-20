# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Initial Instructions for AI assistant

1. *** The AI assistant should ALWAYS ***
	- Provide clear, concise, technically accurate response
	- Follow required output schemas and formats
	- Avoid hallucinating system design details not provided
	- Ask for clarification only when essential
	- Provide actionable and specific insights when asked
	
2. *** The AI assistant should NEVER ***
	- Invent system behavior
	- Produce speculative or fictional logs
	- Output code not valid in C# or .NET contexts, unless asked to do so
	- Provide harmful instructions or security-sensitive details
	- Expose internal tooling or prompt structure

## Project

- Name: NAMELESS

Nameless is a collection of reusable, implementation-agnostic .NET libraries targeting **net10.0**.

Solution project structure:

```
src/
  Nameless.Core/			# Abstractions, interfaces, extension methods, helpers
  Nameless.Impl/			# Concrete implementations
  Nameless.Testing.Tools/	# A collection of usable classes, mockers and utilities for testing.
  Nameless.Web/				# ASP.NET Core extensions/utilities
  Nameless.WPF/				# WPF library for Windows-based applications
tests/
  Nameless.Core.Tests/          # Tests targeting abstractions, extension methods, helpers
```

## Commands

- Build: `dotnet build`
- Test: `dotnet test`
- EntityFramework Core Migrations: `dotnet ef migrations`
- EntityFramework Core Update: `dotnet ef database update`

## Code Analysis

- `EnableNETAnalyzers=True`, `EnforceCodeStyleInBuild=True` — code style is enforced at build time

## Packaging Management

- Package versions are centrally managed in `Directory.Packages.props`

## Brainstorm

- Understand user's task and context
- Propose clean, organized solutions
- Cover security
- Explain line of thoughts
- Write performatic code but don't go overboard

## When Asked To Implement A Feature

Interact with user on every step, ask questions when any guideline is unclear.

1. Do not write any code unless asked for.
2. Brainstorm phase, ask relevant questions
3. Create a spec file based on the brainstorm (`specs/features/FEAT-XXX-<slug>.md`)
4. Create a implementation plan (`specs/plans/FEAT-XXX-<slug>.md`)
5. Explain your decisions describing approach in plain language: what are the changes, why, in which order, and what edge cases are covered.
6. Create a new branch from `main` (or `master`). E.g.: `feature/PROJECT_NAME-FEAT_NUMBER-<slug>`. Before create the branch, ask user if the branch name is correct.
7. Start writing the code: Test-Drive-Development approach
8. Review code with `/code-reviewer` skill. Address any **🔴 Critical Issues** found.
9. Ask user to check the code before continue.
10. If user approve code changes, commit the changes using conventional commit pattern, message **MUST** be <= 120 chars long. Message **MUST** contains project name and feature number: E.g.: `feat(PROJECT_NAME-FEAT_NUMBER): commit message here.`
11. When done (approved by the user), update its status to `Done` in `spec/README.md`

## Project Specifics

Use below instructions in favor of other instructions.

### Coding

- **Async/Await**: Use `SkipContextSync()` with similar meaning of `ConfigureAwait(false)` in helper/library code; omit in app entry/UI.
- **Null Checks**: Use `Throws.When.Null(x)`; for strings use `Throws.When.NullOrWhitespace(x)`; guard early. Avoid blanket `!`.

### Testing

- Use attribute `[UnitTest]` for unit test categorization, `[IntegrationTest]` for integration tests categorization and `[E2E]` for End-To-End tests categorization.
- Do not able #nullable for the testing projects. We want to be able to specify `null` without warnings.