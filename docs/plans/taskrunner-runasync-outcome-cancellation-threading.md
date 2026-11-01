# TaskRunner: real RunAsync completion, distinct cancellation, threading contract

- Ticket: none (fix work from a code review of `libs/windows/src/Nameless.Windows/TaskRunner/`)
- Stack: backend-style .NET library, WPF (`net10.0-windows`, `UseWPF`), project `Nameless.Windows`
- Agent pair: `backend-developer-dotnet` / `backend-reviewer-dotnet` (no Angular/Android frontend involved)
- Scope rule from the user: basic and minimal. The builder pattern, `IWindowFactory`, and the
  notification pub/sub (`SubscribeFor` / `IEventPublisher`) stay as they are. No new abstraction layers.

## 1. Context: what the code does today

Files I checked: `ITaskRunner.cs`, `ITaskRunnerBuilder.cs`, `TaskRunnerBuilder.cs`, `TaskRunnerImpl.cs`,
`TaskRunnerDelegate.cs`, `ServiceCollectionExtensions.cs`, `UI/ITaskRunnerWindow.cs`,
`UI/TaskRunnerWindow.xaml(.cs)`, `UI/TaskRunnerWindowViewModel.cs`, plus `UI/IWindow.cs`,
`UI/WindowFactory.cs`, `UI/ServiceCollectionExtensions.cs` (windows are registered **Transient**) and
`Mvvm/MvvmHostFeature.cs` (view models are registered **Transient**). So each builder gets its own window
and its own view model. The view model is single-use.

The review findings hold, with one correction to finding 1:

1. **RunAsync.** `TaskRunnerWindow.Show` calls `Dispatcher.Invoke(ShowDialog)`. On the UI thread that runs
   inline: `ShowDialog` starts a nested message loop and **blocks the caller synchronously** until the
   window closes. Then `RunAsync` returns `Task.CompletedTask`. Called from a background thread, it blocks
   that thread until the window closes. So `RunAsync` does return after the close. The real defects are:
   (a) it is synchronous work disguised as async (the caller's thread is held, nothing is awaited), and
   (b) **the outcome is lost**: the delegate's exception is swallowed in the view model, and success,
   failure and cancellation all look the same to the caller.
2. **Cancellation logged as an error.** `ExecuteAsync` has one `catch (Exception)` that writes
   `LoggerRichTextBoxEntry.Error(ex.Message)` for everything, user cancellation included.
3. **`CancelAsync` lazily creates a CTS.** `GetCancellationTokenSource()` is `_cts ??= new()`, so a cancel
   before Execute, or after `CleanUp`, creates and cancels an orphan CTS that is never disposed.
4. **Window creation is not thread-safe.** The `Lazy` factory runs `_windowFactory.Create<ITaskRunnerWindow>()`
   (the WPF `Window` constructor needs an STA thread with a dispatcher) and reads
   `WindowsApplication.Current.MainWindow` (the `Application.MainWindow` getter calls `VerifyAccess`) on
   whatever thread first touches `Window`. Any builder call from a background thread throws
   `InvalidOperationException` today.
5. **Close feedback.** `ClosingHandler` cancels every close unless `_canClose` was set by the Close button.
   Two consequences: (a) closing from the title bar or with Alt+F4 while running silently does nothing, and
   (b) **title-bar close or Alt+F4 also does nothing after the task has finished**. Only the Close button
   works. (b) is a real usability bug that the review did not list.

There are also two latent races that this design closes as a side effect (see section 3.5):
- The Close button is bound to `Idle`, and `Idle` starts out `true`. `ExecuteAsync` waits 200 ms
  (`ToggleRunningAsync`) before it flips the state, so for those 200 ms the Close button is enabled before
  the delegate starts. Clicking it runs `CleanUp`, which nulls `_delegate`. `ExecuteAsync` then calls
  `_delegate(...)` on null: the `NullReferenceException` is caught and logged as an Error entry on a window
  that is already closed.
- `CleanUp` while `Running` (in practice only a forced close) cancels the CTS. The delegate's continuation
  then still runs `_unsubscribe`, but `CleanUp` has already set it to null, so the subscriptions are never
  removed. This is a minor leak of registrations on the publisher.

## 2. Approach (summary)

| # | Fix | Where |
|---|-----|-------|
| 1 | The view model owns a one-shot `TaskCompletionSource<TaskRunnerOutcome>` and exposes it as `Completion`. `RunAsync` shows the dialog through `Dispatcher.InvokeAsync` (truly async), then awaits `Completion`. That task carries the outcome, or rethrows the delegate's exception. | ViewModel + Builder |
| 2 | Split the catch. An `OperationCanceledException` caused by *our* token becomes a Warning "cancelled" log entry and outcome `Cancelled`. Every other exception stays an Error entry and faults `Completion`. | ViewModel |
| 3 | Create the CTS explicitly in `ExecuteAsync`. `CancelAsync` is an explicit no-op when no run is active. Remove `GetCancellationTokenSource`. | ViewModel |
| 4 | Marshal the `Lazy` window creation (factory call plus `SetOwner(MainWindow)`) through `WindowsApplication.Current.Dispatcher.Invoke`. Document that the builder may be used from any thread. | Builder + XML docs |
| 5 | Allow closing by any route once `Completion.IsCompleted`, and drop `_canClose`. While running, block the close and add one Warning log entry that explains why. | Window code-behind + ViewModel |

No ADR (section 9).

## 3. Design detail

### 3.1 Outcome type (new public enum)

```csharp
namespace Nameless.Windows.TaskRunner;

public enum TaskRunnerOutcome {
    Completed,   // delegate ran to completion (or there was no delegate to run)
    Cancelled    // user pressed Cancel, or the window was torn down before/while running
}
```

Failure is **not** an enum value. A failure is the faulted task: `await RunAsync()` rethrows the delegate's
original exception, with its stack trace kept by `TaskCompletionSource.SetException` and `await`. That
satisfies "propagate the delegate's exception except user cancellation" while cancellation stays
distinguishable from success.

Alternative considered: keep `Task RunAsync()` and complete it as *Canceled* on user cancel, so the caller
catches `OperationCanceledException`. This is idiomatic .NET and needs no API change. I rejected it because
the requirement is that user cancellation is a normal, non-throwing outcome. If the user prefers zero
public-API change, this alternative is the fallback: the same view-model wiring with `TaskCompletionSource`
(non-generic) and `TrySetCanceled()`.

Also rejected: `Task<bool>`, because `false` has no clear meaning, and a result object with
status/exception/log, which is overengineering here.

### 3.2 ViewModel: `TaskRunnerWindowViewModel`

New members:
- `private readonly TaskCompletionSource<TaskRunnerOutcome> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);`
  `RunContinuationsAsynchronously` is required. Without it, `RunAsync`'s continuation could run inline
  inside `ExecuteAsync`/`CleanUp` on the UI thread, in the middle of a close.
- `public Task<TaskRunnerOutcome> Completion => _completion.Task;`, with XML doc: "completes once per view
  model. Completed/Cancelled, or faulted with the delegate's exception."

`ExecuteAsync` rules (behavior, not code):
1. If `Completion.IsCompleted` (torn down already) → return.
   Take a local copy of the delegate. If it is null → `TrySetResult(Completed)` and return. This keeps
   today's "no delegate = nothing happens" behavior, but now it also completes the signal so the window can
   be closed.
2. `await ToggleRunningAsync()` (unchanged). Then **check again** `Completion.IsCompleted`. If the window
   was torn down during the 200 ms delay, return without running the delegate.
3. `var cts = _cts = new CancellationTokenSource();` and subscribe (unchanged).
4. `try { await @delegate(cts.Token); outcome = Completed; }`
   - `catch (OperationCanceledException) when (cts.IsCancellationRequested)` → add
     `LoggerRichTextBoxEntry.Warning(T["TaskRunnerWindow_Cancelled_Message"])` and set outcome = `Cancelled`.
     The filter must use the **local** `cts`, because `CleanUp` can null the field. An OCE that is *not*
     caused by our token (for example an `HttpClient` timeout) is a failure and falls through to the next
     catch.
   - `catch (Exception ex)` → add `LoggerRichTextBoxEntry.Error(ex.Message)` (unchanged) and keep `ex` as
     the failure. Do **not** rethrow inside the command: `AsyncRelayCommand` would surface it as an
     unhandled dispatcher exception.
   - `finally` → unsubscribe. Take a local copy of `_unsubscribe` at step 3, so that a `CleanUp` that runs
     in the middle still lets the run unregister.
5. `await ToggleRunningAsync()` (back to idle). Then complete with `TrySetResult(outcome)`, or
   `TrySetException(ex)` on failure. Use the `Try*` variants everywhere, because `CleanUp` may already have
   completed the signal.

`CancelAsync`:
- `return _cts is { IsCancellationRequested: false } cts ? cts.CancelAsync() : Task.CompletedTask;`. This
  is an explicit no-op when no run is active or the run was already cancelled.
- Delete `GetCancellationTokenSource()`.

`CleanUp` (called from `ClosingHandler` when the close is allowed):
- Keep the existing cancel/dispose/null behavior.
- Add `_completion.TrySetResult(TaskRunnerOutcome.Cancelled)` as a safety net. It ensures `RunAsync` can
  never hang if the window is closed before or while the delegate runs (for example a forced close we don't
  control). After a normal run, the signal is already set and this call does nothing.
- Known and accepted: if `CleanUp` runs while the delegate is still running, `RunAsync` returns `Cancelled`
  while the delegate may still be finishing in the background after receiving the cancellation. That is
  the existing behavior and is only documented here.

New method for finding 5: `internal void NotifyCloseBlocked()` (or public, as the developer prefers;
`internal` keeps the surface smaller). It adds one `Warning(T["TaskRunnerWindow_CloseBlocked_Message"])`
entry. To avoid spamming on repeated Alt+F4 presses, add it at most once per run, tracked with a private
bool that is reset when a run starts.

### 3.3 Window: `TaskRunnerWindow.xaml.cs`

- `ClosingHandler`: if `ViewModel.Completion.IsCompleted`, call `ViewModel.CleanUp()` and allow the close.
  Otherwise set `args.Cancel = true` and call `ViewModel.NotifyCloseBlocked()`.
- Delete `_canClose`. `CloseButtonHandler` just calls `Close()`. The Close button stays bound to `Idle`
  (no XAML change). During the first 200 ms the button is enabled, but the close is blocked with the
  warning. That is harmless, and it stops the delegate from running behind a closed window.
- `Show(...)`: unchanged (`Dispatcher.Invoke(ShowDialog)`). When it is reached through
  `Dispatcher.InvokeAsync` (section 3.4), it is already on the UI thread and runs inline.
- XAML: no change.

### 3.4 Builder: `TaskRunnerBuilder`

`RunAsync`:
```csharp
public async Task<TaskRunnerOutcome> RunAsync() {
    await WindowsApplication.Current.Dispatcher.InvokeAsync(
        () => Window.Show(WindowStartupLocation.CenterScreen));

    return await Window.ViewModel.Completion;
}
```
- `Dispatcher.InvokeAsync` queues `ShowDialog` and returns an awaitable `DispatcherOperation`. Called from
  the UI thread, `RunAsync` now returns an incomplete task right away, and the caller's `await` resumes on
  the UI thread after the window closes. Called from a background thread, it no longer blocks that thread.
- By the time the dialog closes, `Completion` is always complete: a close is only allowed once it is, and
  `CleanUp` sets it otherwise. The second await therefore only unwraps the result or rethrows the exception.
- Single use: the window is transient and `ShowDialog` cannot be called twice on a closed window. A second
  `RunAsync` on the same builder throws WPF's `InvalidOperationException`. Document this in XML docs
  ("a builder runs once; create a new one via `ITaskRunner.CreateBuilder()`"). No guard code.
- `ConfigureAwait`: leave the default. The first await must resume on the UI thread when called from the UI.

Window creation (finding 4):
```csharp
private ITaskRunnerWindow CreateTaskRunnerWindow()
    => WindowsApplication.Current.Dispatcher.Invoke(() => {
           var window = _windowFactory.Create<ITaskRunnerWindow>();
           window.SetOwner(WindowsApplication.Current.MainWindow);
           return window;
       });
```
- `Dispatcher.Invoke` runs inline on the UI thread, so behavior there does not change. From other threads
  it marshals. `Lazy<T>`'s default `ExecutionAndPublication` mode already guarantees a single creation.
- After creation, `SetName`/`SetDelegate`/`SubscribeFor` only set view-model fields or a scalar observable
  property, which is safe from a background thread for a builder used by one thread at a time.
- Contract (XML docs on `ITaskRunnerBuilder`): "May be called from any thread. A builder instance is not
  meant for concurrent use from several threads. Calls that create the window marshal to the application
  dispatcher. Do not block the UI thread waiting on a builder call made from another thread (deadlock)."
- Alternative considered: enforce UI-thread-only with `Dispatcher.VerifyAccess()` and a clear exception.
  It is just as small, but it breaks background callers instead of supporting them, and it is inconsistent
  with the existing `SetOwner`/`Show`, which already marshal. Rejected.

### 3.5 Public contract impact

| Type | Change | Compatibility |
|------|--------|---------------|
| `ITaskRunnerBuilder.RunAsync` | `Task` → `Task<TaskRunnerOutcome>`; XML docs: outcome, exception propagation, single use, threading | **Breaking** at the binary level (signature change) and for anyone implementing the interface. Source-compatible for `await builder.RunAsync();` callers. The commit must use `!` (e.g. `fix(windows)!: ...`). Package version is 1.0.0. If it has not been published yet, the user may decide that a breaking marker is not needed. |
| `TaskRunnerBuilder.RunAsync` | same | same |
| `TaskRunnerOutcome` | new enum | additive |
| `TaskRunnerWindowViewModel.Completion` | new public property | additive |
| `TaskRunnerWindowViewModel.NotifyCloseBlocked` | new, `internal` recommended | none |
| `ITaskRunnerWindow`, `IWindow`, `IWindowFactory`, `ITaskRunner` | **no change** (XML-doc fixes only: the `SetDelegate` summary "Sets the" is truncated) | none |
| Translator keys | new: `TaskRunnerWindow_Cancelled_Message`, `TaskRunnerWindow_CloseBlocked_Message` | Consuming apps must supply them. `ITranslator` falls back to the key text, the same as the existing `TaskRunnerWindow_*_Caption` keys (I found no resource file in the library that defines them). List all four keys in the `Nameless.Windows` README. |

No in-repo consumers of `TaskRunner` exist (searched `libs/` and `samples/`), so there is nothing to migrate
inside the repo.

## 4. Quality attributes

- **Correctness / consistency of the outcome signal.** There is exactly one completion per run. It is set
  with `Try*` from two places (`ExecuteAsync` and the `CleanUp` safety net). Every state change happens on
  the UI thread, so there are no races beyond the ordering described above.
- **Responsiveness (UI thread).** `RunAsync` no longer holds the caller in a sync-over-dialog call. The
  delegate's own threading is its business, as before.
- **Operability (failure visibility).** Failures now reach the caller as exceptions, in addition to the UI
  Error entry. Cancellation is a Warning, not an Error.
- **Evolvability.** No new layers. The public delta is one enum and one return type.

Fitness functions (cheap, suggested):
- Unit tests on `TaskRunnerWindowViewModel`. It has no WPF window dependency, and `IEventPublisher`/
  `ITranslator` can be mocked with Moq, which is already in `Directory.Packages.props`. They pin the
  outcome mapping: success → `Completed`, own-token OCE → `Cancelled` plus a Warning entry, foreign OCE →
  faulted plus an Error entry, other exception → faulted with the *same* exception instance.
- One STA `[IntegrationTest]` for the builder (section 6) that pins "RunAsync from a background thread does
  not throw".

## 5. Security notes

No new entry points, no I/O, no persistence. The only data crossing a boundary is `ex.Message` written to
the on-screen log, which is existing behavior. Delegates that throw exceptions with sensitive text will show
it to the user. Document this in the XML doc of `SetDelegate`; no design change.

## 6. Work split and tasks

Work split: **BACKEND-ONLY**. It is a single .NET WPF library, with no API between separate frontend and
backend apps.

Build order (one developer, `backend-developer-dotnet`):
1. Add the `TaskRunnerOutcome` enum (F1, F2).
2. View model: `_completion`/`Completion`, rewrite `ExecuteAsync` per 3.2 (local delegate/CTS/unsubscribe,
   re-check after the delay, split catch, `finally`), explicit-no-op `CancelAsync`, remove
   `GetCancellationTokenSource`, add the `CleanUp` safety net, add `NotifyCloseBlocked` (F1, F2, F3, F5).
3. Window code-behind: `ClosingHandler` based on `Completion.IsCompleted`, remove `_canClose` (F1, F5).
4. Builder: `RunAsync` via `Dispatcher.InvokeAsync` + `await Completion`, dispatcher-marshalled `Lazy`
   factory (F1, F4).
5. Interface `ITaskRunnerBuilder.RunAsync` return type and XML docs (threading, single use, outcome,
   exceptions). Fix the truncated `ITaskRunnerWindow.SetDelegate` summary (F1, F4).
6. README: the TaskRunner section with the four translator keys and a short usage snippet showing the
   outcome and exception handling (F2, F5).
7. Tests (section 7). Remove the placeholder `Class1.cs` only if the developer replaces it; otherwise leave it.

UX spec needed: **NO**. The only visible changes are two log lines and the title-bar close behavior, which
follow the existing log-entry pattern.

## 7. Test strategy

Test project: `libs/windows/tests/Nameless.Windows.Tests` (xunit.v3, currently a placeholder only). Add a
`Moq` package reference without a version (central package management). Tests must not enable `#nullable`.

| Finding | Test | Type |
|---|---|---|
| F1/F2 | `ExecuteCommand` with a completing delegate → `Completion` = `Completed`, no Error entry | `[UnitTest]` |
| F1 | delegate throws `InvalidOperationException` → `Completion` faults with the **same instance**; one Error entry | `[UnitTest]` |
| F2 | delegate observes the token; `CancelCommand` executed during the run → `Completion` = `Cancelled`; exactly one Warning entry, zero Error entries | `[UnitTest]` |
| F2 | delegate throws `TaskCanceledException` not tied to our token → faulted, Error entry (not treated as user cancel) | `[UnitTest]` |
| F3 | `CancelCommand` before Execute and after `CleanUp` → no throw, `Completion` unaffected; a later Execute still runs (no pre-cancelled token) | `[UnitTest]` |
| F1 | `CleanUp` before Execute → `Completion` = `Cancelled`; Execute afterwards does not invoke the delegate | `[UnitTest]` |
| F1 | no delegate set → `Completion` = `Completed` | `[UnitTest]` |
| F4 | on an STA thread with an `Application` and dispatcher: create the builder and call `SetName`/`SetDelegate` from a thread-pool thread → no exception | `[IntegrationTest]` (optional; WPF test hosting is fiddly, so the developer decides whether it is worth it) |
| F1 end-to-end, F5 | `await RunAsync()` returns after close; X/Alt+F4 blocked while running with a warning, allowed after completion | manual check in a WPF sample app (no WPF sample exists in `samples/`; only `web/`) |

Note: `ToggleRunningAsync` waits 200 ms twice, so each view-model unit test costs about 400 ms. That is
acceptable; don't add a time abstraction just for tests.

Risky failure modes for QA:
- `RunAsync` hanging forever: Completion never set on some close path. The `CleanUp` safety net covers it;
  test the "close before execution starts" path.
- An OCE from the delegate's *own* timeout being reported as a user cancel. Pinned by the filter test.
- The delegate running after the window closed (the 200 ms race). Pinned by the CleanUp-before-Execute
  test.
- Repeated Alt+F4 while running flooding the log. Expect at most one warning per run.
- Background-thread `RunAsync`: the continuation resumes on the thread pool, which is fine. The caller must
  not touch UI objects without marshalling (existing WPF rule, documented).

## 8. Risks and out of scope

- `Entries` (`ObservableCollection`) is changed from whatever thread publishes the notification.
  `LoggerRichTextBox` marshals its own handler, but the collection itself is not thread-safe. This exists
  today and the user asked not to touch pub/sub, so it is out of scope. Worth a separate look if delegates
  publish from thread-pool threads at high rates.
- I did not verify how WPF treats `Closing` cancellation during `Application.Shutdown`. The `CleanUp`
  safety net makes `RunAsync` safe either way, as long as `Closing` runs at all.

## 9. ADR

No ADR needed. This is a bug fix inside one component. It is not a framework or structure change, not an
external API integration, and not a database change.

## 10. Open decisions (for the user, non-blocking)

- **Breaking `RunAsync` signature.** Recommended: `Task<TaskRunnerOutcome>` with a `!` commit. Fallback
  with zero API change: keep `Task` and represent cancellation as a canceled task (caller catches OCE).
  The developer should go with the recommendation unless the user objects.
