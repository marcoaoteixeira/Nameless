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
  `AppDbContext`'s connection (`_dbContext.Database.CloseConnectionAsync()`), then (b) uses SQLite's
  **online backup API** — `Microsoft.Data.Sqlite`'s `SqliteConnection.BackupDatabase(destinationConnection)`
  — to copy source → destination through two fresh, short-lived connections. We reuse that approach
  instead of a raw file copy: it is the supported way to copy a SQLite file that might be in use, and it
  produces a consistent snapshot even mid-write.
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
    /// Flushes and closes every currently-open index (commits pending changes,
    /// disposes the underlying IndexWriter/IndexReader/FSDirectory), without
    /// disposing the provider itself. A later <see cref="Get"/> call transparently
    /// reopens the index on demand.
    /// </summary>
    void CloseAll();
}
```

`IndexProvider.CloseAll()` disposes each cached `Index` (same effect `Dispose()` has on them today) and
clears the cache dictionary, but leaves the provider itself usable — `Get()` afterward just lazily
recreates an `Index` the same way it does today for a name not yet cached. This is additive and
low-risk: existing callers (`InitializeLuceneIndexStep`, anything calling `Get`) are unaffected.

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

- `PerformApplicationBackupRequest : IRequest<string>` — carries optional `string? DestinationFilePath`
  (null → default backups folder + timestamped name, computed by the handler).
- `PerformApplicationBackupRequestHandler : IRequestHandler<PerformApplicationBackupRequest, string>`
  Constructor deps: `IApplicationContext`, `AppDbContext`, `IIndexProvider`, `IEventPublisher`, `ITranslator`.
  Steps (each wrapped try/catch, each failure short-circuits and reports failure — mirrors
  `SqliteBackupRecoveryRoutine`'s per-step `Result` pattern, simplified to exceptions since this
  codebase's `IRequestHandler` contract doesn't use a `Result<T>` type):
  1. Publish "starting backup" progress event.
  2. `await _dbContext.Database.CloseConnectionAsync(cancellationToken)`.
  3. Build a temp working directory (`FileManagerExtensions.GetTemporaryDirectory()`).
  4. Copy `app.config` into the temp dir as-is (plain file copy — it's not held open by anything).
  5. SQLite backup: open fresh `SqliteConnection`s for source (`databases/app.db`) and a destination
     file inside the temp dir (`databases/app.db`), call `source.BackupDatabase(destination)`, then
     close/dispose both connections (exact shape of `SqliteBackupRecoveryRoutine.ExecuteDatabaseBackupAsync`).
  6. `_indexProvider.CloseAll()`, then recursively copy the `lucene` directory into the temp dir
     (`IDirectory.GetFiles()` recursive helper already available, or `Directory.CreateDirectory` +
     `File.Copy` per file — no existing recursive-directory-copy helper in the repo, write a small
     private helper local to the handler).
  7. `System.IO.Compression.ZipFile.CreateFromDirectory(tempDir, destinationFilePath)`.
  8. Delete the temp working directory.
  9. Publish success progress event + return the destination file path.

  No existing zip/archive abstraction exists anywhere in the repo (confirmed by search) — this handler
  uses `System.IO.Compression.ZipFile`/`ZipArchive` directly; no new library abstraction is introduced
  for it since it's a single call site today.

## 6. Restore feature (`Features/PerformApplicationRestore/`)

- `PerformApplicationRestoreRequest(string SourceZipFilePath) : IRequest<RestoreOutcome>` where
  `RestoreOutcome` is a small enum (`Completed`, `RolledBack`) — mirrors `TaskRunnerOutcome`'s shape for
  consistency, kept local to the feature folder (not a shared type) since nothing else needs it yet.
- `PerformApplicationRestoreRequestHandler`. Same deps as backup, plus `IFileManager` for path
  resolution. Steps:
  1. Publish "starting restore" progress event.
  2. `await _dbContext.Database.CloseConnectionAsync(cancellationToken)`.
  3. `_indexProvider.CloseAll()`.
  4. Rename current `app.config`, `databases`, `lucene` to `*.bak-{timestamp}` siblings (rollback point).
     This is a rename within the same parent directory, not a copy — cheap and atomic-ish on the same
     volume.
  5. Extract the zip (`ZipFile.ExtractToDirectory`) into a temp dir, then move the three expected
     entries (`app.config`, `databases/`, `lucene/`) into place at the root.
     - If the zip doesn't contain the expected structure (e.g. wrong/garbage file), treat as failure →
       go to rollback step below before returning.
  6. On any failure in step 4/5: move the `*.bak-*` siblings back to their original names/locations
     (best-effort — log if even that fails, user is told to check the backups folder manually), publish
     failure progress + `SnackBarNotificationEvent`, return `RestoreOutcome.RolledBack`.
  7. On success: delete the `*.bak-*` siblings, publish success progress event, return
     `RestoreOutcome.Completed`.

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

```csharp
[RelayCommand]
private Task PerformApplicationBackupAsync(CancellationToken cancellationToken) {
    return _taskRunner.CreateBuilder()
                       .SetName(T["AppConfigurationPageViewModel_PerformApplicationBackup_TaskRunnerWindow_Title"])
                       .SubscribeFor<BackupRecoveryProgressNotificationEvent>()
                       .SetDelegate(ct => _mediator.ExecuteAsync(new PerformApplicationBackupRequest(), ct))
                       .RunAsync();
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
