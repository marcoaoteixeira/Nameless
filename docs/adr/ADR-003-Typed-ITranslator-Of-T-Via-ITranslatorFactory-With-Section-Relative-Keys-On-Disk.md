# Typed `ITranslator<T>` resolved through `ITranslatorFactory`; keys stored section-relative on disk, composed as `{Section}_{Key}` for lookup

* **Status:** Accepted
* **Group:** Architecture
* **Deciders:** Marco Teixeira (direction set in #199; design delegated to software-architect)
* **Date:** 2026-11-10

## Context and Problem Statement

`Nameless.Windows` consumers inject one shared `ITranslator` and pass the full flat key, for example
`translator["AppConfigurationPageViewModel_PerformApplicationBackupAsync_Failure"]`. The sample app builds
those keys in two fragile ways:

* by walking the call stack (`CallerInfo.GetKey` through the `ITranslator.Get(...)` extension in
  `Nameless.WinApp/Infrastructure`). This costs 10 to 500 µs per call, breaks if the JIT inlines the
  method, and leaks names like `get_DisplayName` into keys.
* by string concatenation with `nameof()` (`Prefix => $"{nameof(Handler)}_{nameof(HandleAsync)}"`).

#197 grouped the JSON files by section (`{ "Section": [ { "Key": "Text" } ] }`). It kept the full key
inside each section because those call sites still built the full key. So every key on disk still repeats
its section name: `"AppConfigurationPage": [ { "AppConfigurationPage_Title": ... } ]`.

#199 asks for the `ILogger<T>` / `IStringLocalizer<T>` pattern: inject `ITranslator<T>`, let `T` choose the
section, and index with the short key only. Two decisions follow from that:

1. How `ITranslator<T>` is resolved, and how `T` maps to a section.
2. Whether keys on disk become section-relative, or keep the full key that #197 kept.

Constraints:

* `TranslatorExtension` (XAML), `AutoDiscoverableNavigationViewItemProvider`, `Bootstrapper` and
  `TaskRunnerWindowViewModel` index the root `ITranslator` with full keys. They must keep working unchanged.
* The JSON hot-reload, culture-fallback and missing-key behavior from ADR-002, #191 and #197 must not regress.

## Decision Drivers

1. Consumers write only the short key. No stack walking and no hand-built prefixes.
2. `ITranslator`'s public contract, and every caller that passes a full key (XAML first of all), stay as they
   are.
3. A section name is written in exactly one place on disk, which is the goal #197 set but did not reach.
4. Use a resolution model .NET developers already know. Add no new package and no new configuration.
5. Live culture switching (`INotifyPropertyChanged`, `"Item[]"`) works through every translator instance.

## Considered Options

### A. Resolution pattern

* **A1. `ITranslator<out T> : ITranslator` (marker), `Translator<T>` built from `ITranslatorFactory`,
  open-generic singleton registration** (chosen). This is the `ILogger<T>` mechanism:
  `Logger<T>(ILoggerFactory)` registered as `Singleton(typeof(ILogger<>), typeof(Logger<>))`, plus a
  `CreateLogger<T>()` extension for code outside DI. The meaning is the `IStringLocalizer<T>` one: `T`
  scopes lookups and the indexer is unchanged.
  * Pros: familiar, and needs no reflection at the call site. The factory is the single seam for how
    sections are mapped, so a custom factory can change the mapping without touching `Translator<T>`. The
    root `ITranslator` stays the singleton that owns culture state, so `SetCulture` on any typed instance
    switches the culture for the whole app.
  * Cons: adds two layers of thin wrappers (`Translator<T>` over a cached section translator over the root),
    and each wrapper must re-raise `PropertyChanged` itself (see Consequences).
* **A2. `Translator<T>(ITranslator root)` with no factory.**
  * Pros: one less type.
  * Cons: code outside DI (App code-behind, static helpers) has no anchor-based way to get a translator, and
    a different section mapping would mean replacing `Translator<T>` itself. Rejected: the factory is what
    the user asked for and what `ILogger` and `IStringLocalizer` both have.
* **A3. Adopt `IStringLocalizer<T>`.** Rejected for the same reasons as in ADR-002: it has no
  `INotifyPropertyChanged`, and it would add `Microsoft.Extensions.Localization`. We copy its shape, not
  its package.
* **A4. Keep `ITranslator` and add a scope-aware method such as `translator.For<T>()`.** Rejected: the
  consumer still injects the untyped root, so the dependency is invisible in the constructor. That is the
  problem `ILogger<T>` solves.

### B. How `T` maps to a section

* **B1. Convention: `typeof(T).Name`, with generic arity stripped (`Foo`1` becomes `Foo`). For a nested type,
  its own `Name`** (chosen). This matches the section names #197 already wrote to disk, which are simple
  type names (`AppConfigurationPageViewModel`, `PerformApplicationBackupRequestHandler`, ...). It is the
  same rule `CallerInfo.StripArity` uses today.
  * Cons: two types with the same simple name in different namespaces share one section. This is accepted:
    a section is a lookup namespace, and the duplicate-key check at parse time catches any real collision.
* **B2. Full name (`Namespace.Type`), as `ILogger` categories use.** Rejected: section names on disk would
  become 40 to 60 characters long and would change whenever a type moves namespace. That undoes #197.
* **B3. A `[TranslationSection("...")]` attribute override.** Deferred. No current consumer needs a section
  that differs from its type name. `ITranslatorFactory.Create(string section)` covers the rare explicit
  case. The attribute can be added later without breaking anything, honored inside `TranslationKey.GetSection`.

### C. Key shape on disk and at lookup (the question #197 left open)

* **C1. Keys on disk are section-relative. `JsonTranslator` flattens each entry to
  `{Section}_{Key}`, and `ITranslator<T>` composes the same string** (chosen). On disk,
  `"AppConfigurationPage": [ { "Title": "..." } ]` flattens to the lookup key `AppConfigurationPage_Title`,
  which is the exact key used today. The separator `_` was chosen so that **every composed key equals
  today's full key**. All 115 keys in both culture files already start with `{their section}_`, which was
  checked mechanically on the #197 branch. So:
  * XAML (`{loc:Translator AppConfigurationPage_Title}`), `NavigationViewItemAttribute` resource names,
    `Bootstrapper` and `TaskRunnerWindowViewModel` keep working with **no change**. The root
    `ITranslator`'s flat-key contract is unchanged.
  * The JSON rewrite is a mechanical prefix strip, and it can be proven: the flattened key set before the
    rewrite equals the set after it, apart from the two renames listed in the plan.
  * The same short key in two sections no longer collides (`A_Title` and `B_Title`). This removes the
    "last section wins" caveat #197 had to document.
  * Cons: the JSON files are rewritten a second time (2 files × 115 keys). Composed keys are ambiguous when
    names contain `_`: section `A_B` with key `C` and section `A` with key `B_C` both compose to `A_B_C`.
    The parser rejects such a collision loudly, using the same path as #197's duplicate-key rejection, so
    it cannot happen silently.
* **C2. Keep full keys on disk, and have `ITranslator<T>` prepend `{Section}_`.** No JSON churn, but every
  key keeps repeating its section, which #197 existed to remove. It also leaves two sources of truth for
  the section name, the section header and the key prefix, which can drift apart. Rejected (Driver 3).
* **C3. Section-relative keys with a different separator (`.` or `:`).** Rejected: every full-key caller
  (all XAML, the navigation attribute, `Bootstrapper`, `TaskRunnerWindowViewModel`) would have to be
  rewritten, against Driver 2, for no functional gain.
* **C4. Section-relative keys with no composed flat key. The root `ITranslator` gains a
  `(section, key)` member.** Rejected: `this[string section, string key]` would silently take over existing
  calls like `translator["Key", "arg"]`, which today bind to the `params object[]` overload. A named method
  would also change the root contract that XAML binds to through the `[Key]` path.

## Decision Outcome

**Chosen: A1 + B1 + C1.**

* `ITranslator<out T> : ITranslator`, a marker interface with no extra members.
* `ITranslatorFactory` with `Create(Type anchor)` and `Create(string section)`, plus a `Create<T>()`
  extension that returns `ITranslator<T>`. The default `TranslatorFactory(ITranslator root)` caches one
  section translator per section name.
* `Translator<T>(ITranslatorFactory)` is registered as an open-generic singleton by `TranslatorHostFeature`
  in **both** of its branches, the JSON one and the `NullTranslator` one.
* `TranslationKey.GetSection(Type)` and `TranslationKey.Compose(section, key)` are the only place the
  mapping rule and the `_` separator are defined.
* `JsonTranslator` flattens `{Section: [{Key: Text}]}` to `{Section}_{Key}`. The sample JSON files are
  rewritten to section-relative keys in the same commit.
* `TranslatorExtension`, `NullTranslator` and `ITranslator` are not changed. `NullTranslator` does not
  become generic: the factory wraps the single `NullTranslator.Instance` for every `T`.

### Consequences

* **Good:** call sites read `T["PerformApplicationBackupAsync_Failure"]`. `CallerInfo` and the stack-walking
  `Get(...)` extension are deleted from the sample app.
* **Good:** nothing that indexes the root `ITranslator` notices any change. A missing key still renders as
  `"[Section_Key]"`, the same text it rendered before.
* **Good:** one culture switch (`SetCulture` on any instance, typed or root) refreshes every binding,
  because all instances delegate to the one root.
* **Bad (accepted):** each wrapper must re-raise the root's `PropertyChanged` **with itself as the
  sender**. Forwarding `add`/`remove` straight to the root would make WPF's `PropertyChangedEventManager`
  drop the event, because it routes by sender. Wrappers subscribe to the inner translator only while they
  have subscribers of their own, so a `factory.Create<T>()` called in a loop does not leak handlers.
* **Bad (accepted):** a breaking change for the sample app's consumers and their tests. The library's
  public surface only gains members (`ITranslator`, `TranslatorExtension` and `TranslatorFeatureOptions`
  are unchanged in shape). One observable library behavior does change: the flattened lookup key of a
  grouped JSON file is now `{Section}_{Key}` instead of `{Key}`. Because #197 has not shipped (PR #198 is
  unreleased), this is recorded as part of the same unreleased change set and not as a separate breaking
  release.
* **Constraint:** `TranslatorFeatureOptions.WithTranslator(Type)` must reject types that implement
  `ITranslator<>`. Registering `Translator<X>` as the root would build a DI cycle
  (root → factory → root).
* **Follow-up (optional, not in #199):** move `Bootstrapper` and `TaskRunnerWindowViewModel` to
  `ITranslator<T>`. This is a library constructor change, and `TaskRunnerWindowViewModel`'s keys live under
  the `TaskRunnerWindow` section, so it would need `Create("TaskRunnerWindow")` or the attribute from B3.

## Related

* **Story:** #199. **Depends on:** #197 / PR #198 (grouped on-disk shape).
* **Builds on:** ADR-002 (JSON-backed `ITranslator`, which kept `ITranslator` over `IStringLocalizer`).
  This ADR does not supersede ADR-002: the root contract and the per-culture flat lookup stay. ADR-002 rejected
  nested keys because they "would need a key-path convention". This ADR defines that convention as
  `{Section}_{Key}`, chosen so that it equals the existing keys.
* **Plan:** `docs/plans/199-translator-generic-resolution.md`.
