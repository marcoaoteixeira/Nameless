# Application backup / restore (AppConfigurationPage)

- Ticket: none (feature request from user, AppConfigurationPage buttons)
- Stack: WPF (`net10.0-windows`), MVVM (CommunityToolkit.Mvvm) + WPF-UI, project `Nameless.WinApp`,
  shared libs `Nameless.Windows` / `Nameless.Common`
- Scope: wire up the two existing stub commands on `AppConfigurationPageViewModel`:
  `PerformApplicationBackupAsync` and `PerformApplicationRestoreAsync`. No new page, no new nav item.

## 1. What gets backed up

Inside `IApplicationContext.FileManager.Root`:
- `app.config` (file backing `IAppConfigurationManager`)
- `databases/app.db` (SQLite, via `FileManagerExtensions.GetDatabaseDirectory()`)
- `lucene/<indexName>` (Lucene FS index, via `LuceneOptions.DirectoryRelativePath` + `IIndex.Name`)

All three bundled into a single `.zip`, written by default to
`FileManagerExtensions.GetBackupDirectory()` (`{Root}/backups`), named
`backup-{yyyyMMddHHmmss}.zip`. User can redirect the destination via `IFileSystemDialog.OpenSave`.

## 2. Why this isn't a plain recursive file copy

Two subsystems keep files open/locked under normal operation:

- **SQLite**: `AppDbContext` is registered with `AddDbContext` (scoped, not `IDbContextFactory`), so
  whichever DI scope currently holds a context may have an open connection on `app.db`
  (`Pooling=false` only disables ADO.NET pooling, it does not mean "no open handle"). Prior art for
  this exact problem exists in `Nameless` (predecessor repo) at
  `samples/WinApp/Nameless.WinApp/DisasterRecovery/SqliteBackupRecoveryRoutine.cs`: it (a) closes the
  handler's own `AppDbContext` connection (`_dbContext.Database.CloseConnectionAsync()`) for parity —
  **this alone does not guarantee consistency**, other scopes may still hold connections — and (b) uses
  SQLite's **online backup API** — `Microsoft.Data.Sqlite`'s `SqliteConnection.BackupDatabase(destinationConnection)`
  — to copy source → destination through two fresh, short-lived connections. Consistency actually comes
  from `BackupDatabase()` itself: it is designed to work correctly while other connections on the same
  database are open. We reuse that approach instead of a raw file copy. Both connections use
  `Constants.Database.ConnStrPattern` (`Pooling=false`) — if pooling were on, the destination file
  would stay locked after `Dispose()` and both zipping and temp-folder cleanup would fail. Before
  opening the source connection, check the file exists and open with `Mode=ReadOnly`: `SqliteConnection.Open()`
  against a missing `app.db` silently creates an empty database, which would then be backed up as if it
  were real data. Use `await using` for both connections.
- **Lucene**: `IIndexProvider` (DI singleton) caches `IIndex` instances that lazily open an
  `IndexWriter` + `FSDirectory` and hold them until `Dispose()` — there is currently no flush/close/reopen
  method, only `Dispose()` (which is final — the provider can't be reused after). Copying the index
  folder while a writer holds `write.lock` risks a corrupt or stale-locked copy.

## 3. New library API: `IIndexProvider` close/reopen

Add to `libs/common/src/Nameless.Common/Lucene/IIndexProvider.cs`:

```csharp
public interface IIndexProvider : IDisposable {
    IIndex Get(string? indexName);

    /// <summary>
    /// Flushes and closes every currently-open index (commits pending changes —
    /// disposing an IndexWriter in Lucene.NET 4.8 commits, it does not roll back;
    /// call <see cref="IIndex.Rollback"/> first if uncommitted writes must be
    /// discarded — then disposes the underlying IndexWriter/IndexReader/FSDirectory),
    /// without disposing the provider itself. A later <see cref="Get"/> call
    /// transparently reopens the index on demand.
    /// </summary>
    /// <remarks>
    /// Any <see cref="IIndex"/> obtained before this call is disposed by it; using
    /// it afterward throws <see cref="ObjectDisposedException"/> — call
    /// <see cref="Get"/> again for a fresh instance. Do not call this while an
    /// operation (including an in-progress <c>Search</c> enumeration) is using an
    /// index obtained from this provider — <see cref="IIndex"/> is not thread-safe
    /// and this method does not change that. Calling this after <see cref="IDisposable.Dispose"/>
    /// throws <see cref="ObjectDisposedException"/>. Idempotent otherwise.
    /// </remarks>
    void CloseAll();
}
```

`IndexProvider.CloseAll()`: calls the existing disposed-guard first, snapshots cached keys, removes and
disposes each one (reusing the same close loop `Dispose(bool)` uses, rather than duplicating it), and if
one `Index.Dispose()` throws, still closes the rest and aggregates. This is additive and low-risk for
existing callers (`InitializeLuceneIndexStep`, anything calling `Get`).

**Required fix alongside this addition** — `IndexProvider.RemoveFromCache` (`IndexProvider.cs:99-103`)
currently does `_cache.TryRemove(index.Name, out _)`, removing by key only. If `CloseAll` evicts an
index while a concurrent `Get()` caches a *new* instance under the same name before the old instance's
dispose callback runs, that callback evicts the new instance instead — orphaning it (never disposed,
keeps `write.lock`), and the next `Get()` hits `LockObtainFailedException`. Fix: remove only if both key
and value match — `_cache.TryRemove(new KeyValuePair<string, Index>(index.Name, index))`.

Add library tests (`[UnitTest]`/`[IntegrationTest]`, Nameless.Common.Tests, temp-folder `IFileManager`):
`Get` after `CloseAll` returns a different working instance; the old instance throws
`ObjectDisposedException`; `CloseAll` after `Dispose` throws `ObjectDisposedException`; calling
`CloseAll` twice is a no-op; writes committed before `CloseAll` are readable after reopening; the
`RemoveFromCache` race does not evict a new instance cached under the same name.

Used by backup (`CloseAll()` before zipping the `lucene` folder — index reopens itself lazily on next
`Get()`) and by restore (`CloseAll()` before swapping the folder, so the next search reopens against the
restored files — though restore additionally forces an app restart, see §6).

## 4. New progress-notification event

The commented-out code referenced `UseCaseMessage`, a type that doesn't exist in this codebase. Add a
small notification type instead, following the existing `NotificationEvent` / `SnackBarNotificationEvent`
pattern (`libs/windows/src/Nameless.Windows/Notification/`):

```csharp
namespace Nameless.Windows.Notification.BackupRecovery;

public sealed class BackupRecoveryProgressNotificationEvent : NotificationEvent;
```

`ITaskRunnerBuilder.SubscribeFor<BackupRecoveryProgressNotificationEvent>()` wires it into the
`TaskRunnerWindow`'s log list, same as any other `NotificationEvent`. Handlers publish one of these
(via `IEventPublisher.PublishInformationAsync<BackupRecoveryProgressNotificationEvent>(message)` etc.) at
each major step: closing DB connection, backing up database, zipping index, writing zip / extracting zip,
done. Final success/failure also raised as `SnackBarNotificationEvent` after the TaskRunner window closes
(matches `OpenCurrentLogFileRequestHandler`'s convention for user-facing toasts).

## 5. Backup feature (`Features/PerformApplicationBackup/`)

- `PerformApplicationBackupRequest : IRequest<string>` — no parameters for this pass
  (`public sealed class PerformApplicationBackupRequest : IRequest<string>;`). A destination-path
  override is purely additive, deferred until a save-dialog flow is actually needed.
- `PerformApplicationBackupRequestHandler : IRequestHandler<PerformApplicationBackupRequest, string>`
  Constructor deps: `IApplicationContext`, `IAppConfigurationManager`, `AppDbContext`, `IIndexProvider`,
  `IOptions<LuceneOptions>`, `IEventPublisher`, `ITranslator`. Guard all with `Throws.When.Null`
  (matches `OpenCurrentLogFileRequestHandler` convention). Resource keys use the full
  `{Class}_{Method}_{Suffix}` form (the existing handler's literal `"Notify_Missing_Log_File_Title"`
  looks like a lookup bug against its own resx key `OpenCurrentLogFileRequestHandler_HandleAsync_Notify_Missing_Log_File_Title`
  — don't copy that mistake).

  Steps (try/catch per step publishing a step-specific failure progress message, rethrow to the
  handler's outer boundary so §8's view-model wrapper can report the final outcome; one outer `finally`
  deletes the per-run temp folder regardless of outcome or cancellation):
  1. Publish "starting backup" progress event.
  2. `await SaveChangesAsync(cancellationToken)` on `IAppConfigurationManager` — settings changed this
     session (theme/language/confirm-before-exit) only hit disk at app exit otherwise; back up stale or
     possibly-absent data without this.
  3. `await _dbContext.Database.CloseConnectionAsync(cancellationToken)` (parity/best-effort — see §2 note
     on why actual consistency comes from the SQLite online-backup API, not this call).
  4. Build a **per-run** temp working directory: `FileManagerExtensions.GetTemporaryDirectory(timestamp)`
     — never the parameterless overload, which returns the shared `{Root}/temporary` folder also used by
     `XpsDocumentConverter`; deleting that would be a bug, not a cleanup.
  5. Copy `app.config` into the temp dir if it exists; if it still doesn't exist (e.g. fresh install),
     log a warning and skip it rather than failing the whole backup.
  6. SQLite backup: check the source `databases/app.db` file exists first (see §2); open fresh
     `SqliteConnection`s (`await using`) for source (`Mode=ReadOnly`) and a destination file inside the
     temp dir, call `source.BackupDatabase(destination)` under `Task.Run` (synchronous API, keep the
     TaskRunner window responsive), honoring `cancellationToken.ThrowIfCancellationRequested()` around it.
  7. `_indexProvider.CloseAll()`, then recursively copy the Lucene directory (resolved from
     `IOptions<LuceneOptions>.Value.DirectoryRelativePath` via the file manager — a missing folder means
     "nothing to back up yet", not an error) into the temp dir, skipping any `write.lock` file. No
     existing recursive-copy helper in the repo — write one private to this handler.
  8. `await System.IO.Compression.ZipFile.CreateFromDirectoryAsync(tempDir, destinationFilePath, includeBaseDirectory: false, cancellationToken)`
     (.NET 10 async overload, so the token is actually honored). `CreateFromDirectory*` throws if the
     destination already exists; on any failure/cancellation after this point, delete a partially
     written destination zip.
  9. Publish success progress event + return the destination file path.

  No existing zip/archive abstraction exists anywhere in the repo (confirmed by search) — this handler
  uses `System.IO.Compression.ZipFile`/`ZipArchive` directly; no new library abstraction (revisit only if
  the restore handler shares >~10 lines with this one — even then, a private helper in the sample app
  first, not a new `Nameless.Common` public type).

## 6. Restore feature (`Features/PerformApplicationRestore/`)

- `PerformApplicationRestoreRequest(string SourceZipFilePath) : IRequest<RestoreOutcome>` where
  `RestoreOutcome` is a small enum (`Completed`, `RolledBack`) — mirrors `TaskRunnerOutcome`'s shape for
  consistency, kept local to the feature folder (not a shared type) since nothing else needs it yet.
- `PerformApplicationRestoreRequestHandler`. Same deps as backup, plus `IFileManager` for path
  resolution. Steps:
  1. Publish "starting restore" progress event.
  2. `await _dbContext.Database.CloseConnectionAsync(cancellationToken)`.
  3. `_indexProvider.CloseAll()`.
  4. Extract the zip (`ZipFile.ExtractToDirectory`) into a temp dir, then **validate what it actually
     contains** before touching any live file: at least one of `app.config`, `databases/app.db`,
     `lucene/` must be present; `app.config` must parse as JSON; `app.db` must pass SQLite's
     `PRAGMA quick_check`. A validation failure aborts before anything is renamed — there is nothing to
     roll back yet.
     - Only the entries the (now validated) archive actually contains are swapped into place — each one
       individually renamed to a `*.bak-{timestamp}` sibling (rollback point) and replaced by the
       extracted copy. This is a rename within the same parent directory, not a copy — cheap and
       atomic-ish on the same volume.
     - Trade-off this implies: an older config-only backup restored next to a newer live database leaves
       the database as-is. That is intentional — a backup missing an artifact (because it predates that
       artifact's existence, or was deliberately scoped) must not wipe out the live one — not a bug.
  5. On any failure in step 4 (validation, or a swap itself): move the `*.bak-*` siblings already put in
     place back to their original names/locations (best-effort — log if even that fails, user is told to
     check the backups folder manually), publish failure progress + `SnackBarNotificationEvent`, return
     `RestoreOutcome.RolledBack`.
  6. On success: delete the `*.bak-*` siblings, publish success progress event, reload the in-memory
     app configuration (if `app.config` was one of the swapped entries) so a later shutdown-time save
     doesn't clobber it with stale pre-restore values, and return `RestoreOutcome.Completed`.

## 7. Restart requirement

Per user decision: **restore always requires an app restart** to be usable again (EF Core's `AppDbContext`
and the Lucene `IIndexProvider` are not designed to be fully torn down and recreated against swapped files
while the app keeps running — `CloseAll()`/`CloseConnectionAsync()` only guarantee the *old* handles are
released, not that a *new* read against the restored files is safe without reinitializing DI-scoped
state). After a successful restore:
- Show a message dialog (`IMessageDialog`, already used elsewhere e.g. `MainWindow`) telling the user the
  app must restart to use the restored data.
- On confirmation, relaunch the process (`Process.Start` with the current executable path,
  `Environment.ProcessPath`) and call `WindowsApplication.Current.Shutdown()`.
- A rolled-back restore (failure path) does **not** require a restart — nothing was actually swapped in a
  way that matters, since EF/Lucene state still points at the original (renamed-back) files.

Backup does **not** require a restart — nothing in the live app's file handles changes shape afterward
(`CloseConnectionAsync` just drops the pooled handle, the next EF Core use reopens it transparently; the
Lucene index reopens lazily on next `Get()` after `CloseAll()`).

## 8. ViewModel wiring (`AppConfigurationPageViewModel.cs`)

Replace the two stub `[RelayCommand]`s:

`TaskRunnerBuilder.RunAsync()` re-throws the delegate's exception (`Completion` faults — see
`TaskRunnerWindowViewModel.cs:56-61`); it does not swallow it, and it has no way to tell the handler
"the window just closed" so the handler cannot own the final snackbar itself. The view model must catch
the exception and capture the handler's result from inside the delegate:

```csharp
[RelayCommand]
private async Task PerformApplicationBackupAsync(CancellationToken cancellationToken) {
    string? backupFilePath = null;

    try {
        var outcome = await _taskRunner.CreateBuilder()
                                        .SetName(T["AppConfigurationPageViewModel_PerformApplicationBackup_TaskRunnerWindow_Title"])
                                        .SubscribeFor<BackupRecoveryProgressNotificationEvent>()
                                        .SetDelegate(async ct => backupFilePath = await _mediator.ExecuteAsync(new PerformApplicationBackupRequest(), ct))
                                        .RunAsync();

        if (outcome is TaskRunnerOutcome.Completed && backupFilePath is not null) {
            await _eventPublisher.PublishSuccessAsync<SnackBarNotificationEvent>(
                message: string.Format(T["AppConfigurationPageViewModel_PerformApplicationBackup_Success"], backupFilePath)
            );
        }
        // Cancelled: nothing to report, the handler already published a cancellation-aware progress entry.
    } catch (Exception ex) {
        await _eventPublisher.PublishFailureAsync<SnackBarNotificationEvent>(
            message: string.Format(T["AppConfigurationPageViewModel_PerformApplicationBackup_Failure"], ex.Message)
        );
    }
}

[RelayCommand]
private async Task PerformApplicationRestoreAsync(CancellationToken cancellationToken) {
    var filePath = _fileSystemDialog.OpenFile(o => {
        o.Title = T["AppConfigurationPageViewModel_PerformApplicationRestore_SelectFile_Title"];
        o.Filter = "Zip files (*.zip)|*.zip";
    }).FirstOrDefault();

    if (filePath is null) { return; }

    await _taskRunner.CreateBuilder()
                      .SetName(T["AppConfigurationPageViewModel_PerformApplicationRestore_TaskRunnerWindow_Title"])
                      .SubscribeFor<BackupRecoveryProgressNotificationEvent>()
                      .SetDelegate(ct => _mediator.ExecuteAsync(new PerformApplicationRestoreRequest(filePath), ct))
                      .RunAsync();
}
```

Remove the dead `ExecuteApplicationBackupAsync`/`ExecuteSystemUpdateAsync` stub methods (unrelated to
this feature — `ExecuteSystemUpdateAsync` throws `NotImplementedException` and belongs to the
not-in-scope "system update" button). `PerformSystemUpdateAsync` stays untouched/commented — out of
scope for this work.

No XAML changes needed — `PerformApplicationBackupCommand` / `PerformApplicationRestoreCommand` bindings
already exist on the existing cards (`AppConfigurationPage.xaml` lines ~62-104).

## 9. New localization keys

Added to `TranslationResources.resx` + `TranslationResources.pt-BR.resx`:
- `AppConfigurationPageViewModel_PerformApplicationBackup_TaskRunnerWindow_Title`
- `AppConfigurationPageViewModel_PerformApplicationRestore_TaskRunnerWindow_Title`
- `AppConfigurationPageViewModel_PerformApplicationRestore_SelectFile_Title`
- `AppConfigurationPageViewModel_PerformApplicationRestore_ConfirmRestart_Title` / `_Message`
- Backup/restore progress + success/failure message keys (one set per step, named
  `PerformApplicationBackupRequestHandler_<Step>_{Starting|Success|Failure}` /
  `PerformApplicationRestoreRequestHandler_<Step>_{Starting|Success|Failure}`, matching the
  `{CLASS}_{ActionName}_{Suffix}` convention from `SqliteBackupRecoveryRoutine`).

## 10. New `Constants` entries

```csharp
internal static class BackupRecovery {
    internal const string FileNamePattern = "backup-{0:yyyyMMddHHmmss}.zip";
    internal const string AppConfigFileName = "app.config";
}
```

(`Database.FileName` / `FileManagerExtensions.GetDatabaseDirectory()` / Lucene's own
`LuceneOptions.DirectoryRelativePath` already cover the other two paths — no duplication needed.)

## 11. Out of scope / explicitly deferred

- `PerformSystemUpdateAsync` — separate feature, not touched.
- Hot-swap restore without restart — rejected per user decision (§7).
- A shared zip/archive abstraction in `Nameless.Common` — single call site today, revisit if a second
  consumer appears.
- Scope-tracking / forcing-closed of *every possible* `AppDbContext` instance across the app — relying on
  `CloseConnectionAsync` on the handler's own injected context, same as the `Nameless` predecessor's
  routine; no new DI-scope-enumeration infra.

## 12. Testing

- `[UnitTest]` for the restore rollback path (simulate a bad zip → assert original files renamed back,
  `RestoreOutcome.RolledBack`).
- `[UnitTest]` for backup handler happy path against a fake `IFileManager`/temp directory.
- `[IntegrationTest]` round-trip: backup a real small SQLite db + fake index folder, restore it, assert
  file contents match (skip Lucene's actual `IndexWriter` in this test — just verify files round-trip).
