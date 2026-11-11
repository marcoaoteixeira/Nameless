# #199: `ITranslator<T>` generic resolution, in the style of `ILogger<T>` and `IStringLocalizer<T>`

- Ticket: [#199](https://github.com/marcoaoteixeira/Nameless/issues/199). Depends on #197 / PR #198.
- Stack: .NET 10 / WPF. Library `Nameless.Windows` and sample app `Nameless.WinApp`.
- Agent pair: `backend-developer-dotnet` / `backend-reviewer-dotnet`. There is no separate frontend stack.
  The WPF consumers are in the same .NET solution and are built by the same developer.
- Work split: **BACKEND-ONLY, sequential.** The steps are listed in §7.
- UX spec: **not needed.** No screen, state or text a user can see changes. Every rendered string stays
  byte-identical.
- ADR: [ADR-003](../adr/ADR-003-Typed-ITranslator-Of-T-Via-ITranslatorFactory-With-Section-Relative-Keys-On-Disk.md),
  which covers the resolution pattern and the key shape.

## 0. Blocker: rebase onto #197 first

`feature/NAMELESS-199-translator-generic-resolution` was branched from `main` at `73633391`. It does **not**
contain #197's commits (`246b0c8a`, `2f1e38a4`). PR #198 is still OPEN. This design assumes #197's grouped
parser and its regrouped JSON files. So, before any code is written:

1. Merge PR #198, then rebase this branch onto `origin/main`. Alternatively, rebase onto
   `feature/NAMELESS-197-grouped-translation-keys` and rebase again once #198 merges.
2. The working tree has two uncommitted changes that are not part of this ticket:
   `AppHostFeatureCollectionExtensions.cs` (removes `WindowHostFeature` from `CreateMinimal()`) and
   `TranslatorHostFeature.cs` (a `GetSectionFor` cleanup and `TryAddSingleton`). They belong to the user, so
   do not discard them. Ask whether they belong to this ticket before the rebase. `TranslatorHostFeature.cs`
   is edited in step 1 below.

## 1. Context: what the code does today

- `ITranslator`: a flat-key indexer, `this[key]` and `this[key, params object[]]`, plus `CurrentCulture`,
  `SetCulture` and `INotifyPropertyChanged`. It is registered as one singleton (`JsonTranslator`, or
  `NullTranslator.Instance` when there is no `Translator` config section) by `TranslatorHostFeature`.
- #197's `JsonTranslator.TryParseFile` reads `{ "Section": [ { "Key": "Text" } ] }` and flattens it to
  `Key`. The section name is thrown away.
- On disk, both culture files hold 115 keys. **Every key starts with `{its section}_`**. This was checked
  mechanically on the #197 branch. The only odd short keys are two that `CallerInfo` produced from a
  property getter: `get_DisplayName_DisplayName` (in `CleanupPreviousSystemUpdateStep` and
  `EnsureApplicationDirectoriesExistenceStep`).
- Sample-app consumers build the full key in one of two ways:
  - The `ITranslator.Get(part)` extension (`Infrastructure/TranslatorExtensions.cs`), which calls
    `CallerInfo.GetKey` (`Infrastructure/CallerInfo.cs`). That walks the call stack to find
    `{Type}_{Method}_{part}`.
  - `nameof()` concatenation: `private static string Prefix => $"{nameof(X)}_{nameof(HandleAsync)}"`.
- Callers that pass a full key to the root `ITranslator`, all of which stay unchanged:
  - XAML `{loc:Translator AppConfigurationPage_*}` (`TranslatorExtension`).
  - `TaskRunnerWindow.xaml` (`TaskRunnerWindow_*_Caption`).
  - `AutoDiscoverableNavigationViewItemProvider`, which uses `NavigationViewItemAttribute.TitleResourceName`,
    for example `AppConfigurationPage_NavigationViewItem_Title`.
  - `Bootstrapper` (`Bootstrapper_*`).
  - `TaskRunnerWindowViewModel` (`TaskRunnerWindow_*_Message`).

## 2. Approach

### 2.1 Decisions (see ADR-003 for the alternatives)

| # | Question | Decision |
|---|---|---|
| 1 | How `T` maps to a section | `typeof(T).Name` with generic arity stripped (`Foo`1` becomes `Foo`). A nested type uses its own `Name`. No attribute yet; `ITranslatorFactory.Create(string)` is the escape hatch. |
| 2 | How `ITranslator<T>`, `ITranslator` and the factory relate | `ITranslator<out T> : ITranslator` (a marker). `Translator<T>(ITranslatorFactory)` is registered as an open-generic **singleton**. This is the `ILogger<T>` mechanism with `IStringLocalizer<T>` semantics. |
| 3 | Resolution outside DI (App.xaml.cs) | `provider.GetRequiredService<ITranslatorFactory>().Create<TAnchor>()`, which returns `ITranslator<TAnchor>`. `App.xaml.cs` itself needs **no change**: its only translator use is `TranslatorExtension.ResolveTranslator(provider)`, which must get the **root** translator. |
| 4 | Key shape on disk | **Shorten to section-relative keys.** `JsonTranslator` flattens to `{Section}_{Key}`. Because the separator is `_`, every composed key equals today's full key. |
| 5 | `TranslatorExtension` | No change. It keeps the root `ITranslator` and full keys, which still resolve because composed key = old key. |
| 6 | No config / `NullTranslator` | `NullTranslator` stays non-generic and unchanged. The factory wraps `NullTranslator.Instance` for every `T`. `ITranslator<T>` over it returns `"Section_Key"`, which is what consumers already see today when they pass the full key. |

### 2.2 How a lookup flows

```
ctor(ITranslator<AppConfigurationPageViewModel> t)
  t["PerformApplicationBackupAsync_Failure", msg]
    → Translator<T> → inner = factory.Create(typeof(T))       // cached SectionTranslator("AppConfigurationPageViewModel")
      → root["AppConfigurationPageViewModel_PerformApplicationBackupAsync_Failure", msg]   // TranslationKey.Compose
        → JsonTranslator merged dictionary (keys flattened as {Section}_{Key})
```

### 2.3 Alternatives considered

These are summarized here; ADR-003 has the full reasoning.

- No factory (`Translator<T>(ITranslator root)`). Rejected: there is no anchor-based path for code outside
  DI, and changing the section mapping would mean replacing the class.
- Keep full keys on disk and prepend the section in `Translator<T>`. Rejected: the redundancy #197 set out to
  remove would stay, and the section name would have two sources of truth.
- Use a `.` or `:` separator. Rejected: every XAML key and every full-key library caller would have to change.
- Add a `(section, key)` indexer on `ITranslator`. Rejected: it would silently take over existing
  `["key", "arg"]` calls that today bind to the `params` overload.

## 3. Contract (binding for the developer; report any deviation)

All of these go in namespace `Nameless.Windows.Localization`. Follow the library's XML-doc and C# 14
`extension(...)` block conventions.

```csharp
public interface ITranslator<out T> : ITranslator { }            // marker, no extra members

public interface ITranslatorFactory {
    ITranslator Create(Type anchor);                               // section = TranslationKey.GetSection(anchor)
    ITranslator Create(string section);                            // explicit section; null/whitespace → throws
}

// extension block on ITranslatorFactory:
ITranslator<T> Create<T>();                                        // returns new Translator<T>(factory)

public static class TranslationKey {
    public const string Separator = "_";
    public static string GetSection(Type anchor);                  // Name, arity stripped; null → throws
    public static string Compose(string section, string key);      // section + Separator + key
}

public sealed class TranslatorFactory : ITranslatorFactory {      // [SkipAssemblyScan] not needed (not an ITranslator)
    public TranslatorFactory(ITranslator root);                    // caches one SectionTranslator per section (ordinal)
}

[SkipAssemblyScan] public sealed class Translator<T> : ITranslator<T> {
    public Translator(ITranslatorFactory factory);                 // inner = factory.Create(typeof(T))
}

[SkipAssemblyScan] internal sealed class SectionTranslator : ITranslator {
    SectionTranslator(ITranslator root, string section);
}
```

Behavior rules for `SectionTranslator` and `Translator<T>`:

- **Indexers.** If `key` is null or whitespace, pass it to the inner translator **unchanged**, so the inner
  translator keeps deciding (`JsonTranslator` returns `""`, `NullTranslator` returns the key). Otherwise call
  `inner[TranslationKey.Compose(section, key), parameters]`. `Translator<T>` delegates as-is, because its
  inner translator is already section-scoped.
- **`CurrentCulture` and `SetCulture(...)`.** Delegate to the inner translator, which ends at the root. One
  culture switch affects every instance.
- **`PropertyChanged`.** Re-raise the inner translator's events **with `this` as the sender**. Do not forward
  `add`/`remove`: WPF's `PropertyChangedEventManager` routes by sender, so bindings would stop refreshing.
  Subscribe to the inner translator when the first handler is added, and unsubscribe when the last one is
  removed. Guard this with a lock, so `factory.Create<T>()` called repeatedly cannot leak handlers on the
  root.
- **Missing key.** Whatever the root returns. For `JsonTranslator` that is `"[Section_Key]"`, identical to
  today's output.

DI registration in `TranslatorHostFeature.Register`, added to **both** branches (the `NullTranslator` early
return and the configured branch), next to the existing root registration:

```csharp
builder.Services.TryAddSingleton<ITranslatorFactory, TranslatorFactory>();
builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ITranslator<>), typeof(Translator<>)));
```

`TranslatorFeatureOptions.WithTranslator(Type)` must also throw when `type` implements `ITranslator<>`. If it
did not, the root would be a `Translator<X>`, and DI would form the cycle root → factory → root.

`JsonTranslator.TryParseFile`, which is the only behavior change in the library:

- Flatten each entry to `TranslationKey.Compose(sectionName, entryKey)`. Do not use `entryKey` alone.
- If a composed key already exists in the flattened dictionary, which can only happen when names containing
  `_` collide across sections, log a warning naming both sections and return `null`. This takes the same
  "fall back / keep last known-good" path as #197's duplicate-key rule.
- An empty or whitespace section name or entry key is a shape mismatch, with the same handling.
- Leave the fallback chain, merge order, file cache, hot-reload and `"Item[]"` signalling alone.
- Update the `<remarks>` text: the old note that the last section wins for a repeated key is now obsolete.

## 4. Sample JSON rewrite (`default.json`, `pt-BR.json`)

This is a mechanical strip of `{Section}_` from every key, in both files, done **in the same commit as the
parser change**. If either lands alone, every lookup breaks. The only non-mechanical change is two renames:

| Section | Old full key | New short key |
|---|---|---|
| `CleanupPreviousSystemUpdateStep` | `CleanupPreviousSystemUpdateStep_get_DisplayName_DisplayName` | `DisplayName` |
| `EnsureApplicationDirectoriesExistenceStep` | `EnsureApplicationDirectoriesExistenceStep_get_DisplayName_DisplayName` | `DisplayName` |

Proof the developer should run once and include in the PR description: the flattened composed key set after
the rewrite equals the pre-rewrite key set (115 keys per culture), with only the two renames above applied.

## 5. Breaking call sites and their migration

Line numbers are on `main` / `73633391`. #197 does not touch these files.

### 5.1 The migration pattern

```csharp
// Before
private readonly ITranslator _translator;
public PerformApplicationBackupRequestHandler(..., ITranslator translator)
_translator[$"{Prefix}_{suffix}"]                       // Prefix = "PerformApplicationBackupRequestHandler_HandleAsync"
T.Get("Failure", ex.Message)                            // CallerInfo → "AppConfigurationPageViewModel_PerformApplicationBackupAsync_Failure"

// After
private readonly ITranslator<PerformApplicationBackupRequestHandler> _translator;
public PerformApplicationBackupRequestHandler(..., ITranslator<PerformApplicationBackupRequestHandler> translator)
_translator[$"HandleAsync_{suffix}"]                    // Prefix property deleted
T["PerformApplicationBackupAsync_Failure", ex.Message]  // explicit, no stack walk
```

Rule for the short key: **the old full key with `{Section}_` removed.** The `{Method}_` part that
`CallerInfo` used to add implicitly stays in the key and is now written out at the call site. That keeps the
JSON rewrite purely mechanical. Shortening keys further, for example dropping `HandleAsync_`, is out of scope.

### 5.2 Sample app production code (`samples/windows/Nameless.WinApp/`)

| File | Lines | Change |
|---|---|---|
| `ViewModels/Pages/AppConfigurationPageViewModel.cs` | 50 (prop), 82 (ctor) | Change the type to `ITranslator<AppConfigurationPageViewModel>`. |
| | 133, 142, 143, 156, 176, 182, 185, 190 | `T.Get("X", …)` becomes `T["PerformSystemUpdateAsync_X", …]`. Line 143 uses the named argument `key:`, so drop the name. |
| | 213, 220, 226 | Becomes `T["PerformApplicationBackupAsync_X", …]`. |
| | 234 (inside a lambda), 244, 255, 256, 266 | Becomes `T["PerformApplicationRestoreAsync_X", …]`. |
| | 286 `T.SetCulture` | No change (it delegates to the root). |
| `Views/Windows/MainWindow.xaml.cs` | 30, 38 | Change the type to `ITranslator<MainWindow>`. |
| | 92, 93 | `T["MainWindow_ConfirmApplicationExit_MessageBox_*"]` becomes `T["ConfirmApplicationExit_MessageBox_*"]`. |
| `Features/PerformApplicationBackup/PerformApplicationBackupRequestHandler.cs` | 31, 65 | Change the type to `ITranslator<PerformApplicationBackupRequestHandler>`. |
| | 185, 297, 303 | `{Prefix}_` becomes `HandleAsync_`. |
| | 307 | Delete `Prefix`. |
| `Features/PerformApplicationRestore/PerformApplicationRestoreRequestHandler.cs` | 43, 81 | Change the type to `ITranslator<PerformApplicationRestoreRequestHandler>`. |
| | 165, 177, 388, 451, 471, 477 | `{Prefix}_` becomes `HandleAsync_`. |
| | 481 | Delete `Prefix`. |
| `Features/PerformSystemUpdate/PerformSystemUpdateRequestHandler.cs` | 40, 54 | Change the type to `ITranslator<PerformSystemUpdateRequestHandler>`. |
| | 147, 156, 412, 494, 500 | `{Prefix}_` becomes `HandleAsync_`. |
| | 504 | Delete `Prefix`. |
| `Features/OpenCurrentLogFile/OpenCurrentLogFileRequestHandler.cs` | 17, 19 | Change the type to `ITranslator<OpenCurrentLogFileRequestHandler>`. |
| | 31, 32 | `.Get("Notify_…")` becomes `["HandleAsync_Notify_…"]`. |
| `Bootstrap/Steps/InitializeLuceneIndexStep.cs` | 12, 14 | Change the type to `ITranslator<InitializeLuceneIndexStep>`. |
| | 20, 24 | `.Get("Startup")` / `.Get("Finish")` become `["ExecuteAsync_Startup"]` / `["ExecuteAsync_Finish"]`. |
| `Bootstrap/Steps/EnsureApplicationDirectoriesExistenceStep.cs` | 19, 34 | Change the type to `ITranslator<EnsureApplicationDirectoriesExistenceStep>`. |
| | 22 | `T.Get(nameof(DisplayName))` becomes `T["DisplayName"]` (the renamed key). |
| | 42, 46, 50, 54 | Becomes `T["ExecuteAsync_Create…Directory"]`. |
| `Bootstrap/Steps/CleanupPreviousSystemUpdateStep.cs` | 40, 67 | Change the type to `ITranslator<CleanupPreviousSystemUpdateStep>`. |
| | 49 | Becomes `T["DisplayName"]` (the renamed key). |
| | 91 | Becomes `T["ExecuteAsync_CleaningUp"]`. |
| `Infrastructure/TranslatorExtensions.cs` | the whole file | **Delete** it. No `.Get(` callers remain. |
| `Infrastructure/CallerInfo.cs` | the whole file | **Delete** it. Its only user was `TranslatorExtensions`, and it has no tests. |
| `App.xaml.cs` | 67 | **No change.** `ResolveTranslator` must keep the root translator. |
| `Infrastructure/ApplicationHostFeature.cs` | 37 | **No change.** `WithTranslator<JsonTranslator>()` still registers the root. |
| `Views/Pages/AppConfigurationPage.xaml`, `.xaml.cs` | all | **No change.** The full keys still resolve. |

### 5.3 Sample app tests (`samples/windows/Nameless.WinApp.Tests/`)

- Add `Tooling/FakeTranslator.cs`: a helper `FakeTranslator.For<T>()` that returns
  `new TranslatorFactory(new FakeTranslator()).Create<T>()`. It goes through the **real** `Translator<T>`
  and `SectionTranslator`, so the echoed string is the composed key, which equals the old full key. As a
  result, existing assertions such as `"AppConfigurationPageViewModel_PerformSystemUpdateAsync_Failure"`
  (`AppConfigurationPageViewModelTests.cs:161`) **keep passing unchanged**. That makes them a free
  regression check on the migration.
- Replace `translator: new FakeTranslator()` at:
  - `CleanupPreviousSystemUpdateStepTests.cs:64,132`
  - `AppConfigurationPageViewModelTests.cs:75`
  - `PerformSystemUpdateRequestHandlerTests.cs:175,493,569`
  - `PerformApplicationRestoreRequestHandlerTests.cs:71,638`
  - `PerformApplicationBackupRequestHandlerTests.cs:71`
- Change the `ITranslator translator` helper parameters in `PerformSystemUpdateRequestHandlerTests.cs:455,525`
  to `ITranslator<PerformSystemUpdateRequestHandler>`.
- `Resources/loc/TranslationJsonKeyParityTests.cs`: flatten with `TranslationKey.Compose(section, key)`.
- `Resources/loc/FileSystemTranslationFileProviderSampleAppTests.cs`: its lookups go to the root, so they
  should still pass. Check them after the rewrite.

### 5.4 Library tests (`libs/windows/tests/Nameless.Windows.Tests/`, in the state #197 left them)

`JsonTranslatorTests`, `JsonTranslatorHotReloadTests`, `JsonTranslatorFileSystemWatcherIntegrationTests` and
`TranslatorHostFeatureTests` all have grouped fixtures that look up `translator["Key1"]`. Those lookups
become `translator["Section_Key1"]`. #197's test
`Indexer_WhenKeysAreGroupedIntoDifferentSections_BothSectionsResolveByTheirOwnUnprefixedKey` changes its
meaning: the same short key in two sections now resolves to two **distinct** composed keys. Rename it and
rewrite its assertion to match.

### 5.5 Not migrated in #199 (they keep using the root `ITranslator` with full keys, which still works)

- `libs/windows/src/Nameless.Windows/Bootstrap/Bootstrapper.cs`
- `TaskRunner/UI/TaskRunnerWindowViewModel.cs`
- `Navigation/AutoDiscoverableNavigationViewItemProvider.cs`
- `Navigation/NavigationHostFeature.cs:53`

Moving these would mean changing library constructors. It is an optional follow-up; ADR-003 has the details.

## 6. Quality attributes and fitness functions

| Attribute | How the design serves it | Fitness function (suggested; the developer writes it) |
|---|---|---|
| Compatibility | Composed key = old key, so XAML and library callers are unaffected. | Updated `TranslationJsonKeyParityTests`: the composed key sets are equal across cultures. |
| Evolvability / no redundancy | The section name is written once on disk. | New `[IntegrationTest]` `TranslationJsonFiles_KeysAreSectionRelative`: no key in either file starts with `{its section}_`. |
| Consistency of the pattern | WinApp consumers declare a typed dependency. | New `[UnitTest]` architecture check: no public constructor of a concrete type in the `Nameless.WinApp` assembly takes a plain `ITranslator` parameter. The allowlist starts empty. |
| Correctness of live culture refresh | Wrappers re-raise events with themselves as the sender. | `[UnitTest]`: after a root culture switch, a `Translator<T>` handler sees `sender == translator` and `"Item[]"`. After the last handler is removed, the root has no subscriber left from that wrapper. |
| Performance | Removes the 10 to 500 µs stack walk per lookup. Adds one string concatenation per lookup, which is negligible. | None needed. |

## 7. Tasks (`backend-developer-dotnet`), in build order

Step 0 is §0: rebase onto #197 and settle the WIP files.

1. **Library: the typed-translator core.** This is additive and changes no existing behavior.
   - Add `TranslationKey`, `ITranslator<out T>`, `ITranslatorFactory` with the `Create<T>()` extension,
     `TranslatorFactory`, `SectionTranslator` and `Translator<T>`.
   - Add the registration in both branches of `TranslatorHostFeature`.
   - Add the `WithTranslator` guard against `ITranslator<>`.
   - Unit tests: section mapping (plain type, generic type, nested type), composition, whitespace-key
     pass-through, format arguments, delegation of `SetCulture` and `CurrentCulture`, event re-raise with
     sender and lazy subscription, factory caching, resolving `ITranslator<X>` with and without the
     `Translator` config section (the second case gives `"X_Key"` through `NullTranslator`), and the guard.
   - Commit as `feat(199): add ITranslator<T> and ITranslatorFactory`.
2. **Library and sample, one atomic commit.**
   - `JsonTranslator` flattens to `{Section}_{Key}`, with the collision and empty-name rules.
   - Rewrite the two sample JSON files (§4).
   - Update the library JSON test fixtures (§5.4) and `TranslationJsonKeyParityTests`.
   - Add the section-relative fitness test.
   - Commit as `feat(199)!: store translation keys section-relative on disk`.
   - Step 2 does not depend on step 1's types except `TranslationKey`, so put `TranslationKey` in step 1.
3. **Sample migration.** Do one consumer per commit, or a single commit. Update each consumer's tests in
   the same commit.
   - Add the `FakeTranslator.For<T>()` tooling first.
   - Then migrate, in this order: the three request handlers, `OpenCurrentLogFileRequestHandler`, the three
     steps, `AppConfigurationPageViewModel`, `MainWindow`.
   - Delete `TranslatorExtensions.cs` and `CallerInfo.cs` last.
   - Add the architecture fitness test.
   - Commit as `refactor(199)!: inject ITranslator<T> in WinApp consumers`.
4. **Documentation.**
   - In `libs/windows/src/Nameless.Windows/README.md`, update the "Translator keys" section to show the
     section-relative shape, the composition rule and `ITranslator<T>` and factory usage.
   - Add a `CHANGELOG.md` entry under `[Unreleased]` with an `Added` item (`ITranslator<T>`,
     `ITranslatorFactory`) and a `Changed` item (on-disk keys are section-relative). Fold this into #197's
     unreleased entry where it applies.
   - Add a short "Amended by #199" pointer to ADR-002 linking ADR-003, following the project's existing
     amendment practice. Keep it to a pointer only.

**Parallel vs sequential.**
- Steps 1 and 2 touch disjoint files and could run in parallel worktrees, but each is small. Splitting them
  costs more in merging than it saves.
- Step 3 must come after both 1 and 2. The new types must exist, and the short keys only resolve once the
  parser composes them.
- Within step 3, each consumer is independent, but they share the tooling helper and each is about 5
  minutes of work.
- **Verdict: one developer, sequential.** Each commit must build and test green on its own. That holds for
  this order: step 1 is additive, step 2 keeps every old full key resolvable, and step 3 swaps call sites
  over to composed keys that equal the old ones.

## 8. Test strategy and risks for QA

**Per acceptance criterion:**

- Typed resolution: unit tests on the factory and wrappers, plus an integration test that builds the host
  through `TranslatorHostFeature` and resolves `ITranslator<T>`.
- Unchanged XAML / root behavior: the existing `JsonTranslator`, hot-reload and watcher tests (fixtures
  updated), plus the parity test.
- Consumers migrated: the existing WinApp handler, step and viewmodel tests. Their asserted key strings must
  not change, and that is the check.

**Risks worth probing:**

1. **Live culture switch through a typed translator.** WPF bindings whose `Source` is an `ITranslator<T>`
   must still refresh, both on `SetCulture` and on hot-reload. Bindings on the root through XAML must
   refresh when `SetCulture` is called on a *typed* instance (`AppConfigurationPageViewModel.cs:286`).
2. **Hot-reload of a file with a cross-section composed-key collision.** The file must be rejected, with
   the last known-good state kept and no `"Item[]"` signal raised.
3. **Unconfigured host** (no `Translator` section). `ITranslator<T>` must resolve and return `"Section_Key"`,
   and must not throw.
4. **Missing key.** The output must be `"[Section_Key]"`, not `"[Key]"`.
5. **The two renamed `DisplayName` keys.** The bootstrap splash must show the localized step names in both
   cultures.
6. **Repeated `factory.Create<T>()`** must not grow the root's `PropertyChanged` invocation list.

**Already broken before this ticket; do not fix here.** Report these as separate bugs:

- `MainWindow_ConfirmApplicationExit_MessageBox_Title` and `_Message` are absent from both JSON files, so
  the exit confirmation renders `[MainWindow_…]`.
- `TaskRunnerWindow_CloseBlocked_Message` and `TaskRunnerWindow_Cancelled_Message` are also absent (since
  `aad08f27`). The parity test cannot catch either case, because it only compares the two cultures with
  each other.

## 9. Security notes

There is no new entry point and no new data flow. Section names come from compile-time types or from
developer-supplied strings, never from user input. Translation templates still go through `string.Format`
with the same `FormatException` exposure as before, and the files remain local and app-owned. Nothing new is
logged beyond section and key names, which contain no PII.

## 10. Open points to confirm before implementation (none blocking the design)

1. **Shorten on-disk keys (recommended) vs keep full keys.** This plan shortens them. The cost is a second
   mechanical rewrite of the JSON files. Keeping full keys would still work with the same `ITranslator<T>`
   API, but it would leave the redundancy #197 aimed to remove.
2. **`App.xaml.cs` needs no factory call today.** The factory path `Create<TAnchor>()` is available for any
   code-behind or static context that needs a typed translator outside constructor injection. Confirm you
   do not expect `App.xaml.cs` itself to start translating something.
3. **The two `get_DisplayName_DisplayName` renames to `DisplayName`.** These are the only changes that are
   not mechanical.
4. **Library consumers stay on the root `ITranslator`** (`Bootstrapper`, `TaskRunnerWindowViewModel`,
   navigation). Moving them is an optional follow-up ticket.
