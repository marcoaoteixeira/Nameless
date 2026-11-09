# JSON String Localizer

**Type:** Epic · **Key:** not created yet · **Project/Repo:** GitHub `marcoaoteixeira/Nameless` · **Team:** Solo maintainer
**Source:** brainstorming doc `JSON-String-Localizer-Epic.md` (decision refs D1–D35)

---

## Summary

Swap `ITranslator`'s backing store (in `Nameless.Windows`) from `.resx`/`ResourceManager` to JSON files, with hot-reload on file changes — so translations can be edited and added without a rebuild, and the sample WinApp sees them live. `ITranslator`'s contract, location, and every existing consumer (including `TranslatorExtension`'s XAML binding and the 9 direct-injection call sites in the sample WinApp) stay unchanged, and so does every behavior detail already visible today (missing-key format, format-mismatch errors) — only the storage format changes.

This epic:
1. **JSON-backed `ITranslator` implementation** — reads a flat JSON dict per culture, with fallback and missing-key handling matching today's `ResourceTranslator` exactly, replacing it outright.
2. **Hot-reload** — watches JSON files on disk, reloads on change, and refreshes bound WPF controls live (same `PropertyChanged("Item[]")` signal `SetCulture` already uses).
3. **DI registration** — re-points the existing `TranslatorHostFeature`/`TranslatorOptions` at the new implementation; no new public type names.
4. **Tests** — unit tests for lookup/fallback/formatting logic, integration tests for real file I/O and hot-reload.

## Background

- **Current state** (`libs/windows/src/Nameless.Windows/Localization/`): `ITranslator` — `INotifyPropertyChanged`, `CurrentCulture`, `SetCulture(...)`, indexers `this[key]` / `this[key, params object[] parameters]` — backed today by `ResourceTranslator` (resx/`ResourceManager`). `ResourceTranslator` returns `"[{key}]"` on a missing key, throws `FormatException` on a format-argument mismatch, and fires `PropertyChanged("Item[]")` on `SetCulture` so bound indexers re-evaluate. `TranslatorOptions.ResourceFullName` is a single process-wide string. Registered via `TranslatorHostFeature : HostFeature<TranslatorFeatureOptions>`, falling back to `NullTranslator.Instance` when unconfigured.
- **Why this shape:** an earlier design explored replacing `ITranslator` with `Microsoft.Extensions.Localization.IStringLocalizer`. Review found `ITranslator` is injected directly in 9 places in the sample WinApp and relies on `CurrentCulture`/`SetCulture`/change-notification, none of which `IStringLocalizer` has an equivalent for — so `ITranslator` is kept exactly as it is; only what's behind it changes. A further review pass found the originally-planned per-type JSON grouping (`TYPE_FULL_NAME` sections) didn't match `ResourceFullName` being a single process-wide value — simplified to a flat dict.
- **Existing building blocks reused:** `TranslatorHostFeature`, `TranslatorOptions`, `NullTranslator`, `TranslatorExtension` (WPF XAML markup extension) — none change shape, only what `TranslatorOptions`/`TranslatorHostFeature` wire up internally.

## Flow (JSON resolution + hot-reload)

```
[Any ITranslator consumer] --this["key", args]--> [JSON-backed ITranslator impl]
   (ViewModels, handlers,                                  |
    TranslatorExtension/XAML)              resolve culture: pt-BR -> pt -> default.json
                                                             |
                                            [cached per-culture flat dict, in-memory]
                                                             ^
                                                             |
                                       [FileSystemWatcher] --reload on change--
                                   (success -> fire PropertyChanged("Item[]"), refresh UI)
                                   (parse/shape/IO error -> log, keep last good)
```

## Scope

### 1. JSON-backed `ITranslator` implementation
- Implements `ITranslator` as-is (no interface change); replaces `ResourceTranslator` outright in `libs/windows/src/Nameless.Windows/Localization/` — not moved to `Nameless.Common`, no standalone NuGet package.
- One flat JSON dict per culture file: `{ "Key1": "Text1", "Key2": "Text2" }`.
- Default/invariant file: `default.json`. Culture fallback chain: specific → neutral → `default.json`.
- Missing key: returns `"[{key}]"` (bracketed) — matches `ResourceTranslator`'s current behavior exactly.
- Format-argument mismatch (template placeholders don't match supplied `parameters`): throws `FormatException`, same as today — not caught or logged.
- v1 formatting: plain `string.Format`-style `{0}` substitution only — no ICU plural/select.
- Root path configurable via `TranslatorOptions`, bound from `IConfiguration`.

### 2. Hot-reload
- Raw `FileSystemWatcher` per configured root path.
- On change: reload the affected culture file in full (no partial reload).
- On successful reload: fire `PropertyChanged("Item[]")` (same signal `SetCulture` uses) so bound WPF controls refresh live.
- On parse error / shape mismatch / IO error: log the exception and, where available, the specific reason; ignore the update and keep serving the last good in-memory state.
- Cache granularity: per culture file.

### 3. DI registration
- No new public types. `TranslatorHostFeature`/`TranslatorOptions` are extended (root path, default-culture override, watcher debounce added to `TranslatorOptions`) and re-pointed at the JSON-backed implementation.
- Unconfigured behavior unchanged: falls back to `NullTranslator.Instance`.

### 4. Tests
- `[UnitTest]`: fallback chain, formatting, missing-key behavior, format-mismatch exception — against a fake/in-memory file provider.
- `[IntegrationTest]`: real JSON file round-trip, hot-reload via `FileSystemWatcher`, and the `PropertyChanged("Item[]")` signal firing on reload.

## Out of scope
- ICU-style pluralization/select syntax (v1 is `{0}`-style substitution only).
- `Microsoft.Extensions.Localization.IStringLocalizer` — explored and dropped; see Background.
- Per-type/multi-section JSON grouping — explored and dropped; see Background.
- Any change to `ITranslator`'s public contract, `TranslatorExtension`'s XAML binding behavior, or any of its 9 existing direct-injection consumers in the sample WinApp.
- Keeping `ResourceTranslator`/resx support alongside the JSON implementation — it is deleted outright, not kept as a migration option.
- Standalone NuGet package.

## Dependencies
None identified.

## To settle at refinement
- Exact JSON schema example / sample payload beyond the flat-dict shape already confirmed.
- Whether an ADR is warranted for the JSON file format and resolution strategy, since the sample WinApp's 9 direct-injection call sites depend on `ITranslator`'s behavior staying identical.

## Candidate tickets

| Area | Ticket | Acceptance criteria |
|------|--------|---------------------|
| Core | JSON-backed `ITranslator` implementation, replacing `ResourceTranslator` | Given a culture file `{ "Key1": "Text1" }`, `translator["Key1"]` returns `"Text1"`; `pt-BR` falls back to `pt`, then `default.json`; missing key returns `"[Key1]"`; `translator["Greeting", "Marco"]` substitutes positional args via `string.Format`; a format-argument mismatch throws `FormatException`, matching today's behavior. `ResourceTranslator` and its resx dependency are removed. |
| Hot-reload | `FileSystemWatcher`-based reload with UI refresh and failure handling | Editing a culture JSON file on disk updates subsequent lookups without an app restart, replacing the previous cache for that culture in full, and fires `PropertyChanged("Item[]")` so bound XAML controls refresh immediately; an invalid edit (parse error, wrong shape, or IO error) is logged with the exception and reason, and the previous good cache is served unchanged (no refresh fired). |
| DI | Re-point `TranslatorHostFeature`/`TranslatorOptions` at the JSON-backed implementation | Registering the host feature with a configured root path resolves the JSON-backed `ITranslator` from DI; omitting configuration falls back to `NullTranslator.Instance`, matching today's behavior. |
| Quality | Unit tests for core lookup/fallback/formatting | Covers: exact match, fallback chain, missing key format, format-arg substitution, format-mismatch exception — all against a fake file provider. |
| Quality | Integration tests for file I/O and hot-reload | Covers: real JSON file read at startup, on-disk edit triggers reload and UI-refresh signal, invalid edit is logged and ignored. |

## References
- Linked docs: none
- Tracker: not created yet
