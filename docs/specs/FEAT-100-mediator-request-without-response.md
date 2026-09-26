# FEAT-100 — Mediator: requests without a response

**Status:** Approved (D1 recommended option, D3 accepted, same branch `refactor`)
**Scope:** `src/Nameless.Common/Mediator` (+ tests in `tests/Nameless.Common.Tests/Mediator`)
**Feature number:** unknown (assigned: 100) — to be assigned.

## 1. Motivation

Every request currently must produce a `TResponse`. Commands that only need
"do something and finish" have to invent a dummy response type. Add first-class
support for requests that return nothing (`Task`), symmetric to the existing
`IRequest<TResponse>` support, without breaking existing typed requests.

## 2. Current state (verified in code)

| Piece | Today |
|---|---|
| `IRequest` | Marker already added (uncommitted WIP): `interface IRequest;` and `IRequest<out TResponse> : IRequest;` |
| `IRequestHandler<TRequest, TResponse>` | `Task<TResponse> HandleAsync(TRequest, ct)`; `TRequest : IRequest<TResponse>` |
| `IRequestHandlerInvoker` | `Task<TResponse> ExecuteAsync<TResponse>(IRequest<TResponse>, ct)` |
| `IRequestPipelineBehavior<TRequest, TResponse>` | `TRequest : notnull` |
| `RequestHandlerDelegate<TResponse>` | `Task<TResponse>(ct)` |
| `RequestHandlerWrapper` (non-generic) | `Task<object?> HandleAsync(object, provider, ct)` |
| `RequestHandlerWrapper<TResponse>` | adds `Task<TResponse> HandleAsync(IRequest<TResponse>, provider, ct)` |
| `RequestHandlerInvoker` | `ConcurrentDictionary<Type, RequestHandlerWrapper> _cache` keyed by request runtime type |
| `IMediator` | `IRequestHandlerInvoker + IEventHandlerInvoker + IStreamHandlerInvoker`; `MediatorImpl` delegates |
| Registration | `MediatorRegistration` / `ServiceCollectionExtensions.RegisterMediator` only know `IRequestHandler<,>` and `IRequestPipelineBehavior<,>`; optional `ValidateRequestPipelineBehavior<,>` |
| Consumers outside Common | `Nameless.Windows` use cases (typed requests only) — unaffected |

## 3. Requirements

### 3.1 Contracts (namespace `Nameless.Mediator.Requests`)
1. `IRequest` — already exists; keep `IRequest<out TResponse> : IRequest`.
2. New `IRequestHandler<in TRequest>` where `TRequest : IRequest`:
   `Task HandleAsync(TRequest request, CancellationToken cancellationToken)`.
3. `IRequestHandlerInvoker` gains
   `Task ExecuteAsync(IRequest request, CancellationToken cancellationToken)`;
   the typed overload is unchanged. `IMediator`/`MediatorImpl` get it through
   inheritance/delegation.
4. New `delegate Task RequestHandlerDelegate(CancellationToken)`.
5. New `IRequestPipelineBehavior<in TRequest>` where `TRequest : IRequest`:
   `Task HandleAsync(TRequest, RequestHandlerDelegate next, CancellationToken)`.
   Existing `IRequestPipelineBehavior<TRequest, TResponse>` constraint changes
   `notnull` → `IRequest<TResponse>` (**breaking for implementers**, see §6).
6. `RequestHandlerWrapper` (non-generic base): `Task HandleAsync(IRequest request, IServiceProvider provider, CancellationToken ct)`
   (replaces `Task<object?> HandleAsync(object, …)`). `RequestHandlerWrapper<TResponse>`
   keeps `Task<TResponse> HandleAsync(IRequest<TResponse>, …)`.
7. New `RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper`
   (`TRequest : IRequest`): resolves `IRequestPipelineBehavior<TRequest>`
   (reversed, aggregated, same token-substitution rule as today) around
   `IRequestHandler<TRequest>`.
   `RequestHandlerWrapperImpl<TRequest, TResponse>` implements the inherited
   `Task HandleAsync(IRequest, …)` by awaiting its typed method and discarding
   the result.

### 3.2 Invoker
- `RequestHandlerInvoker` keeps **one** `_cache` (`ConcurrentDictionary<Type, RequestHandlerWrapper>`) shared by both overloads.
- Typed overload: unchanged behaviour (cast cached wrapper to `RequestHandlerWrapper<TResponse>`).
- New overload: `Throws.When.Null(request)`; get/create wrapper for `request.GetType()`; call the base `HandleAsync(IRequest, …)`.
- Wrapper creation is decided **by the request runtime type** (see D1), so a
  given request type always maps to exactly one wrapper regardless of which
  overload created it first.

### 3.3 Registration
- `MediatorRegistration`:
  - `RequestHandlers` scan covers both `IRequestHandler<,>` and `IRequestHandler<>`.
  - `WithRequestHandler(Type)` accepts a type assignable to either; new
    `WithRequestHandler<THandler, TRequest>()` (arity 2) next to the existing arity 3.
  - `WithRequestPipelineBehavior(Type)` accepts either behavior interface; new
    `WithRequestPipelineBehavior<TBehavior, TRequest>()`.
- `ServiceCollectionExtensions.RegisterMediator`: register handlers and
  behaviors for **both** interface families. `RegisterHandlers` must only
  register an open-generic implementation under the service definition it
  actually implements (today it registers every open type under the single
  service passed in — harmless with one family, wrong with two).
- `UseValidateRequestPipelineBehavior` adds a new
  `ValidateRequestPipelineBehavior<TRequest>` (void) in addition to
  `ValidateRequestPipelineBehavior<TRequest, TResponse>`.

### 3.3.1 Validate behavior
`ValidateRequestPipelineBehavior<TRequest>` mirrors the typed one: validate →
`await next(ct)` on success; log + `throw new ValidationException(result)` on failure.
Typed behavior constraint becomes `IRequest<TResponse>`.

## 4. Decisions / questions (please confirm)

**D1 — Void overload called with a request that implements `IRequest<T>`.**
Because `IRequest<T> : IRequest`, `ExecuteAsync(IRequest)` accepts typed
requests (e.g. through a variable statically typed as `IRequest`).
- *Recommended:* run the **typed** wrapper and discard the result. The
  factory inspects the runtime type: if it implements exactly one
  `IRequest<T>` create `RequestHandlerWrapperImpl<TRequest, T>`, otherwise
  `RequestHandlerWrapperImpl<TRequest>`. One wrapper per type ⇒ shared cache
  is consistent; this is the reason the base wrapper method takes `IRequest`
  and returns `Task`.
- *Alternative:* reject with `ArgumentException` (fail fast; base-class
  method on typed wrappers would be dead code).
- A type implementing several `IRequest<T>` closings → `InvalidOperationException` (ambiguous), in both options.

**D2 — Overload resolution.** For a static type `IRequest<string>` the compiler
picks the typed generic overload (better conversion), so existing call sites do
not change. Only expressions statically typed `IRequest`/non-generic classes hit
the new overload. No `ExecuteAsync` rename needed.

**D3 — Pipeline constraint change is breaking.** Implementers of
`IRequestPipelineBehavior<,>` with `where TRequest : notnull` stop compiling
(constraint must be `IRequest<TResponse>`). In repo: `ValidateRequestPipelineBehavior<,>`
and the `PassThroughRequestBehavior<,>` test double. Accept as intended (per request).

**D4 — Names.** Same-name/different-arity types (`IRequestHandler<T>` vs
`<T,R>`, `RequestHandlerDelegate` vs `<T>`, `ValidateRequestPipelineBehavior<T>` vs `<T,R>`)
follow the existing `IRequest`/`IRequest<T>` convention.

**D5 — Streams.** Out of scope (`IStream` untouched).

**D6 — Docs location.** Spec/plan live in `docs/specs` and `docs/plans` as requested
(CLAUDE.md names `.claude/specs/...`). Feature number: 100.

## 5. Edge cases
- `null` request → `ArgumentNullException` (both overloads).
- No handler registered → `InvalidOperationException` from `GetRequiredService`
  (unchanged semantics).
- Cancellation: pipeline token substitution rule preserved
  (`token == CancellationToken.None ? original : token`).
- Concurrency: `GetOrAdd` on shared cache; wrapper instances stateless.
- Behavior order: registration order, outermost first (same as today).
- Handler throws → exception propagates unwrapped.
- Open-generic void handlers/behaviors resolve via DI open-generic closing.
- Request type implementing both `IRequest<A>` and being registered for
  `IRequestHandler<TRequest>` (void) — not supported; typed wins (D1).

## 6. Compatibility / impact
- Source-breaking: behavior constraint (D3); `RequestHandlerWrapper.HandleAsync(object…)` signature (internal-ish, public class).
- Additive: everything else. `Nameless.Windows` typed requests compile unchanged.
- Public members added to `IRequestHandlerInvoker` (and therefore `IMediator`): external implementers/fakes must add the method.

## 7. Security / performance
- No new input surface. Reflection (`MakeGenericType`) happens once per request type (cached).
- Extra type inspection only at wrapper creation (D1), not per call.

## 8. Test strategy (TDD, `[UnitTest]`)
See plan. Target ≥90% line coverage for all touched classes (project-wide rule).
