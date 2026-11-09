# JSON String Localizer — Tickets

Epic: [#187](https://github.com/marcoaoteixeira/Nameless/issues/187) "JSON String Localizer"

| # | Issue | Status |
|---|-------|--------|
| 1 | [#188](https://github.com/marcoaoteixeira/Nameless/issues/188) — JSON-backed ITranslator core | Backlog |
| 2 | [#189](https://github.com/marcoaoteixeira/Nameless/issues/189) — Migrate resx to JSON | Backlog (blocked by #188) |
| 3 | [#190](https://github.com/marcoaoteixeira/Nameless/issues/190) — Re-point DI, retire ResourceTranslator | Backlog (blocked by #188, #189) |
| 4 | [#191](https://github.com/marcoaoteixeira/Nameless/issues/191) — Hot-reload | Backlog (blocked by #190) |
| 5 | [#192](https://github.com/marcoaoteixeira/Nameless/issues/192) — Integration tests | Backlog (blocked by #191) |

All attached as sub-issues of #187; dependencies marked via `Blocks #N` in blocker bodies + `blocked` label on dependents (GitHub has no native blocking link).

Source: `JSON-String-Localizer-Epic.md` (brainstorm, D1–D35), `JSON-String-Localizer-Epic-Tracker.md` (tracker-ready), PO breakdown (open questions Q1/Q2 resolved below).

Resolved open questions:
- **Migrate 119 existing resx keys (default + pt-BR) to JSON** as part of this epic → ticket #2.
- **Short ADR** for JSON-over-resx / flat-dict / `ITranslator`-kept decisions → folded into ticket #1, not standalone.
- **No generic "write unit tests" tickets** → coverage folded into tickets #1 and #4's own acceptance criteria. Ticket #5 (integration tests, real filesystem) kept standalone — genuinely separate risk.

---

## 1. [Story] JSON-backed `ITranslator` core: lookup, culture fallback, missing-key and format behavior

**Value:** A JSON-backed `ITranslator` implementation with zero behavioral drift for any of the 9 existing consumers — the foundation the rest of the epic builds on.

**Context:** Replaces `ResourceTranslator` (`libs/windows/src/Nameless.Windows/Localization/ResourceTranslator.cs`). Stays in `Nameless.Windows`, no new package. `ITranslator`'s contract is unchanged.

**Scope:**
- New `ITranslator` implementation reading one JSON file per culture, flat dict shape `{ "Key1": "Text1" }`.
- Culture fallback chain: specific (`pt-BR`) → neutral (`pt`) → `default.json`.
- Missing key returns `"[{key}]"` (bracketed), matching current behavior exactly.
- `this[key, parameters]` applies `string.Format`-style `{0}` substitution; a format-argument mismatch throws `FormatException`, uncaught — matching current behavior exactly.
- Root path for JSON files configurable via `TranslatorOptions` (new field), bound through `GetSectionFor<TranslatorOptions>()`.
- Short ADR capturing: why JSON over resx, why flat-dict not per-type, why `ITranslator` kept over `IStringLocalizer`.
- Fix `ITranslator.cs`'s stale XML doc comment ("returns the key itself") to say `"[key]"`.

**Out of scope:** Hot-reload (ticket #4), DI re-registration / deleting `ResourceTranslator` (ticket #3), migrating the sample app's existing translation content (ticket #2).

**Acceptance criteria:**
- Given a culture file `{ "Key1": "Text1" }`, `translator["Key1"]` returns `"Text1"`.
- Given no `pt-BR.json` but a `pt.json` and a `default.json`, requesting a key under culture `pt-BR` returns the value from `pt.json`.
- Given none of `pt-BR.json`/`pt.json` exist, the value comes from `default.json`.
- Given a key absent from the resolved file, the indexer returns `"[Key1]"`.
- Given `translator["Greeting", "Marco"]` where `Greeting = "Hello, {0}"`, the result is `"Hello, Marco"`.
- Given a template with a placeholder the supplied parameters don't satisfy, the indexer throws `FormatException`.

**Assumptions:** JSON file access in this ticket is via a fake/in-memory file abstraction, not real disk I/O (real I/O arrives with hot-reload in ticket #4/#5).

**Dependencies:** None.

**Notes:** Define the file-abstraction seam so ticket #4 can swap in `FileSystemWatcher` without reworking lookup/cache logic. Unit-test against the fake provider per the AC above — `[UnitTest]`.

---

## 2. [Task] Migrate existing sample-app translations from resx to JSON

**Value:** WinApp users keep every currently-localized string (119 keys × 2 cultures) working, unchanged, once the backing store swaps to JSON — no regression from this epic.

**Context:** `samples/windows/Nameless.WinApp/Resources/loc/TranslationResources.resx` (default) and `TranslationResources.pt-BR.resx` (pt-BR), 119 keys each, currently wired via `appsettings.json`'s `Translator:ResourceFullName`.

**Scope:**
- Produce `default.json` and `pt-BR.json` (flat dict, per ticket #1's shape) with the same keys/values as the two existing resx files, byte-for-byte equivalent text.
- Update the sample app's `appsettings.json` `Translator` section to point at the new root path instead of `ResourceFullName`.

**Out of scope:** Any wording/content change to the translated strings — format conversion only. Deleting the resx files themselves (done in ticket #3, alongside `ResourceTranslator`'s removal).

**Acceptance criteria:**
- Given the generated `default.json`, every key present in `TranslationResources.resx` is present with the identical value.
- Given the generated `pt-BR.json`, every key present in `TranslationResources.pt-BR.resx` is present with the identical value.
- No key present in one culture's JSON is missing from the other.

**Assumptions:** The two resx files currently have identical key sets (confirmed: both report 119 `<data name=...>` entries) — if that's false on closer inspection, reconcile the mismatch rather than silently dropping a key.

**Dependencies:** Ticket #1 (needs the confirmed flat-dict shape).

**Notes:** Mechanical/scriptable conversion — a one-off script is fine. QA: diff the converted JSON against the resx content for the full key set, not a sample.

---

## 3. [Story] Re-point DI at the JSON-backed implementation and retire `ResourceTranslator`

**Value:** The sample WinApp (and any future consumer) sources all translations from JSON through the existing `ITranslator`/`TranslatorHostFeature` registration — no code changes in any of the 9 consumer call sites or `TranslatorExtension`.

**Context:** `TranslatorHostFeature.Register(...)` currently registers `ResourceTranslator` or `NullTranslator.Instance` when unconfigured. `TranslatorOptions.ResourceFullName` is the single resx-pointing field.

**Scope:**
- `TranslatorOptions` gains the JSON root-path field (and a default-culture override field) and drops `ResourceFullName` (resx-specific, no longer used).
- `TranslatorHostFeature` registers the JSON-backed implementation (ticket #1) in place of `ResourceTranslator`.
- `ResourceTranslator` class and its resx dependency deleted outright — including the sample app's two `.resx` files once ticket #2's JSON equivalents are confirmed correct.
- Unconfigured behavior unchanged: falls back to `NullTranslator.Instance`.

**Out of scope:** Hot-reload wiring (ticket #4) — this ticket only needs the JSON file read once at startup.

**Acceptance criteria:**
- Given `TranslatorOptions` configured with a valid JSON root path, resolving `ITranslator` from DI returns the JSON-backed implementation, and lookups return the sample app's migrated strings correctly for both default and `pt-BR` cultures.
- Given no `Translator` configuration section at all, resolving `ITranslator` from DI returns `NullTranslator.Instance`, matching today's behavior.
- `ResourceTranslator`, its resx files, and `ResourceFullName` are fully removed; the solution builds with no dangling references.
- The sample WinApp's 9 existing `ITranslator` consumers and `TranslatorExtension`'s XAML bindings compile and run unmodified.

**Assumptions:** Ticket #2's JSON content is validated/merged before this ticket's `ResourceTranslator` deletion lands — no window where the app runs with new DI wiring but old/missing content.

**Dependencies:** Ticket #1, ticket #2.

**Notes:** Double-check `TranslatorFeatureOptions`'s assembly-scan path still resolves the new concrete type the same way it resolved `ResourceTranslator`. QA: regression-check `SetCulture`'s UI refresh still works post-swap.

---

## 4. [Story] Hot-reload: watch JSON files on disk and refresh bound UI live

**Value:** Whoever maintains translation content can edit a culture's JSON file on disk and see the WinApp's UI update immediately, with no rebuild/restart — directly solves the stated pain point (resx being hard to edit).

**Context:** `SetCulture` already fires `PropertyChanged(new PropertyChangedEventArgs("Item[]"))` so every bound indexer re-evaluates; hot-reload reuses that exact signal.

**Scope:**
- A `FileSystemWatcher` per configured root path, watching the relevant culture JSON files.
- On a file change: reload that culture's file in full (no partial reload), replace the in-memory cache for that culture.
- On successful reload: fire `PropertyChanged("Item[]")` so bound WPF controls refresh without a culture switch.
- On parse error / shape mismatch / IO error during reload: log the exception and reason where available; keep serving the last known-good state; do not fire the refresh signal.
- Add a watcher-debounce option to `TranslatorOptions` so rapid successive file-system events don't cause a reload storm.
- Unit-test the reload/cache/logging logic against a fake provider (folded in per the "no generic test ticket" decision) — `[UnitTest]`.

**Out of scope:** ICU plural/select syntax, per-type JSON sections — both explicitly dropped.

**Acceptance criteria:**
- Given the app is running and a culture's JSON file is edited on disk with valid content, a subsequent lookup for a key in that file returns the new value without an app restart, and bound XAML controls visibly refresh.
- Given an edit introduces invalid JSON or a shape that isn't a flat string dict, the exception and reason are logged, the previous good state is still served unchanged, and no refresh signal fires.
- Given two rapid successive edits to the same file within the debounce window, only one reload/refresh occurs.

**Assumptions:** `FileSystemWatcher`'s raw event stream (including duplicate/rename-pattern events on Windows) is tamed by the debounce window — if insufficient in testing, revisit scope before closing.

**Dependencies:** Ticket #3.

**Notes:** Riskiest ticket in the epic — `FileSystemWatcher` is flaky around debounce/duplicate events (repo already has a known flaky CI timing issue elsewhere, #179). Size the debounce mechanism (timer-based coalescing is the common fix) up front.

---

## 5. [Task] Integration tests for real JSON file I/O and hot-reload lifecycle

**Value:** Confidence that hot-reload behaves correctly against the real filesystem and real `FileSystemWatcher` timing — what unit tests against a fake provider (tickets #1/#4) cannot cover — before relying on it in the sample app.

**Context:** Tickets #1/#4's AC are unit-level (fake file provider). This covers what only shows up with real disk I/O: startup load timing, watcher event timing/debounce, OS-level file lock/partial-write edge cases.

**Scope:**
- `[IntegrationTest]`-categorized tests covering: real JSON file read at startup; on-disk edit triggers reload and the `PropertyChanged("Item[]")` signal; an invalid edit is logged and ignored (previous state still served); rapid successive edits are debounced to one reload.
- Runs against real temp-directory JSON files and a real `FileSystemWatcher`, not fakes.

**Out of scope:** Unit-level behavior already covered by tickets #1/#4's AC — not re-tested here against real files.

**Acceptance criteria:**
- A test suite exists, categorized `[IntegrationTest]`, exercising each of the four scenarios in Scope against real files on disk.
- The suite passes reliably on CI (no new flaky test) — if timing proves unreliable, report it rather than paper over with sleeps.

**Assumptions:** CI environment allows real file-system watch events to fire within a reasonable test timeout; if unreliable (echoing #179's CI timing flakiness), revisit ticket #4's debounce/test design rather than widening timeouts indefinitely.

**Dependencies:** Ticket #4.

**Notes:** Watch for Windows file-lock behavior when a test writes then immediately reads a file the watcher is also observing — classic flaky-test source.
