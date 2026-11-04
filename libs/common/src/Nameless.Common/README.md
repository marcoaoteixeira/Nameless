# Nameless.Common

Implementation-agnostic building blocks shared across Nameless
applications: host bootstrapping, result/error modeling, resilience,
Lucene search, a lightweight mediator, EF Core helpers, and common
guard/extension utilities.

## Host features

`HostFactory` builds an `IHost` from a collection of `HostFeature`
instances — each feature registers the services it needs against the
`IHostApplicationBuilder` (logging, EF Core, Lucene, resilience, periodic
workers, the mediator, ...). A `HostFeature<TOptions>` feature can be
configured before the host is built:

```csharp
var host = HostFactory.Create(args, features => features
    .Add(new EntityFrameworkCoreHostFeature<AppDbContext>()
        .Configure(options => options.ConnectionString = "..."))
    .Add(new LuceneHostFeature())
    .Add(new MediatorHostFeature()));
```

## Result and error modeling

`Result` / `Result<T>` represent an operation's outcome without throwing
for expected failures; `Error` / `ErrorType` describe why an operation
failed. `ResultExtensions` adds functional-style composition helpers.

## Resilience

`IRetryPipelineFactory` builds retry pipelines (`IRetryPipeline`) with a
configurable `BackoffType` for transient-failure handling, independent of
any specific I/O or HTTP stack.

## Mediator

`IMediator` and the `Mediator*` types provide a minimal in-process
request/notification dispatcher, with assembly scanning to discover
handlers (`AssemblyScanAware`, `SkipAssemblyScanAttribute`).

## EF Core helpers

`AuditableSaveChangesInterceptor` stamps created/modified metadata on
save; `IDatabaseSeeder` implementations (`JsonResourceDatabaseSeeder`,
`ResourceDatabaseSeeder`) seed a database from embedded resources via
`DatabaseSeederAggregator`.

## Lucene

`IIndexProvider` / `IIndex` / `IQueryBuilder` wrap Lucene.NET indexing and
querying behind an abstraction that a `LuceneHostFeature` wires up.

## Guards and extensions

`Throws.When` provides argument-validation guards (null, empty,
out-of-range, ...) used throughout the Nameless libraries. The
`Extensions` namespace adds helpers over `string`, `DateTime`,
`DateOnly`, `Enum`, `Type`, `Version`, `Task`, `IServiceCollection` and
`IServiceProvider`.
