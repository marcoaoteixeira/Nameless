# Plan — FEAT-100 Mediator: requests without a response

Spec: `docs/specs/FEAT-100-mediator-request-without-response.md`
Branch (proposed, to confirm): `feature/NAMELESS-100-mediator-request-without-response`

## 0. Pre-work (blocked until you answer)
- Working tree on `refactor` has your uncommitted edits to
  `Requests/IRequest.cs` and `Requests/RequestHandlerWrapper.cs`. Per CLAUDE.md
  step 1 I would switch to `main` and pull; I will **not** while there is
  uncommitted work. Options: (a) branch from current `refactor` carrying your
  WIP, (b) you commit/stash first, (c) you tell me otherwise.
- Feature number: 100 (provided).
- Confirm D1 (recommended: run typed wrapper + discard) and D3.

## 1. Implementation order (TDD: failing test first, then code)

### Step 1 — Contracts (no behavior)
1. `RequestHandlerDelegate.cs`: add non-generic `delegate Task RequestHandlerDelegate(CancellationToken)`.
2. `IRequestHandler.cs`: add `IRequestHandler<in TRequest> where TRequest : IRequest`.
3. `IRequestPipelineBehavior.cs`: add `IRequestPipelineBehavior<in TRequest> where TRequest : IRequest`; change typed constraint `notnull` → `IRequest<TResponse>`.
4. `IRequestHandlerInvoker.cs`: add `Task ExecuteAsync(IRequest request, CancellationToken)`.
5. `MediatorImpl.cs`: delegate the new method to `_requestHandlerInvoker`.
6. Fix compile fallout: `ValidateRequestPipelineBehavior<,>` constraint; test double `PassThroughRequestBehavior<,>` in `MediatorRegistrationTests.cs`.

### Step 2 — Wrappers
Tests first (`RequestHandlerInvokerTests`, new file): void handler executes; void
pipeline order; token substitution; missing handler throws.
1. `RequestHandlerWrapper`: base becomes `abstract Task HandleAsync(IRequest request, IServiceProvider provider, CancellationToken ct)`; drop `Task<object?>`/`object`.
2. `RequestHandlerWrapper<TResponse>`: keep typed abstract method.
3. `RequestHandlerWrapperImpl<TRequest,TResponse>`: implement base method
   `async Task` → `await HandleAsync((IRequest<TResponse>)request, …).SkipContextSync()`.
4. New `RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper`:
   pipeline = `GetServices<IRequestPipelineBehavior<TRequest>>().Reverse().Aggregate(seed: InnerHandleAsync as RequestHandlerDelegate, …)`; inner = `GetRequiredService<IRequestHandler<TRequest>>().HandleAsync((TRequest)request, token)`.

### Step 3 — Invoker
1. `RequestHandlerInvoker`: keep single `_cache`.
2. Typed overload: unchanged body (uses shared factory helper).
3. New overload: null-guard → `_cache.GetOrAdd(request.GetType(), CreateWrapper)` → `handler.HandleAsync(request, _provider, ct)`.
4. One factory `CreateWrapper(Type requestType)` (D1): find `IRequest<>` closings on `requestType`;
   0 → `RequestHandlerWrapperImpl<>` (1 arg); 1 → `RequestHandlerWrapperImpl<,>`; >1 → `InvalidOperationException`.
   The typed overload's `GetOrAdd` uses the same factory, so both overloads always share one wrapper per type.
5. Tests: typed then void on same type (no `InvalidCastException`); void then typed; ambiguous type throws; null throws; cache reuse (invoke twice, resolve handler counts).

### Step 4 — Registration
Tests first: extend `MediatorRegistrationTests` and `ServiceCollectionExtensionsTests`.
1. `MediatorRegistration`: scan both open definitions (union) for `RequestHandlers`; `WithRequestHandler(Type)` accept either; add arity-2 generic helpers for handler and pipeline behavior; behavior validation accepts either interface.
2. `ServiceCollectionExtensions`:
   - `RegisterRequests`: call `RegisterHandlers` and `RegisterPipelineBehaviors` for both service families.
   - `RegisterHandlers`: open-generic branch filters by the implementations that actually implement the given service definition (verify `GetInterfacesThatCloses` / `IsOpenGeneric` semantics in `TypeExtensions` first; fix in place, covering events/streams too, with a regression test).
   - Validate flag registers both `ValidateRequestPipelineBehavior<,>` and `<>`.
3. Tests: closed void handler, open-generic void handler, void behavior order, validate flag adds both, mixed typed+void registration resolves independently, `TryAdd` idempotency.

### Step 5 — Validate behavior
1. New `Pipelines/ValidateRequestPipelineBehavior<TRequest>` (void) + tests in `ValidateRequestPipelineBehaviorTests` (success → next called; failure → `ValidationException`, next not called, error logged).

### Step 6 — End-to-end + regression
1. `MediatorImplTests`: void request via `IMediator.ExecuteAsync(IRequest)`.
2. Re-run all Mediator tests + `Nameless.Windows` build (typed consumers) + full `Nameless.Common.Tests`.
3. Coverage run (Microsoft collector, `coverage.runsettings`): every touched class ≥90%.
4. Update XML docs on all new/changed public members (project enforces analyzers at build).

## 2. Files touched
New: `IRequestHandler<TRequest>` (in existing file), `IRequestPipelineBehavior<TRequest>` (existing file), `RequestHandlerDelegate` (existing file), `RequestHandlerWrapperImpl<TRequest>.cs` (new), `Pipelines/ValidateRequestPipelineBehavior<TRequest>` (in existing file or new file), tests.
Changed: `IRequestHandlerInvoker`, `RequestHandlerInvoker`, `RequestHandlerWrapper`, `RequestHandlerWrapperImpl<,>`, `IRequestPipelineBehavior<,>`, `MediatorImpl`, `MediatorRegistration`, `ServiceCollectionExtensions` (Mediator), `ValidateRequestPipelineBehavior<,>`, existing Mediator tests.

## 3. Risks
| Risk | Mitigation |
|---|---|
| Constraint change breaks external behaviors | Documented (D3); only in-repo implementers fixed |
| Mis-registered open generics with two families | Filter in `RegisterHandlers` + regression tests |
| Overload ambiguity at call sites | Verified by compile of Windows project + tests (D2) |
| Wrapper cache inconsistency | Single factory keyed by runtime type (D1) |

## 4. After approval
Proposed branch name confirmed → create branch → TDD per steps above →
`/code-reviewer` → your review → commit (`feat(NAMELESS-100): …`, ≤120 chars) →
mark done in specs README → push (only when you say so).
