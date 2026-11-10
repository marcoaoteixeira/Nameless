# Epic (DRAFT): JSON String Localizer

| Field   | Value |
|---------|-------|
| Tracker | not created yet |
| Team    | Solo maintainer (Marco Teixeira) |
| Project/Repo | GitHub `marcoaoteixeira/Nameless`, Project #4 "Nameless" |
| Owner   | Marco Teixeira |
| Status  | Brainstorming — pivoted (D28–D30), all parts closed, ready for tracker-ready doc rewrite |
| Updated | 2026-11-09 |

> This draft contains only what is confirmed by the product owner, sources, or existing tickets.
> Anything not confirmed is listed under **Open Questions** rather than assumed.
> Work proceeds part by part.

---

## 1. Summary

**Pivoted (D28).** PO review surfaced a real contradiction: `ITranslator` is injected directly in 9 places in the sample WinApp, and exposes `CurrentCulture`/`SetCulture`/`INotifyPropertyChanged` with no `IStringLocalizer` equivalent. Decision: drop `IStringLocalizer` entirely. Keep `ITranslator` as the public contract, unchanged. Swap its backing implementation from `.resx`/`ResourceManager` (`ResourceTranslator`) to JSON files, keeping all the hot-reload/fallback/caching behavior already decided below.

## 2. Decisions (confirmed by product owner)

| # | Decision |
|---|----------|
| D1 | **Epic name:** "JSON String Localizer". |
| ~~D2~~ | ~~Lives under `libs/common`.~~ **Superseded by D29:** `ITranslator` is not moving — the JSON-backed implementation lives alongside it in `libs/windows/src/Nameless.Windows/Localization/`. |
| D3 | **Team:** solo maintainer — no cross-team coordination needed. |
| ~~D4~~ | ~~Consumer scope: library-agnostic `IStringLocalizer` core.~~ **Superseded by D28:** contract stays `ITranslator`, unchanged; only the backing store is swapped to JSON. (Q1.1) |
| ~~D5~~ | ~~Replaces `ITranslator`/resx entirely with `IStringLocalizer`.~~ **Superseded by D28:** `ITranslator` is kept as the interface; only its implementation changes (resx → JSON). (Q1.2) |
| D6 | **Driving pain point:** `.resx` is hard to edit; want hot-reload without rebuild. (Q1.3) — still valid after the pivot. |
| ~~D7~~ | ~~Packaging: ships inside `Nameless.Common`.~~ **Superseded by D29:** ships inside `Nameless.Windows` (where `ITranslator` already lives) — still no standalone NuGet package. |
| D8 | **v1 formatting:** plain `string.Format`-style `{0}` substitution only — no ICU plural/select. (Q1.5) |
| D9 | **Hot-reload required.** JSON files are watched on disk; on a failed/invalid update, log and ignore it — keep serving the last good in-memory state. |
| D10 | **JSON file structure** — one file per culture, top-level object keyed by `TYPE_FULL_NAME`, each value an array of single-key/value objects: `{ "TYPE_FULL_NAME": [ { "Key1": "Text1" }, { "Key2": "Text2" } ] }`. — **re-open:** this grouping matched `IStringLocalizer<T>`'s per-type `ResourceManager` convention; `ITranslator` is a single flat resource (one `ResourceFullName`), so the grouping key's purpose needs re-confirming post-pivot. See Q6.2. |
| D11 | **Schema shape confirmed as-is** — array of single-key objects per type, not flattened to a plain dict. (Q2.1) |
| D12 | **File resolution:** one JSON file per culture, holding all types, under a configurable root path. (Q2.2) |
| D13 | **Culture fallback chain:** `pt-BR` → `pt` → default (invariant) file. (Q2.3) |
| D14 | **Missing key:** return the key itself, `LocalizedString.ResourceNotFound = true`. (Q2.4) |
| D15 | **Hot-reload failure handling:** on parse error / shape mismatch / IO error — log the exception and, where possible, the main reason; ignore the update, keep last good state. (Q3.1) |
| D16 | **Cache granularity:** per culture file; a reload is always a full reload of that file (no partial/per-type reload). (Q3.2) |
| D17 | **File watcher:** raw `FileSystemWatcher`, not `PhysicalFileProvider`/`IChangeToken`. (Q3.3) |
| ~~D18~~ | ~~Duplicate keys within a type's array → shape error.~~ **Moot under D31:** flat-dict JSON has no per-type array; a duplicate top-level key is parsed silently (parser keeps the last occurrence) — not a shape error. |
| D19 | **Default/invariant file name:** `default.json`. (Q2.6) |
| D20 | **Root path configuration:** via an `Options` class, populated from `IConfiguration` using `GetSectionFor` (repo's existing config-binding helper). (Q2.7) |
| D21 | **DI registration:** must use the repo's `HostFeature` convention — re-pointed post-pivot at the existing `TranslatorHostFeature`/`TranslatorOptions` (no new `*StringLocalizer*`-named types). |
| ~~D22~~ | ~~`TranslatorExtension` switches its backing abstraction to `IStringLocalizer`.~~ **Superseded by D28 — moot:** `TranslatorExtension` already binds to `ITranslator`; nothing changes here. No code change needed in this class at all. |
| ~~D23~~ | ~~New `IStringLocalizer` indexer-extension class.~~ **Superseded by D28 — moot:** `ITranslator` already has the real indexer; no wrapper/extension type is needed. |
| ~~D24~~ | ~~Concrete wrapper class for WPF binding reflection.~~ **Superseded by D28 — moot:** same reason as D23. |
| ~~D25~~ | ~~Indexer-extension returns `string`.~~ **Superseded by D28 — moot:** `ITranslator`'s indexer already returns `string`. |
| ~~D26~~ | ~~Static field holds non-generic `IStringLocalizer`.~~ **Superseded by D28 — moot:** `TranslatorExtension`'s static field stays `ITranslator` — the open "concrete resource anchor" question goes away with it. |
| D27 | **Testing split:** `[UnitTest]` for fallback/formatting/missing-key logic against a fake file provider; `[IntegrationTest]` for real JSON file round-trip and hot-reload via `FileSystemWatcher`. (Q5.1) — still valid after the pivot. |
| D28 | **Pivot:** drop `IStringLocalizer`. Keep `ITranslator` (with `CurrentCulture`, `SetCulture`, `INotifyPropertyChanged`, both indexers) as the public contract, unchanged. Replace only its backing implementation — a new JSON-backed class replacing/alongside `ResourceTranslator` — keeping D6, D8–D21, D27. |
| D29 | **`ITranslator` stays in `Nameless.Windows`** — not moved to `Nameless.Common` or anywhere else. The JSON-backed implementation ships alongside it in the same library. |
| ~~D30~~ | ~~`TYPE_FULL_NAME` grouping kept.~~ **Superseded by D31.** |
| D31 | **JSON simplified to a flat dict per culture file** — `{ "Key1": "Text1", "Key2": "Text2" }`. `TranslatorOptions.ResourceFullName` is one process-wide string (confirmed in `TranslatorOptions.cs`/`ResourceTranslator.cs`); the `TYPE_FULL_NAME` multi-section grouping (D10/D11/D30) was a leftover from the dropped `IStringLocalizer<T>` design and is dropped. **Supersedes D10, D11, D30.** |
| D32 | **Missing key: return `"[{key}]"`** (bracketed), matching `ResourceTranslator.cs:63`'s actual current behavior exactly — not the bare key. **Supersedes D14's "return the key itself"** (which was based on XML-doc comments, not the real implementation). |
| D33 | **Hot-reload fires `PropertyChanged("Item[]")`** on a successful reload, same signal `SetCulture` already uses — so bound WPF controls refresh live when the JSON file changes on disk, not just on culture switch. |
| D34 | **`ResourceTranslator` (resx-backed) is deleted outright** once the JSON implementation ships — no dual-implementation/migration switch. |
| D35 | **Format-argument mismatch** (stored template has placeholders the supplied `parameters` don't satisfy) **keeps throwing `FormatException`**, matching current behavior exactly — not caught/logged. |

## 3. Background (what exists today)

**Repo scan (grep + `gh issue search`):**
- No existing `IStringLocalizer` or JSON-localization code anywhere in the repo.
- No prior epic or issue about localization exists on the tracker.

**`libs/windows/src/Nameless.Windows/Localization/`** — existing, related but distinct:
- `ITranslator` — WPF-specific, backed by `.resx`/`ResourceManager`, indexer-based lookup, has a `NullTranslator` default.
- Registered via a `*HostFeature` + `*Options` class pair — this is the repo's established convention for pluggable cross-cutting services.
- **Not to be merged with or replace this epic's work** — different contract (WPF binding vs. `Microsoft.Extensions.Localization.IStringLocalizer`), but its DI/null-object conventions should be mirrored for consistency.

**Related epics / tickets (context only):** none found.

## 4. Part 1 — Scope & Contract

**Goal (from product owner / inferred):** Provide a reusable, implementation-agnostic `IStringLocalizer`/`IStringLocalizerFactory` implementation whose resources live in JSON files, so any .NET app (not just WPF) can localize strings without `.resx`.

**Confirmed:** D1–D3.

### Working flow (provisional)

```
[Consumer code] --IStringLocalizer["key", args]--> [JsonStringLocalizer]
                                                          |
                                                   resolves culture file
                                                          v
                                            [JSON resource file(s) on disk]
                                                          |
                                                 cache (in-memory, per D?)
```

### Answered — Part 1

| # | Question | Answer |
|---|----------|--------|
| ~~Q1.1~~ | Consumer scope | Library-agnostic → D4 |
| ~~Q1.2~~ | Relation to `ITranslator` | Replaces it → D5 |
| ~~Q1.3~~ | Pain point | resx hard to edit, want hot-reload → D6 |
| ~~Q1.4~~ | Packaging | Inside `Nameless.Common`, no standalone package → D7 |
| ~~Q1.5~~ | v1 formatting | Plain `{0}` substitution only → D8 |

**Part 1: closed.**

## 5. Part 2 — JSON Schema & File Resolution

**Goal:** Define the JSON file shape and how files are located per culture.

**Confirmed:** D10 — one JSON file per culture, top-level keyed by `TYPE_FULL_NAME`, each value an array of single-key/value objects.

```json
{
    "TYPE_FULL_NAME": [
        { "Key1": "Text1" },
        { "Key2": "Text2" },
        { "KeyN": "TextN" }
    ]
}
```

**Observations (facts only):**
- `TYPE_FULL_NAME` matches .NET's convention for `IStringLocalizer<T>` / `ResourceManagerStringLocalizerFactory`, where the resource base name is the consuming type's full name — one JSON file likely holds every type's strings for a given culture, not one file per type.
- Entries are an array of single-key objects rather than a flat `{ "Key1": "Text1" }` object — unusual; needs confirmation of intent (e.g. preserving duplicate keys, ordering, or just how the file is hand-authored).

### Answered — Part 2

| # | Question | Answer |
|---|----------|--------|
| ~~Q2.1~~ | Schema shape (array vs flat dict) | Kept as-is: array of single-key objects → D11 |
| ~~Q2.2~~ | File discovery | One file per culture, all types, configurable root → D12 |
| ~~Q2.3~~ | Culture fallback | `pt-BR` → `pt` → default → D13 |
| ~~Q2.4~~ | Missing key | Return key itself, `ResourceNotFound = true` → D14 |

### Answered — Part 2 (follow-ups)

| # | Question | Answer |
|---|----------|--------|
| ~~Q2.5~~ | Duplicate keys in a type's array | Shape error → D18 |
| ~~Q2.6~~ | Default/invariant file name | `default.json` → D19 |
| ~~Q2.7~~ | Root path configuration | Options class via `IConfiguration.GetSectionFor` → D20 |

**Part 2: closed.**

## 6. Part 3 — Caching & Hot-Reload

**Confirmed:** D9, D15–D17.

### Answered — Part 3

| # | Question | Answer |
|---|----------|--------|
| ~~Q3.1~~ | Failure triggers/logging | Parse error / shape mismatch / IO error — log exception + main reason → D15 |
| ~~Q3.2~~ | Cache granularity | Per culture file, always full reload → D16 |
| ~~Q3.3~~ | Watcher mechanism | Raw `FileSystemWatcher` → D17 |

**Part 3: closed.**

## 7. Part 4 — DI Registration & Packaging

**Confirmed:** D21 — registration must use the repo's `HostFeature` mechanism.

**Background (from actual repo code, `libs/windows/src/Nameless.Windows/Localization/`):**
- `TranslatorHostFeature : HostFeature<TranslatorFeatureOptions>` — `Register(IHostApplicationBuilder, HostFeatureContext)` calls `builder.Services.ConfigureOptions<TranslatorOptions>(builder.Configuration)` then registers `ITranslator` (or `NullTranslator.Instance` as default) via `TryAdd`.
- `TranslatorOptions` is a `record` decorated `[ConfigurationSectionName("Translator")]` — this is almost certainly what D20's "`IConfiguration.GetSectionFor`" binding relies on (the section name attribute drives the lookup).

### Answered — Part 4

| # | Question | Answer |
|---|----------|--------|
| ~~Q4.1~~ | Host feature / options naming | **Superseded by D21/D29:** no new type names — re-point the *existing* `TranslatorHostFeature`/`TranslatorOptions` at the JSON-backed implementation instead. |
| ~~Q4.2~~ | Options surface | Root path (D20), default-culture override, watcher debounce — these fields are added to the existing `TranslatorOptions`, not a new options type. |
| ~~Q4.3~~ | Unconfigured default | No-op default — already covered by the existing `NullTranslator.Instance` fallback in `TranslatorHostFeature`; no new null type needed. |
| ~~Q4.4~~ | `IStringLocalizer<T>`/`IStringLocalizerFactory` registration | **Moot under D28** — nothing `IStringLocalizer`-shaped is registered; `TranslatorHostFeature` keeps registering `ITranslator`. |

**Part 4: closed** (re-confirmed post-pivot).

## 8. Part 5b — WPF Compatibility Layer — **voided by D28**

Every decision and question in this part (D22–D26, Q5b.1–Q5b.3) existed only because `TranslatorExtension` was going to switch from `ITranslator` to `IStringLocalizer`. Under the pivot, `TranslatorExtension` keeps binding to `ITranslator` exactly as it does today — **no code change in this class, no wrapper type, no indexer-extension, no reflection risk.** See the strikethroughs on D22–D26 above.

**Part 5b: voided — no longer applicable.**

**Part 1–4: closed. Part 5b: voided.**

## 9. Part 5 — Testing

### Answered — Part 5

| # | Question | Answer |
|---|----------|--------|
| ~~Q5.1~~ | Test categorization split | Confirmed — `[UnitTest]` for fallback/formatting/missing-key against a fake file provider; `[IntegrationTest]` for real JSON file round-trip and hot-reload (`FileSystemWatcher`) → D27 |

**Part 5: closed.**

## 10. Part 6 — Pivot Clarification

**Confirmed:** D28–D30.

### Answered — Part 6

| # | Question | Answer |
|---|----------|--------|
| ~~Q6.1~~ | Does `ITranslator` move out of `Nameless.Windows`? | No — stays put. → D29 |
| ~~Q6.2~~ | Keep `TYPE_FULL_NAME` JSON grouping despite `ITranslator` being a flat lookup? | Yes, kept as-is; `TranslatorOptions.ResourceFullName` now selects the section. → D30 |

**Part 6: closed.**

## 13. Part 7 — Pivot Refinement (PO re-review findings)

**Confirmed:** D31–D35.

**Background (from actual code, `ResourceTranslator.cs`):**
- Missing key returns `$"[{key}]"` — confirmed by reading the real implementation (line 63), not the XML doc comment D14 had relied on.
- `SetCulture` fires `PropertyChanged(new PropertyChangedEventArgs("Item[]"))` so every WPF indexer binding re-evaluates. This signal must also fire after a successful hot-reload for bound UI to pick up new text without a culture switch.
- `TranslatorOptions.ResourceFullName` is a single bound string (one `IOptions<TranslatorOptions>` value per process) — confirmed in both `TranslatorOptions.cs` and the sample's `appsettings.json`. There is no per-call or per-type selection today.

### Answered — Part 7

| # | Question | Answer |
|---|----------|--------|
| ~~Q7.1~~ | Missing-key format: bracketed `[key]` or bare key? | Keep `[key]` bracketed, matches real behavior → D32 |
| ~~Q7.2~~ | Should hot-reload fire `PropertyChanged("Item[]")`? | Yes → D33 |
| ~~Q7.3~~ | Keep `TYPE_FULL_NAME` grouping or flatten? | Flatten to plain dict → D31 |
| ~~Q7.4~~ | `ResourceTranslator` fate — delete or keep as migration aid? | Delete outright → D34 |
| ~~Q7.5~~ | Format-argument mismatch — keep throwing `FormatException`, or catch/log? | Keep throwing → D35 |

**Part 7: closed.**

**All parts closed.**

## 11. Dependencies

| Dependency | Owner | Source |
|------------|-------|--------|
| None identified — the D26-era dependency (`IStringLocalizer` resource anchor) is moot under D28. | — | — |

## 12. To settle at refinement

- Exact JSON schema example / sample payload beyond the shape already confirmed (D31's flat dict).
- Whether an ADR is warranted for the JSON file format and resolution strategy, since the sample WinApp's 9 direct-injection call sites depend on `ITranslator`'s behavior staying identical.
