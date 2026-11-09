# JSON-backed ITranslator, flat per-culture dictionaries, ITranslator kept over IStringLocalizer

* **Status:** Accepted
* **Group:** Architecture
* **Deciders:** Marco Teixeira (delegated the design to the JSON String Localizer epic brainstorm)
* **Date:** 2026-11-09

## Context and Problem Statement

`Nameless.Windows` localizes WPF-bound text through `ITranslator`, currently implemented by
`ResourceTranslator` on top of a generated `.resx`/`ResourceManager` pair (#188's epic, #187, replaces it).
Editing `.resx` requires Visual Studio's designer or hand-editing XML with codegen side effects, and every
new culture needs a new `.resx` + a rebuild to ship. The epic wants translations a non-developer can edit
and a translator app can hot-reload (#191) without a rebuild. This ADR is the foundation the rest of the
epic (#189 content migration, #190 DI swap, #191 hot-reload) builds on, so it needs to land before any of
those start.

**Assumptions:**

* `Nameless.WinApp` is a desktop sample app with a handful of cultures and a few hundred keys at most — this
  is not a multi-tenant, thousands-of-keys localization platform.
* `ITranslator`'s public contract (indexer, `CurrentCulture`, `SetCulture`, `INotifyPropertyChanged`) is
  fixed for this story; no consumer of the 9 existing call sites may observe a behavior change.

## Decision Drivers

* Zero behavioral drift for the 9 existing `ITranslator` consumers (Value statement of #188): same missing-
  key format (`"[key]"`), same `string.Format`-style parameter substitution, same uncaught `FormatException`
  on a mismatch.
* Translations must be editable by hand, by a non-developer, without Visual Studio or codegen.
* The storage format must support per-culture files so hot-reload (#191) can watch and reload one file
  without touching the others, and so a translator can ship a new culture by dropping in one new file.
* Keep the footprint proportional to a sample app's localization needs — no new package, no new DI
  abstraction layer beyond what hot-reload will need.

## Considered Options

### Storage format: JSON vs. resx vs. YAML/PO

* **JSON, one file per culture** — flat `{ "Key1": "Text1" }`.
  * Pros: no codegen, no designer, trivial to hand-edit or generate from a translation tool. Deserializes
    with the already-referenced `System.Text.Json`, no new package. `FileSystemWatcher` (hot-reload, #191)
    naturally maps one file to one culture.
  * Cons: no compile-time key checking (resx's generated accessor gave this for free); a stray trailing
    comma breaks the whole file.
* **resx (status quo)** — rejected: the exact problem this epic exists to solve. Needs Visual Studio or
  hand-edited XML, a rebuild per change, and `ResourceManager`'s satellite-assembly model fights hot-reload.
* **YAML or `.po`**: rejected — no package in this repo parses either today (YAGNI, Decision Driver 4); `.po`
  also brings gettext-style plural/context semantics this contract does not need.

### Shape: flat per-culture dict vs. per-type/per-feature nesting

* **Flat dict, one file per culture** (chosen): every key for a culture lives in one file,
  `{ "Key1": "Text1" }`. Matches `ResourceTranslator`'s flat `ResourceManager.GetString(key, culture)`
  lookup exactly, so the migration (#189) is a content copy, not a reshape.
  * Cons: one growing file per culture. Acceptable at this app's key count (Driver 4); revisit only if it
    stops being true.
* **Nested per-type or per-feature** (e.g. `{ "MainWindow": { "Key1": "Text1" } }`, or one file per
  feature folder): mirrors how the keys are consumed in XAML/view-models.
  * Rejected: `ITranslator`'s indexer takes one flat string key — nesting would need a key-path convention
    (`"MainWindow.Key1"`) invented for this ticket alone, a bigger change than the epic asked for, and it
    would make the #189 migration a reshape of every existing key instead of a copy.

### Contract: keep `ITranslator` vs. adopt `IStringLocalizer`

* **Keep `ITranslator`** (chosen): `Microsoft.Extensions.Localization`'s `IStringLocalizer` is designed for
  the ASP.NET Core request pipeline (`IStringLocalizer<T>`, `RequestLocalizationMiddleware`,
  `CurrentCulture` driven by the HTTP request). `Nameless.Windows` is WPF: culture is a user action
  (`SetCulture`) the UI thread drives, and bindings need `INotifyPropertyChanged`'s `"Item[]"` signal to
  refresh — `IStringLocalizer` has no such notification.
  * Pros: zero drift for the 9 consumers (Driver 1); no new package; keeps the WPF-specific culture-switch
    notification `IStringLocalizer` was never built for.
  * Cons: one more localization abstraction in the repo (`Nameless.Web` uses ASP.NET Core's own elsewhere) —
    accepted, because the two run in different hosting models with different lifecycle needs.
* **`IStringLocalizer`**: rejected — adopting it would mean reworking all 9 call sites and the XAML markup
  extension (`TranslatorExtension`) around a contract with no live-rebind notification, for a package this
  app does not otherwise depend on. Out of proportion to the epic's goal (Driver 4).

## Decision Outcome

**Chosen:** JSON, one flat-dictionary file per culture, read through a narrow `ITranslationFileProvider` seam
(`Nameless.Windows.Localization.Json`) so `JsonTranslator`'s lookup/fallback/cache logic never has to change
when hot-reload (#191) swaps in a `FileSystemWatcher`-backed provider. `ITranslator`'s contract is unchanged.

### Consequences

* **Good:** `ResourceTranslator` → `JsonTranslator` is a drop-in replacement once DI is repointed (#190) —
  same missing-key format, same format-argument behavior, same culture-switch notification.
* **Good:** the fallback chain (specific → neutral → `default.json`) is resolved per key, not per file —
  `JsonTranslator` merges every file in the chain (least specific first) into one dictionary, so a key
  missing from, say, `pt-BR.json` still resolves from `pt.json` or `default.json` instead of rendering
  `"[key]"`. This matches `ResourceManager`'s own specific-neutral-invariant resolution exactly, so culture
  behavior does not change for existing translations once migrated (#189).
* **Good:** `ITranslationFileProvider` is a one-method seam a test fake implements directly — this story's
  tests need no real disk I/O, and hot-reload (#191) only has to write one new implementation.
* **Bad (accepted):** no compile-time key checking — a typo'd key silently renders as `"[key]"` at runtime,
  same risk `ResourceManager.GetString` already had for a key missing from a satellite resource.
* **Good:** a malformed JSON file (trailing comma, wrong shape) does not crash the lookup — `JsonTranslator`
  logs the failure at `Error` with the offending culture/file key and continues down the fallback chain as
  if that file were absent.
* **Constraint:** `TranslatorOptions.RootPath` is added as the configuration seam for where culture files
  live, bound the same way `ResourceFullName` already is. It is not consumed by any shipped file provider
  yet — the provider that reads real files from disk, and the DI registration that wires it in, land with
  hot-reload (#191) and the DI swap (#190) respectively.
* **Follow-up:** #189 migrates the sample app's existing `.resx` content into the new JSON files. #190 swaps
  the DI registration from `ResourceTranslator` to `JsonTranslator` and retires `ResourceTranslator`. #191
  adds the `FileSystemWatcher`-backed `ITranslationFileProvider` and wires `TranslatorOptions.RootPath` into it.

## Related

* **Epic:** #187 JSON String Localizer.
* **Story:** #188 JSON-backed `ITranslator` core (this ADR).
* **Follow-ups:** #189 (content migration), #190 (DI re-registration), #191 (hot-reload).
* **Code:** `libs/windows/src/Nameless.Windows/Localization/JsonTranslator.cs`,
  `libs/windows/src/Nameless.Windows/Localization/Json/ITranslationFileProvider.cs`,
  `libs/windows/src/Nameless.Windows/Localization/TranslatorOptions.cs`.

## Update — 2026-11-09 (#190)

`TranslatorOptions.RootPath` is no longer unconsumed: #190 wired it into a disk-backed
`FileSystemTranslationFileProvider` (read-once-at-startup; #191's hot-reload is still the
`FileSystemWatcher`-backed implementation of the same seam), registered by
`TranslatorHostFeature` only when a `Translator` configuration section is present —
otherwise the host falls back to `NullTranslator.Instance`, matching this ADR's zero-drift
intent for hosts that never opt into localization. `ResourceTranslator` and
`TranslatorOptions.ResourceFullName` are retired as part of the same ticket (breaking
change — see CHANGELOG).

## Amended by #191

Three claims made above about the initial design did not survive contact with hot-reload
and are corrected here rather than silently edited away:

* **"`JsonTranslator`'s lookup/cache logic never has to change"** — it did. The single
  merged `Dictionary<string, string>` this ADR originally described could not support
  hot-reload's "last known-good state per file" promise: a bad reload of one culture's
  file, once merged, cannot be told apart from a good one already in the same dictionary,
  so there would be nothing to roll back to. `JsonTranslator` now caches each file's parsed
  entries separately (`_fileCache`, keyed by culture/`"default"`) and only re-merges the
  fallback chain into `_translations` lazily, invalidating that merged view on a successful
  reload or a culture switch. This is the mechanism that lets a malformed edit to one file
  leave every other cached file, and that file's own last-good content, untouched.
* **"a one-method seam"** — `ITranslationFileProvider` gained a second member,
  `Changed`, because reading a file on demand (the original one method) has no way to tell
  `JsonTranslator` *which* culture's file changed on disk without the provider pushing that
  information itself; there is no polling loop to notice it otherwise.
* **"hot-reload... only has to write one new implementation"** — in practice, #191 also
  had to modify the *existing* `FileSystemTranslationFileProvider` (adding the
  `FileSystemWatcher` subscription, debounce timers and the `Changed` raise) rather than
  add a second implementation alongside it, because the read-once provider from #190 and
  the watching one from #191 are the same disk-backed provider at two different points in
  its life, not two alternative strategies a consumer would choose between.
  `FileSystemTranslationFileProvider` is also no longer "read-once-at-startup" — that line
  above is superseded by this section.

None of this was a mistake in the original ADR: #191's hot-reload requirement did not exist
when #188 made these calls, and the fallback-merge and one-method-seam designs were correct
for what #188 had to deliver. They just did not generalize to a requirement added two
tickets later, which is what this amendment records.
