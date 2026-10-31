# Nameless.Windows

WPF building blocks for Nameless applications.

## TaskRunner

`ITaskRunner` builds a modal "task runner" window that runs a single
cancellable background delegate, shows its progress log, and reports how
the run ended.

```csharp
var outcome = await taskRunner.CreateBuilder()
    .SetName("Importing data")
    .SetDelegate(async ct => await importer.RunAsync(ct))
    .SubscribeFor<ImportProgressEvent>()
    .RunAsync();

switch (outcome) {
    case TaskRunnerOutcome.Completed:
        // the delegate ran to completion.
        break;
    case TaskRunnerOutcome.Cancelled:
        // the user cancelled, or the window was closed before/while running.
        break;
}
```

`RunAsync` returns a `Task<TaskRunnerOutcome>`. A delegate exception other
than a cancellation of its own token propagates to the caller through that
same task (`await` rethrows it); cancellation of the run's own token is
reported as `TaskRunnerOutcome.Cancelled` instead of throwing.

A builder is single-use: a second call to `RunAsync` on the same builder
throws `InvalidOperationException`. Create a new builder via
`ITaskRunner.CreateBuilder()` for another run. Builder methods may be
called from any thread (calls that touch the window marshal to the
application dispatcher internally); a builder instance itself is not
meant for concurrent use from more than one thread at a time.

### Behavior changes from the previous `RunAsync` (`Task`, no outcome)

If you are upgrading code written against the old `Task RunAsync()`
signature, note that this is more than a return-type change:

- **A delegate exception used to be swallowed.** It was logged as an Error
  entry in the window and otherwise discarded. Now `await RunAsync()`
  rethrows it. If your delegate can fail and you relied on the old
  "nothing ever throws" behavior, wrap the call in `try`/`catch`.
- **On the UI thread, `RunAsync` used to block until the window closed.**
  The old implementation called `Dispatcher.Invoke(ShowDialog)` directly,
  which pumps a nested message loop synchronously. Code after
  `builder.RunAsync();` (without awaiting) only ran once the dialog
  closed. Now the window is shown through `Dispatcher.InvokeAsync`, so
  `RunAsync` returns an incomplete task immediately: code after a
  non-awaited `builder.RunAsync();` call now runs **before** the dialog
  even shows. Await it if you need to wait for the result.
- **`RunAsync().Wait()` / `.Result` on the UI thread used to work.** It no
  longer does: calling either now deadlocks the UI thread, because the
  dialog needs that same thread's dispatcher to pump in order for the run
  (and therefore the task) to ever complete. Always `await RunAsync()`
  from the UI thread; never block on it synchronously.

### Translator keys

The task runner window and view model look up the following keys through
`ITranslator`. Supply them in your application's resources; when missing,
`ITranslator` falls back to the key text itself:

| Key | Used when |
|---|---|
| `TaskRunnerWindow_Cancelled_Message` | The run is cancelled through its own token (Warning log entry). |
| `TaskRunnerWindow_CloseBlocked_Message` | A title-bar/Alt+F4 close is blocked because the run has not completed yet (Warning log entry, at most once per run). |
