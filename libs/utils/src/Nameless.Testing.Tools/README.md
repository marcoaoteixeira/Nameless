# Nameless.Testing.Tools

Shared test building blocks for xUnit-based Nameless test projects: test
categorization attributes, mocking helpers, in-memory logging, and a
disposable temp-directory helper.

## Test categorization

Apply one of these traits to a test class or method to categorize it,
matching the project's testing conventions:

- `[UnitTest]` — isolated tests with no external dependencies.
- `[IntegrationTest]` — tests that cross a component or process boundary.
- `[E2E]` — end-to-end tests.

Other attributes available:

- `[Bug]` — marks a test written to reproduce/guard a specific bug.
- `[RunsOnDevMachine]` — flags a test that only runs on a developer machine.
- `[SymlinkFact]` — an xUnit `[Fact]` that skips when the host cannot
  create filesystem symlinks (e.g. unprivileged Windows CI).
- `[WindowsFact]` — an xUnit `[Fact]` that skips on non-Windows hosts.

## Mocking helpers

```csharp
var service = Quick.Mock<IMyService>(); // loose mock
var strict = Quick.Mock<IMyService>(strict: true);
```

`Mocker` / `MockWrapper` wrap Moq mocks for scenarios that need to track
and verify calls across a test's lifetime.

## In-memory logging

`InMemoryLogger` / `InMemoryLoggerFactory` capture `ILogger` output as
`LogEntry` records, so a test can assert on what was logged without a
real logging sink.

## Temp directories

```csharp
using var dir = new TempDirectory("my-test-prefix");
// dir.Path exists and is deleted (recursively) when disposed.
```
