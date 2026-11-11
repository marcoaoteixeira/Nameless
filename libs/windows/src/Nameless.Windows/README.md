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

On disk, each culture's JSON file groups its keys by section:

```json
{
  "TaskRunnerWindow": [
    { "Cancelled_Message": "The run was cancelled." }
  ]
}
```

The section is not just organizational: `JsonTranslator` flattens every
entry to `{Section}_{Key}` (`TranslationKey.Compose`), and that composed
string is the flat key `ITranslator`'s indexer actually looks up. So the
entry above resolves as `translator["TaskRunnerWindow_Cancelled_Message"]`
— a key can still be renamed within its own section freely, but moving it
to a *different* section changes its lookup key, because the section name
is part of it. A missing key falls back to the bracketed composed key
itself, e.g. `[TaskRunnerWindow_Cancelled_Message]`.

The task runner window and view model look up the following keys through
the root `ITranslator` (see below for why), under the `TaskRunnerWindow`
section. Supply them in your configured translation JSON files (see
`TranslatorOptions.RootPath`).

| Key | Used when |
|---|---|
| `TaskRunnerWindow_Cancelled_Message` | The run is cancelled through its own token (Warning log entry). |
| `TaskRunnerWindow_CloseBlocked_Message` | A title-bar/Alt+F4 close is blocked because the run has not completed yet (Warning log entry, at most once per run). |

#### `ITranslator<T>` and `ITranslatorFactory`

Most consumers should not build composed keys by hand or inject the
untyped root `ITranslator`. Inject `ITranslator<T>` instead — the same
resolution pattern as `ILogger<T>` and `IStringLocalizer<T>` — and index it
with the section-relative key only:

```csharp
public sealed class MyPageViewModel(ITranslator<MyPageViewModel> translator) {
    // Resolves "MyPageViewModel_Title" — the section ("MyPageViewModel",
    // from the anchor type's own simple name) is supplied automatically.
    public string Title => translator["Title"];
}
```

`ITranslator<T>` is registered as an open-generic singleton, resolved
through `ITranslatorFactory` (`Create(Type)`, `Create(string)`, and the
`Create<T>()` extension for code outside DI, e.g. static helpers or
code-behind). `TranslationKey.GetSection(Type)` is the only place the
type-to-section mapping rule lives: a type's own `Name`, with generic
arity stripped. `ITranslatorFactory.Create(string section)` is the escape
hatch for an anchor whose section differs from its own type name.

One culture switch (`SetCulture`, on any instance — typed or root) still
refreshes every binding, because every `ITranslator<T>` ultimately
delegates to the same root. The library's own XAML markup extension
(`TranslatorExtension`) and a few library-internal consumers
(`Bootstrapper`, `TaskRunnerWindowViewModel`, the navigation attribute
provider) keep using the untyped root `ITranslator` with full,
pre-composed keys — moving them is an optional follow-up, not required to
use `ITranslator<T>` in your own code.
