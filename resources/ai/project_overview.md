## Solution Overview

Nameless is a collection of reusable, implementation-agnostic .NET libraries targeting **net10.0**. The solution is structured as three layered library projects under `src/`, plus a microservice template under `templates/`. There is also test projects targeting previous projects under `tests/`.

```
src/
  Nameless.Core/          # Abstractions, interfaces, extension methods, helpers
  Nameless.Core.Impl/     # Concrete implementations (depends on Nameless.Core)
  Nameless.Web.Core/      # ASP.NET Core extensions/utilities (depends on Nameless.Core.Impl)
templates/
  Microservice/           # Starter template for a new microservices
tests/
  Nameless.Core.Tests/          # Tests targeting abstractions, extension methods, helpers in Nameless.Core
  Nameless.Core.Impl.Tests/     # Tests targeting the concrete implementations in Nameless.Core.Impl
  Nameless.Web.Core.Tests/      # Tests targeting extensions/utilities in Nameless.Web.Core
```

**Dependency chain:** `Nameless.Core` ← `Nameless.Core.Impl` ← `Nameless.Web.Core`

## Architecture

### Project: `Nameless.Core`
Pure abstractions and utilities — no concrete dependencies beyond `Microsoft.Extensions.Hosting.Abstractions` and `Microsoft.Extensions.Options.ConfigurationExtensions`. Key subsystems:

- **Contracts / Guard clauses** — `Throws.When` (partial class split across `Contracts/Throws.*.cs`) provides fluent guard clause assertions (e.g., `Throws.When.Null(value)`)
- **Mediator** — `IMediator` composes `IRequestHandlerInvoker`, `IEventHandlerInvoker`, and `IStreamHandlerInvoker` for CQRS-style request/event/stream dispatch
- **Result pattern** — `Result<T>` and `Switch<TArg0, TArg1>` discriminated unions for error-as-data (see `src/Nameless.Core/Results/README.md`)
- **Bootstrap** — `IBootstrapper` + `FlowContext` / `StepProgress` for structured async app initialization
- **ProducerConsumer** — `IProducer` / `IProducerFactory` interfaces for message-based communication
- **Null objects** — `NullDisposable`, `NullProgress`, `NullDictionary`, `NullServiceProvider`, etc.
- **Collections** — `ICircularBuffer<T>`, `IPage<T>`, `QueryableExtensions.CreatePage<T>`
- **Extensions** — helpers on `Array`, `byte[]`, `DateTime`, `Enum`, `Exception`, `Task`, `XContainer`, and more
- **Registration** — `AssemblyScanAware` / `IgnoreAssemblyScanAttribute` for DI container scanning

### Project: `Nameless.Core.Impl`
Concrete implementations backed by MailKit (email), RabbitMQ.Client (messaging), FluentValidation, Polly, EF Core, Serilog, and SQL Server. Mirrors the abstraction folders in `Nameless.Core` (e.g., `Mailing/Mailkit/`, `ProducerConsumer/RabbitMQ/`, `Mediator/`, `Logging/`, `Validation/`).

### Project: `Nameless.Web.Core`
ASP.NET Core (framework reference) utilities including: minimal endpoints, health checks, CORS, rate limiting, output caching, request timeout, JWT auth helpers, OpenTelemetry, service discovery, OpenAPI/Scalar, and Serilog ASP.NET integration.

## Project Specific Global Build Settings (`Directory.Build.props`)

- Projects under `src/` are NuGet-packable (set `IsPackable=True` via `IsSrcProject` detection)
- Projects under `tests/` disable nullable and use `xunit.v3`; `Nullable=disable` applies automatically

## Project Specific Coding Conventions

- Use `Throws.When.*` (guard clauses) instead of inline `ArgumentException`/`if`-null checks
- Use the `Result<T>` / `Switch<TArg0, TArg1>` pattern instead of exceptions for expected failures
