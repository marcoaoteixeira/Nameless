# #163 — Restore: commit point after the artifact swap; fixed temp file for app.config saves

- Ticket: [#163](https://github.com/marcoaoteixeira/Nameless/issues/163) (follow-up from #162 review, round 3)
- Stack: .NET 10 / WPF. Sample app `Nameless.WinApp` and library `Nameless.Windows`.
- Agent pair: `backend-developer-dotnet` / `backend-reviewer-dotnet`. No frontend side.
- Work split: BACKEND-ONLY.
- UX spec: not needed. No new screen or state. The visible change is that a late failure no longer shows a
  "rolled back" toast for a restore that actually finished.
- ADR: not needed. This is a local bug fix inside one handler and one class. No framework, external API or
  data-model change.

## 1. Context: what the code does today

Files checked on `feature/163-restore-commit-point` (same as `main`):

- `samples/windows/Nameless.WinApp/Features/PerformApplicationRestore/PerformApplicationRestoreRequestHandler.cs`
- `libs/windows/src/Nameless.Windows/AppConfiguration/AppConfigurationManager.cs`
- `samples/windows/Nameless.WinApp.Tests/Features/PerformApplicationRestore/PerformApplicationRestoreRequestHandlerTests.cs`
- `libs/windows/tests/Nameless.Windows.Tests/AppConfiguration/AppConfigurationManagerTests.cs`
- `samples/windows/Nameless.WinApp/ViewModels/Pages/AppConfigurationPageViewModel.cs` (the caller: it offers
  a restart only on `RestoreOutcome.Completed`)

The `try` block of `HandleAsync` runs these steps in order: `RunStepAsync("RestoringArtifacts")`, then
`DeleteBackupSiblingsAsync(swaps)`, then `PublishProgressAsync("Success")`, then `Reload()` (only when
`ConfigPresent`), then `return Completed`. Both `catch` blocks call `RollbackSwapsAsync(swaps)` with no
condition. So the defect described in the ticket is real. If anything after `DeleteBackupSiblingsAsync`
throws, `RollbackSwapsAsync` deletes each restored `OriginalPath`, then `Move(BackupPath, ...)` throws
`FileNotFoundException`. That exception is swallowed by its own per-swap catch, which publishes "RollingBack
failure". The method then returns `RolledBack`, but the live artifacts are gone.

### Corrections to the ticket

1. **An existing test pins the buggy behavior.**
   `HandleAsync_WhenSuccessPublishFailsAfterConfigSwap_NeverReloadsAheadOfTheRollback` (around line 378)
   forces the `Success` publish to throw and asserts `RolledBack` and `Reload()` `Times.Never`. Its comment
   even names this gap. The fix changes that test's expected outcome, so it must be **rewritten**, not kept
   next to a new test. The ticket's "add a test" is really "replace this test".
2. **Where `Reload()` goes: a refinement of the ticket.** The ticket puts `Reload()` first in the
   post-commit tail and only logs it if it fails. That leaves a gap. If `Reload()` throws (it can:
   `file.Open()` in `GetAppConfiguration` is not guarded against `IOException`), the method would return
   `Completed` with stale values in memory. The restart prompt then shuts the app down, and the host's
   shutdown-time `SaveChangesAsync` (`App.xaml.cs`) **overwrites the restored `app.config` with the
   pre-restore values**. That is the exact data loss #162 added `Reload()` to prevent.
   **Decision:** `Reload()` is the **last action before the commit point**, not the first action after it.
   - If it throws, the `.bak-*` backups still exist, so the rollback works fully. `Reload()` only assigns
     `_appConfiguration` after `GetAppConfiguration()` returns, so the in-memory state stays on the
     original values. Disk and memory agree, and `RolledBack` is reported truthfully.
   - If it succeeds, nothing after it can trigger a rollback. So the #162 concern ("memory holds the
     restored values while disk is rolled back") can no longer happen.
3. **The tail failure must not depend on the event publisher alone.** The failure the ticket names is the
   publisher itself throwing. A post-commit catch that only "publishes a warning" can throw again from
   inside the catch and escape `HandleAsync`. The tail's catch must (a) write to a logger, and (b) treat
   any warning publish as best effort inside its own guard.
4. **Late cancellation.** The current `catch (OperationCanceledException)` would also turn a cancellation
   after the commit into `RolledBack_Cancelled`. Today no tail step observes the token, but the tail guard
   must catch `Exception` (which includes OCE), so a late OCE still ends in `Completed`.

## 2. Approach: restore handler

Target shape of `HandleAsync` (described in words, the developer writes the code):

```
var committed = false;
try {
    ... unchanged steps up to and including RunStepAsync("RestoringArtifacts", ...) ...

    if (validated.ConfigPresent) { _appConfigurationManager.Reload(); }   // last failable pre-commit action

    committed = true;   // commit point: from here on, never roll back

    await CompleteCommittedRestoreAsync(swaps);   // never throws: see below
    return RestoreOutcome.Completed;
}
catch (OperationCanceledException) when (!committed) { rollback; publish RolledBack_Cancelled; return RolledBack; }
catch (Exception) when (!committed)                 { rollback; publish RolledBack; return RolledBack; }
finally { DeleteExtractedDirectory(extractedDirectory); }
```

- `CompleteCommittedRestoreAsync` (name up to the developer) runs `DeleteBackupSiblingsAsync(swaps)` and
  then `PublishProgressAsync("Success")`. Each runs in its own guard, so a failed cleanup still lets the
  success publish run. Each guard catches `Exception`, logs it, and swallows it.
- There are two ways to keep `RollbackSwapsAsync` from running after the commit: the guarded tail, or
  `when (!committed)` filters. Use **both**. The guarded tail is the main mechanism. The filters (or an
  `if (!committed)` check inside each catch, as the ticket says) are a second safeguard: a future edit that
  adds an unguarded call after the commit then fails loudly instead of deleting live files. With
  exception filters, a post-commit exception that escaped would propagate. That is the correct outcome:
  never report `RolledBack` for a committed restore.
- Rewrite the comment block above the old `Reload()` call. It describes the old ordering.
- **Logging:** add `ILogger<PerformApplicationRestoreRequestHandler>` to the constructor (DI already
  provides `ILogger<T>`). Log tail failures at Warning level, following the `CommonLog` / `ILogger` pattern
  used in `AppConfigurationManager`. Tests pass `NullLogger<T>.Instance` or a mock.
- No new translation keys are needed. If you want a user-facing warning for the post-commit failure,
  reuse the existing `_DeletingBackupSiblings_Failure` key for cleanup. Do not add a toast for a failed
  success publish (the publisher is what is broken).

### Alternatives considered

| Option | Verdict |
|---|---|
| Ticket as written: `Reload()` first in the tail, failure only logged | Rejected. If `Reload()` fails, the shutdown save later overwrites the restored `app.config` while `Completed` is reported. |
| Move `DeleteBackupSiblingsAsync` after the success publish, no flag | Rejected. It narrows the window but does not close it. Any later throw still deletes the live files. |
| `Reload()` as the last pre-commit action plus commit flag plus guarded tail (chosen) | Every failure before the commit rolls back fully. Every failure after it is cosmetic. |

## 3. Approach: `AppConfigurationManager` temp file (secondary nit)

**Decision: use a single fixed temp filename (`app.config.tmp`) instead of `app.config.tmp-<guid>`. Do not
add a startup sweep.**

- Why not a sweep: the restart flow in `AppConfigurationPageViewModel` calls `Process.Start(...)` and then
  `Shutdown()`. So the new process starts **while the old process is still running its shutdown-time
  `SaveChangesAsync`**. A startup sweep of `app.config.tmp-*` in the new process can delete the old
  process's temp file between `stream` dispose and `SysFile.Move`. The old process's save is then lost.
  A sweep that is safe needs an age threshold, which is more code for less benefit.
- A fixed name cleans itself up: the next save's `FileMode.Create` overwrites any orphan, so at most one
  orphan can ever exist.
- Concurrent writers (unlikely: the new process only saves on its own shutdown or before a backup):
  `FileShare.None` makes the second writer fail with `IOException`, which is logged as today, and
  `app.config` is never partial. **Required guard:** the catch block currently deletes `tempFile` whenever
  it exists. With a fixed name, a writer that failed to *open* the temp file would delete the *other*
  writer's in-flight temp file. Delete the temp file only if this call actually created or opened it (set
  a local `tempCreated = true` right after `tempFile.Open(...)` succeeds).
- No public API change. `APP_CONFIGURATION_FILE` is private, and the temp name is an internal detail.
  #162 has not been released yet (`CHANGELOG.md` `[Unreleased]`), so no `.tmp-<guid>` orphans exist on
  user machines and no one-time sweep of the old pattern is needed.

## 4. Quality attributes

- **Consistency / data integrity:** `Completed` means the restored artifacts are live on disk and loaded in
  memory. `RolledBack` means the originals are live. No outcome leaves the live artifacts missing.
- **Operability:** failures after the commit are logged and never reported as a failed restore.
- **Evolvability:** the `when (!committed)` filters keep a future unguarded call after the commit from
  silently starting a destructive rollback.

Fitness function: the rewritten unit test in §6 (task B2). No architecture test is needed.

## 5. Security notes

There is no new entry point or data flow. Log messages may include file paths (local app-data paths, the
same as the existing warnings). Do not log the contents of `app.config`.

## 6. Tasks (backend-developer-dotnet), in build order

**A. Restore handler**

1. Add `ILogger<PerformApplicationRestoreRequestHandler>` to the constructor (with a null guard), and
   update the test fixture's `CreateSut`.
2. Move `Reload()` (still only when `ConfigPresent`) to right after `RunStepAsync("RestoringArtifacts")`.
   Then set `committed = true`.
3. Move `DeleteBackupSiblingsAsync` and the `Success` publish into a guarded post-commit tail that never
   throws: each step in its own try/catch(Exception) that logs and swallows. Then return `Completed`.
4. Add `when (!committed)` filters to both catch blocks, so `RollbackSwapsAsync` and the RolledBack toasts
   only run before the commit.
5. Update the comments that describe the old ordering.

**B. Handler tests** (`PerformApplicationRestoreRequestHandlerTests.cs`)

1. **Replace** `HandleAsync_WhenSuccessPublishFailsAfterConfigSwap_NeverReloadsAheadOfTheRollback` with
   `HandleAsync_WhenSuccessPublishFailsAfterCommit_KeepsRestoredArtifactsAndReturnsCompleted`.
   - Arrange as today: original `app.config`, a config-only zip, and the `Success` publish throws.
   - Assert: `Completed`. `app.config` exists and contains the restored content. No `app.config.bak-*`
     sibling is left. `Reload()` is called once. No `SnackBarNotificationEvent` failure is published.
   - Mark it `[UnitTest]` to match the existing test (it touches a temp dir like its neighbours).
2. Add `HandleAsync_WhenReloadThrows_RollsBackToOriginalsAndReturnsRolledBack`. Set up
   `AppConfigurationManagerMock.Setup(x => x.Reload()).Throws<IOException>()`. Assert `RolledBack`, and
   that `app.config` holds the original content (the backup still existed, so the rollback was full).
3. Optional, cheap: `HandleAsync_WhenBackupSiblingCleanupWarningPublishFails_StillReturnsCompleted`. Lock
   or otherwise fail a `.bak-*` delete and make the warning publish throw. If it is hard to set up on
   Windows, skip it. Task B1 covers the same guard.

**C. AppConfigurationManager** (`AppConfigurationManager.cs`)

1. Use the fixed temp name `app.config.tmp`.
2. Track `tempCreated` and only delete the temp file in the catch when this call created it.

**D. AppConfigurationManager tests** (`AppConfigurationManagerTests.cs`)

1. Update the two `".tmp-"` assertions (around lines 105 and 135) to check for the fixed `app.config.tmp`.
2. Add `SaveChangesAsync_WhenStaleTempFileExists_OverwritesItAndLeavesNoTempFile`. Pre-create
   `app.config.tmp` with junk. Save. Assert `app.config` has the new content and `app.config.tmp` is gone.
3. Add `SaveChangesAsync_WhenTempFileIsHeldByAnotherWriter_DoesNotDeleteItAndLeavesDestinationIntact`.
   Hold `app.config.tmp` open with `FileShare.None`. Save. Assert the save fails without throwing (it is
   logged), the held temp file still exists, and `app.config` is unchanged.

**E. CHANGELOG** (at the final integration step): add a `### Fixed` entry under `[Unreleased]` for #163.
Describe it as: a failure after the restore committed no longer deletes the restored files or reports a
rollback, and the `app.config` save uses a single fixed temp file.

## 7. Test strategy and risks for QA

| Concern | Level | Test |
|---|---|---|
| A failure after the commit keeps the restored files and returns `Completed` | Unit (temp dir) | B1 |
| A `Reload()` failure rolls back fully, with disk and memory consistent | Unit | B2 |
| No regression in the existing rollback paths (corrupt zip, partial swap, cancel) | Unit/Integration | existing suite, unchanged |
| A stale temp orphan is self-healed | Unit | D2 |
| A failed open never deletes another writer's temp file | Unit | D3 |

Risks to probe:

- A cancellation token firing **during** the tail (no step observes it today) must still give `Completed`.
- Restart flow: after a `Completed` restore with a cleanup warning, confirm the restart and check that the
  relaunched app shows the restored config. The shutdown save must not overwrite it.
- Pre-existing, out of scope, noted for QA: `ValidateConfigurationFile` only checks that the JSON parses.
  A restored `app.config` whose root is not an object (e.g. `[]`) passes validation. `Reload()` then logs
  and loads `{}`, and the shutdown save writes `{}` over it. Worth a separate ticket if it matters.

## 8. Open decisions

None.
