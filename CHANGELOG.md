# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- `Nameless.Windows`'s `JsonTranslator` hot-reloads translation files edited on disk while
  the app is running: `FileSystemTranslationFileProvider` now watches its `RootPath` folder
  with a `FileSystemWatcher`, debouncing rapid successive events for the same file through
  the new `TranslatorOptions.WatcherDebounceDelay` option before raising the new
  `ITranslationFileProvider.Changed` event. `JsonTranslator` reloads only the affected
  culture's file on `Changed`, replacing its cached entries and re-raising the `"Item[]"`
  `PropertyChanged` signal on success; a malformed or unreadable file is logged and leaves
  the last known-good cached content (and the UI) untouched. All `JsonTranslator` state
  shared between the UI thread and the watcher's reload callbacks is now synchronized, so
  two files changing together (e.g. a bulk save or a `git pull`) can no longer corrupt the
  cache or leave the UI stale despite both reloads succeeding.
  `ITranslationFileProvider.Changed` has a default no-op implementation, so adding it is
  not a breaking change for a provider that does not watch for changes.
  ([#191](https://github.com/marcoaoteixeira/Nameless/issues/191))
- `Nameless.WinApp` self-update: the `AppConfigurationPage` Version card checks GitHub
  Releases for the newest non-draft, non-prerelease `winapp-v*` tag, confirms with the
  user, then downloads, SHA-256-verifies, extracts and swaps the release's files into
  the install directory in place of the running application — renaming each replaced
  entry aside as a `.old-{timestamp}` sibling first, so a pre-commit failure rolls
  back to byte-identical originals and a post-commit failure (including the final
  success notification) never undoes already-swapped files. The app relaunches itself
  and shuts down on success; a new `CleanupPreviousSystemUpdateStep` bootstrap step
  deletes the `.old-*`/`.update-*` leftovers once the new version proves it can start.
  Also fixes `IApplicationContext.Version` always showing `v1.0.0`: the assembly's
  informational version (no `+<sha>` suffix) is now passed to the host as
  `--ApplicationContext:Version`, and only a release build stamped with
  `-p:SelfUpdateEnabled=true` carries the marker that allows self-update to run.
  New `.github/workflows/winapp-release.yml` builds and publishes WinApp releases per
  `docs/specs/winapp-self-update-release-contract.md`.
  ([#167](https://github.com/marcoaoteixeira/Nameless/issues/167))
- Application restore: the `AppConfigurationPage` restore command extracts a backup
  zip, validates its contents (JSON-parses `app.config`, runs SQLite's `PRAGMA
  quick_check` on `app.db`), and swaps the validated artifacts into place, renaming
  the previous files aside as a rollback point instead of overwriting them outright.
  On success the user is prompted to restart the app — `AppDbContext` and the Lucene
  `IIndexProvider` are not designed to be torn down and recreated against swapped
  files while the process keeps running. A validation or swap failure rolls every
  swapped artifact back to its original file and leaves the app usable without a
  restart. ([#162](https://github.com/marcoaoteixeira/Nameless/issues/162))
- `IAppConfigurationManager.Reload()`: discards in-memory configuration changes and
  reloads from the backing `app.config` file. Restore calls it right after swapping
  a restored `app.config` into place, so a later shutdown-time `SaveChangesAsync()`
  persists the reloaded values instead of clobbering the restore with stale,
  pre-reload in-memory data. ([#162](https://github.com/marcoaoteixeira/Nameless/issues/162))
- `Nameless.Windows`'s `ITranslator<T>` and `ITranslatorFactory`: inject `ITranslator<T>`
  to get a translator scoped to the section mapped from `T` (its simple name, generic
  arity stripped), in the style of `ILogger<T>`/`IStringLocalizer<T>` — index it with the
  section-relative key only, no stack-walking or hand-built key prefix needed.
  `ITranslatorFactory.Create(Type)`/`Create(string)`, plus a `Create<T>()` extension,
  cover resolution outside DI (e.g. code-behind). Registered as an open-generic singleton
  by `TranslatorHostFeature` alongside the root `ITranslator`, with or without a
  `Translator` configuration section. `Nameless.WinApp`'s request handlers, bootstrap
  steps, `AppConfigurationPageViewModel` and `MainWindow` now use it; the old
  reflection/call-stack-based `ITranslator.Get(...)` extension and `CallerInfo` helper
  are removed from the sample app. ([#199](https://github.com/marcoaoteixeira/Nameless/issues/199))

### Changed

- `Nameless.Windows`'s `JsonTranslator` now expects each culture's JSON file to group
  its keys by section — `{ "SectionName": { "Key1": "Text1" } }` — instead of a flat
  `{ "Key1": "Text1" }` object, purely so a human editing the file by hand can find related
  keys together. **Keys are section-relative on disk** (#199 amends #197's initial shape):
  `JsonTranslator` flattens each entry to `{Section}_{Key}` (`TranslationKey.Compose`), and
  that composed string is the actual lookup key (`translator["SectionName_Key1"]`, not
  `translator["Key1"]`) — the section name is now written once, not twice. A key duplicated
  within the same section, a section name duplicated at the top level, an empty/white-space
  section or key name, or two different sections composing to the same flat key (only
  possible when a section/key name itself contains `_`) is logged and treated as a
  malformed file (falls back through the culture chain) instead of silently picking one of
  the values. Not marked as a breaking change: `JsonTranslator` itself has not shipped in a
  release yet (see the #191 and #190 entries above).
  ([#197](https://github.com/marcoaoteixeira/Nameless/issues/197),
  [#199](https://github.com/marcoaoteixeira/Nameless/issues/199))
- `AppConfigurationManager.SaveChangesAsync` now writes `app.config` through a
  write-then-atomic-rename (temp file on the same volume, moved into place with
  `overwrite: true`) instead of writing the destination file directly, so a reader
  racing the write (e.g. a freshly relaunched process) never observes a
  truncated/partial file. ([#162](https://github.com/marcoaoteixeira/Nameless/issues/162))
- `Nameless.Common` depends on `Microsoft.EntityFrameworkCore.Sqlite.Core` (provider-
  agnostic) instead of the full `Microsoft.EntityFrameworkCore.Sqlite` package, so
  library consumers keep the ability to choose their own SQLite native provider; the
  full package (and its transitive `SQLitePCLRaw.bundle_e_sqlite3` native bundle) is
  now referenced only by the sample app, `Nameless.WinApp`.
  ([#162](https://github.com/marcoaoteixeira/Nameless/issues/162))

### Removed

- **BREAKING:** `ResourceTranslator` and `TranslatorOptions.ResourceFullName` are removed.
  `Nameless.Windows`'s `TranslatorHostFeature` now resolves `JsonTranslator` (backed by
  `TranslatorOptions.RootPath` and a disk-backed `ITranslationFileProvider`) when a
  `Translator` configuration section is present, falling back to `NullTranslator.Instance`
  otherwise. Consumers still referencing `ResourceTranslator`/`ResourceFullName` must switch
  to the JSON-backed translator and its `RootPath` configuration.
  ([#190](https://github.com/marcoaoteixeira/Nameless/issues/190))

### Fixed

- Fixed `JsonTranslator` letting a `null`/empty/whitespace translation value in a more
  specific culture file (e.g. `pt-BR`) silently overwrite a valid value already merged
  in from a less specific culture (`pt`/`default`), since `System.Text.Json` deserializes
  a JSON `null` into a `string` dictionary value without error. `TryParseFile` now treats
  a blank entry value the same as every other malformed shape it already rejects: logged
  at Warning and the whole file's parse fails, so the merge falls back to the next
  culture in the chain instead of rendering `"[key]"`.
  ([#201](https://github.com/marcoaoteixeira/Nameless/issues/201))
- Fixed `PerformSystemUpdateRequestHandlerTests.HandleAsync_HappyPath_...` failing
  under Visual Studio/Rider Test Explorer (`RolledBack` instead of `Completed`). The
  test used `Environment.ProcessPath` to get a real PE to stamp as the fake update
  package, assuming the current process is always the test project's own apphost —
  true for `dotnet test` (pinned to Microsoft.Testing.Platform), but Test Explorer
  launches the host via the `dotnet` muxer against the managed dll, so
  `ProcessPath` resolved to `dotnet.exe` and its version never matched. Now derived
  from the test assembly's own location instead.
  ([#173](https://github.com/marcoaoteixeira/Nameless/issues/173))
- Fixed a restore regression where a restored `app.config` was silently overwritten
  the next time the app shut down, because the long-lived
  `IAppConfigurationManager` singleton never reloaded the swapped-in file after a
  restore — see the `Reload()` addition above.
  ([#162](https://github.com/marcoaoteixeira/Nameless/issues/162))
- Fixed a data-loss bug in `PerformApplicationRestoreRequestHandler` where a failure
  occurring after artifacts were restored and the config reloaded, but before backup
  cleanup or the success notification finished, rolled every restored artifact back
  to its original file while still reporting `RolledBack` — deleting the live,
  already-restored data it claimed to have rolled back. A `committed` flag is now set
  right after the last failable pre-commit step (`Reload()`); once set, no later
  failure can trigger `RollbackSwapsAsync`, and post-commit cleanup/notification
  failures are logged and swallowed instead of propagating.
  ([#163](https://github.com/marcoaoteixeira/Nameless/issues/163))
- Fixed `AppConfigurationManager.SaveChangesAsync` using a per-call GUID-suffixed temp
  file name, which let one writer's failure-cleanup delete another writer's
  still-in-flight temp file. The atomic write now uses a single fixed temp file name
  (`app.config.tmp`) and only deletes it on failure if the current call actually
  created it.
  ([#163](https://github.com/marcoaoteixeira/Nameless/issues/163))
- Fixed `dotnet test` being broken repo-wide on the .NET 10 SDK, which no longer
  supports the legacy VSTest target: a new root `global.json` pins the test runner to
  Microsoft.Testing.Platform. This also unblocks the `winapp-v*` release workflow's
  test step. One side effect is handled too — `Nameless.Web.Tests` has no test files
  yet, and MTP treats "zero tests ran" as a failure (exit code 8); that exit code is
  now ignored for that project only, until real tests are added.
  ([#167](https://github.com/marcoaoteixeira/Nameless/issues/167))
