# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

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

### Changed

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

### Fixed

- Fixed a restore regression where a restored `app.config` was silently overwritten
  the next time the app shut down, because the long-lived
  `IAppConfigurationManager` singleton never reloaded the swapped-in file after a
  restore — see the `Reload()` addition above.
  ([#162](https://github.com/marcoaoteixeira/Nameless/issues/162))
