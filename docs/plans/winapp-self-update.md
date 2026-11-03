# Nameless.WinApp in-app self-update (AppConfigurationPage "Version" card)

- Ticket: none (feature from the self-update brainstorm). No GitHub issue was given. Create one before
  starting work so the project board can track it (CLAUDE.md workflow).
- Stack: .NET 10 / WPF (`net10.0-windows`, `win-x64`). Sample app `Nameless.WinApp`. There are no library
  changes. GitHub Actions is used for the release workflow.
- Agent pair: `backend-developer-dotnet` / `backend-reviewer-dotnet`. No frontend side (same as #162 and #163).
- Work split: BACKEND-ONLY.
- UX spec: not needed. The button already exists, and every state reuses a pattern this page already has:
  a `MessageDialog` question, the `TaskRunner` progress window and `SnackBar` toasts. The texts are in §3.7.
- Contract: [`docs/specs/winapp-self-update-release-contract.md`](../specs/winapp-self-update-release-contract.md).
  It is binding for the release workflow and for the app.
- ADR: [`docs/adr/ADR-001-Self-Update-WinApp-In-Process-By-Renaming-Running-Files-Aside.md`](../adr/ADR-001-Self-Update-WinApp-In-Process-By-Renaming-Running-Files-Aside.md)
  (Integration: new external API, GitHub Releases).

## 1. Context

Settled in the brainstorm, not revisited here: the update runs in-process, uses the rename trick, gets
releases from GitHub Releases, and does not reuse `PerformApplicationRestoreRequestHandler`. This plan
borrows that handler's *shape*: TaskRunner progress events, a commit point, rollback on failure and
SnackBar outcomes.

What the code does today:

- `AppConfigurationPageViewModel.PerformSystemUpdateAsync` is a stub. The button in `AppConfigurationPage.xaml`
  (Version card) is already bound to `PerformSystemUpdateCommand`.
- `IApplicationContext.Version` reads the `ApplicationContext:Version` configuration key, which **nothing
  sets**. So every build shows `v1.0.0` (the `SemVersion.V1` fallback). `AssemblyVersion` is pinned to
  `1.0.0` for every project in `Directory.Build.props`. Neither can be compared against a release today.
  Fixed in §3.1.
- App data (`app.config`, `databases/`, `backups/`, `temporary/`, `updates/`) lives under
  `IApplicationContext.FileManager.Root`. With the default `ApplicationDataLocation.Machine` that is
  `C:\ProgramData\Windows Application`, outside the install directory. `FileManagerExtensions.GetUpdateDirectory()`
  (`{Root}/updates`) already exists and `EnsureApplicationDirectoriesExistenceStep` already creates it.
  Nothing uses it yet.
- Serilog writes to `logs/app-.log` relative to the working directory. `OpenCurrentLogFileRequestHandler`
  reads `logs/` under `AppContext.BaseDirectory`. So in a normal launch **the log file is open inside the
  install directory** for the whole process lifetime.
- `win-x64.pubxml` (project root, not `Properties/PublishProfiles`) describes a self-contained single-file
  publish. The repository has **no** `.github/` folder, so the release workflow is new work.
- Existing repo tags are `v2.2.6` … `v3.7.4` (library releases). That is why the WinApp uses `winapp-v*`.

### 1.1 Deviation from the brief: swap files, not the directory (verified)

The brief describes renaming the whole install **directory** to a `.old-{timestamp}` sibling. I tested
this on this machine (Windows 11) before designing around it:

| Experiment | Result |
|---|---|
| Rename the directory that holds a running exe, with the process CWD outside it | OK |
| Same, with the process CWD **inside** it (a double-click launch from Explorer) | **Fails** (sharing violation) |
| Rename a directory that holds a file open without `FILE_SHARE_DELETE` (like the Serilog log) | **Fails** (access denied) |
| Rename the **running exe file** in place (`ping.exe` and a .NET 10 single-file self-contained bundle) | OK |
| Put a new file at the running exe's original path, after the rename | OK |
| Running single-file process after its exe was renamed and replaced: keeps running, lazily loads new types | OK |
| Delete the renamed `.old` exe while its process runs / after it exits | Fails / OK |

The Serilog log file alone makes the directory rename fail on every normal run. A directory rename would
also need write access to the install directory's **parent**. So the design keeps Option A (in-process,
rename the running exe aside, relaunch) but applies it **per top-level entry** inside the install
directory. Each entry that the package replaces is renamed to `{name}.old-{yyyyMMddHHmmss}` next to itself,
and the new entry is moved into its place. `logs/` and `App_Data/` are never touched. This is a detail
inside the chosen option, not a different option. The ADR records it.

## 2. Approach overview

```
[Version card button]
   -> CheckSystemUpdateRequest           (GitHub list releases -> pick highest eligible winapp-v*)
   -> UpToDate? Information SnackBar, stop
   -> MessageDialog "Update vX -> vY? The app will restart."  (No: stop)
   -> TaskRunner window
        PerformSystemUpdateRequest       (prereqs -> download -> verify -> extract -> validate
                                          -> close DB/Lucene -> swap entries -> verify -> COMMIT -> tidy)
   -> Completed? Process.Start(new exe) + Application.Current.Shutdown()
   -> RolledBack? handler already published progress + SnackBar, stop

[Next startup, last bootstrap step]
   CleanupPreviousSystemUpdateStep: delete {install}\*.old-{14 digits}, {install}\.update-{14 digits}, {Root}\updates\*
```

## 3. Design

### 3.1 Version source (fixes `v1.0.0` everywhere)

- `Nameless.WinApp.csproj`:
  - `<IncludeSourceRevisionInInformationalVersion>false</IncludeSourceRevisionInInformationalVersion>`,
    so the informational version is `1.4.0` and not `1.4.0+<sha>`.
  - A conditional assembly attribute, emitted only when `$(SelfUpdateEnabled) == 'true'`:
    `AssemblyMetadata("Nameless.WinApp.SelfUpdate", "Enabled")` (an `<AssemblyAttribute>` item in an
    `ItemGroup` with that condition).
- `App.xaml.cs`: add `$"--ApplicationContext:Version={informationalVersion}"` to the `args` passed to
  `WinHostFactory.CreateDefault`. `Host.CreateApplicationBuilder(args)` maps command-line args to
  configuration, the same way `--applicationName` already works. Read the value from
  `typeof(App).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()`. Fallback: the
  `Version` property of the same attribute set, or omit the arg, which keeps today's `v1.0.0`.
- Result: the Version card shows the real version, and the update compares against the **same** value the
  user sees. Mapping table: contract §3.

### 3.2 GitHub client (`Infrastructure/GitHub/`)

- `IGitHubReleaseClient` / `GitHubReleaseClient`, a typed `HttpClient`:
  - `Task<IReadOnlyList<GitHubRelease>> ListReleasesAsync(CancellationToken)`: contract §4.1.
  - `Task<string> DownloadTextAsync(Uri url, long maxBytes, CancellationToken)`: for the sidecar, capped
    at 1 KiB.
  - `Task DownloadToFileAsync(Uri url, string destinationPath, CancellationToken)`: streamed with
    `HttpCompletionOption.ResponseHeadersRead` and `CopyToAsync(token)`. The stream is never buffered.
  - Maps rate-limit and non-2xx responses to a dedicated `GitHubReleaseClientException` (with an `IsRateLimited`
    flag and a `RateLimitResetsAt`). The handlers translate it into the SnackBar texts.
- DTOs `GitHubRelease` / `GitHubReleaseAsset`: `sealed record`s with `[JsonPropertyName]` for the snake_case
  fields in contract §4.1 only. Default `System.Text.Json` options ignore unknown members.
- Registration: a new `Infrastructure/SystemUpdateHostFeature.cs` (a `HostFeature`, like
  `CustomFileManagerHostFeature`), added in `ApplicationHostFeature.Configure`. It calls
  `services.AddHttpClient<IGitHubReleaseClient, GitHubReleaseClient>(...)` with
  `BaseAddress = https://api.github.com/`, `Timeout = 15 s`, and default headers `Accept`,
  `X-GitHub-Api-Version` and `User-Agent: Nameless.WinApp/{IApplicationContext.Version}`. It also registers
  `ISystemUpdateEnvironment` (§3.4).
  - Downloads use `ResponseHeadersRead`, so the 15 s `Timeout` only bounds the time to the response headers.
    The body copy is bounded by the TaskRunner's cancel button. Confirm this during implementation. If the
    timeout turns out to cover the body, use a second named client with `Timeout.InfiniteTimeSpan`.
- **No new package.** `Microsoft.Extensions.Http` already comes in through `Nameless.Common`, and
  `System.Text.Json` / `System.Security.Cryptography` / `System.IO.Compression` are in the BCL. If the
  developer finds the transitive reference does not flow into `Nameless.WinApp`, add
  `<PackageReference Include="Microsoft.Extensions.Http" />` with no version. The version is already in
  `Directory.Packages.props`.
- Constants: a new `Constants.SystemUpdate` with `RepositoryOwner = "marcoaoteixeira"`,
  `RepositoryName = "Nameless"`, `TagPrefix = "winapp-"`, `RuntimeIdentifier = "win-x64"`,
  `PackageNamePattern = "Nameless.WinApp-{0}-win-x64.zip"`, `ChecksumSuffix = ".sha256"`,
  `ExecutableName = "Nameless.WinApp.exe"`, `MaxPackageBytes = 512 MiB`, `OldSuffixPattern = "{0}.old-{1:yyyyMMddHHmmss}"`
  and `StagingDirectoryPattern = ".update-{0:yyyyMMddHHmmss}"`. Constants and not options, matching the
  existing `Constants.BackupRecovery` style. Tests go through a fake `HttpMessageHandler`, not a different
  repository.

### 3.3 Feature `Features/CheckSystemUpdate/`

- `CheckSystemUpdateRequest : IRequest<SystemUpdateCheckResult>` (sealed class, no payload, like
  `PerformApplicationBackupRequest`).
- `SystemUpdateCheckResult(SemVersion Current, AvailableRelease? Release)` with `IsUpdateAvailable => Release is not null`.
- `AvailableRelease(SemVersion Version, string TagName, Uri HtmlUrl, ReleaseAsset Package, ReleaseAsset Checksum)`
  and `ReleaseAsset(string Name, long Size, Uri DownloadUrl)`.
- `CheckSystemUpdateRequestHandler`:
  1. `current = SemVersion.Parse(IApplicationContext.Version)`.
  2. `ListReleasesAsync`. Keep releases where `!draft && !prerelease`, the tag starts with `TagPrefix`
     (ordinal), and `SemVersion.TryParse(tag[TagPrefix.Length..])` succeeds.
  3. Pick the maximum by `SemVersion` (not by list order or date). If there is none, or `max <= current`,
     return `Release = null`.
  4. For that release only, find exactly one asset named `PackageNamePattern` (with `{VERSION}` = the
     version formatted **without** its prefix) and exactly one named `+ ChecksumSuffix`. Check
     `Package.Size` against 1 byte .. `MaxPackageBytes`, and both download URLs against contract §4.1. Any
     failure throws `InvalidDataException("release {tag} is malformed: …")`. Do not fall back to an older
     release, because a malformed newest release is a publishing bug to surface, not to hide.
- This handler does no file I/O and publishes nothing. The view model owns the user-facing outcome.

### 3.4 Feature `Features/PerformSystemUpdate/`

- `PerformSystemUpdateRequest(AvailableRelease Release) : IRequest<SystemUpdateOutcome>` (sealed record).
- `SystemUpdateOutcome { Completed, RolledBack }`. Same semantics as `RestoreOutcome`, with XML docs.
  `Completed` means the new files are in place and a relaunch is required.
- `SystemUpdateProgressNotificationEvent : NotificationEvent`, declared in the **feature folder** (sample
  only). It is not added to `Nameless.Windows`, because no library consumer needs it.
- `ISystemUpdateEnvironment` / `SystemUpdateEnvironment` (singleton). This is the test seam for
  process-level facts:
  - `string InstallDirectoryPath` = `AppContext.BaseDirectory` (normalized, no trailing separator).
  - `string ExecutablePath` = `Environment.ProcessPath!`.
  - `bool IsSelfUpdateEnabled`: true iff the entry assembly has the `AssemblyMetadata` marker from §3.1.
- `PerformSystemUpdateRequestHandler`. Dependencies: `IGitHubReleaseClient`, `ISystemUpdateEnvironment`,
  `IFileManager`, `AppDbContext`, `IIndexProvider`, `IEventPublisher`, `ITranslator`, `ILogger<>`.
  Progress keys are `PerformSystemUpdateRequestHandler_HandleAsync_{Step}` and
  `…_{Step}_Failure` (`RunStepAsync`/`PublishProgressAsync` exactly as in the restore handler).

  `timestamp = DateTimeOffset.UtcNow`. `stagingPath = {Install}\.update-{ts}`.
  `packagePath = {Root}\updates\{Package.Name}`. Both are resolved up front so `finally` can always clean
  them up.

  | # | Step key | Does | Failure means |
  |---|---|---|---|
  | 0 | `Starting` | | |
  | 1 | `CheckingPrerequisites` | `IsSelfUpdateEnabled`, otherwise throw ("only available in release builds"). `ExecutablePath`'s directory equals `InstallDirectoryPath` and its file name equals `ExecutableName`. Write probe: create and delete `{Install}\.update-probe-{ts}`. On `UnauthorizedAccessException`, give a clear message: "install folder is not writable (for example under Program Files). Move the app or reinstall it in a user-writable folder". **Never self-elevate.** | Nothing touched |
  | 2 | `DownloadingChecksum` | `DownloadTextAsync(Checksum.DownloadUrl, 1 KiB)`, parsed per contract §2.2 | Nothing touched |
  | 3 | `DownloadingPackage` | `DownloadToFileAsync` to `packagePath` (overwrite) | Partial file removed in `finally` |
  | 4 | `VerifyingPackage` | File length == `Package.Size`, then streamed SHA-256 == sidecar hash | Same |
  | 5 | `ExtractingPackage` | `ZipFile.ExtractToDirectoryAsync(packagePath, stagingPath, overwriteFiles: false, ct)`. Staging is **inside** the install directory so every later move is a same-volume rename. ProgramData and the install directory can be on different volumes, and `Directory.Move` cannot cross volumes. | Staging removed in `finally` |
  | 6 | `ValidatingPackage` | `ExecutableName` exists at the staging root. No forbidden root entry (contract §2.1). `FileVersionInfo.GetVersionInfo(stagedExe).ProductVersion` parses to `Release.Version` | Same |
  | 7 | `ClosingDatabaseConnection` / `ClosingLuceneIndex` | Same as restore: `CloseConnectionAsync()`, `IIndexProvider.CloseAll()`. This frees Lucene's `write.lock` before the new process starts. Otherwise the relaunched process races the exiting one for it. Both reopen lazily if the run rolls back. | Nothing swapped yet |
  | 8 | `SwappingFiles` | For each top-level staging entry, **`ExecutableName` last**: if `{Install}\{name}` exists, rename it to `{name}.old-{ts}`, then move the staged entry to `{Install}\{name}`. Record `EntrySwap(OriginalPath, BackupPath?, IsDirectory)`. On a move failure, self-heal that entry (move the backup back) and rethrow, exactly as `SwapFileArtifactAsync` does. Install-dir entries the package does not contain (`logs/`, `App_Data/`, user files) are not touched. | Rollback |
  | 9 | `VerifyingInstallation` | `ExecutablePath` exists and its `ProductVersion` parses to `Release.Version`. **Last failable pre-commit action.** | Rollback |
  | | **commit** | `committed = true` | |
  | 10 | (tail, guarded) | Publish `Success`. Each part has its own try/catch plus a logger call. Same rules as `CompleteCommittedRestoreAsync`: never throws, never rolls back. | Logged only |
  | | `finally` | Best-effort delete of `stagingPath` (recursive) and `packagePath` | |

  - The `catch (OperationCanceledException) when (!committed)` and `catch (Exception) when (!committed)`
    blocks are copied from restore. They call `RollbackSwapsAsync` (back-to-front: delete the placed
    entry, then move `BackupPath` back; per-entry failures publish `RollingBack_Failure` and continue),
    then publish the `RolledBack_Cancelled` / `RolledBack` SnackBar and return `RolledBack`. Moving the
    renamed running exe back to its original name is a rename of a running image, which the experiments
    in §1.1 showed works.
  - `.old-*` entries are **not** deleted in the tail. The renamed exe cannot be deleted while this process
    runs. The others are the only manual-recovery path until the new version proves it can start (§3.6).
  - Cancellation is observed through step 7 (download, hash and extract all take `ct`). Steps 8 and 9 do
    not observe it: the swap is a handful of renames, and a cancel after it would only trigger a rollback
    of work that already succeeded. A cancel that lands before step 8 rolls back nothing, because there
    are no swaps yet.
  - `EntrySwap` is a private record in this handler. Do **not** extract a shared swap helper from the
    restore handler in this ticket. The two have different targets (data root vs install directory),
    different failure tails and separate tests. Extract a helper if a third consumer appears.

### 3.5 View model (`AppConfigurationPageViewModel.PerformSystemUpdateAsync`)

Inject `ISystemUpdateEnvironment` (for the relaunch path) and keep the method's existing name and binding.

1. `var check = await _mediator.ExecuteAsync(new CheckSystemUpdateRequest(), ct)`. No TaskRunner window is
   needed: the call is one request with a 15 s cap, and the async `RelayCommand` already disables the
   button while it runs.
2. If `!check.IsUpdateAvailable`: Information SnackBar `…_UpToDate` with `{current}`. Return.
3. `_messageDialog.ShowQuestion(title: …_Confirm_Title, message: …_Confirm_Message(current, new, htmlUrl))`.
   Anything other than `Yes` returns.
4. TaskRunner: `.SetName(…_TaskRunnerWindow_Title).SubscribeFor<SystemUpdateProgressNotificationEvent>()`,
   with a delegate that runs `PerformSystemUpdateRequest(check.Release)`.
5. `outcome is not Completed` means return, because the handler already reported it.
6. `Completed` means relaunch at once. There is no second prompt: the user already agreed to a restart in
   step 3, and the running process is now an orphan of its install. Use
   `using (Process.Start(new ProcessStartInfo(env.ExecutablePath) { UseShellExecute = false, WorkingDirectory = env.InstallDirectoryPath })) { }`,
   then `System.Windows.Application.Current.Shutdown()`.
7. If `Process.Start` throws **after** `Completed`, the update is committed. Do not roll back and do not
   shut down. Show the Failure SnackBar `…_RelaunchFailed` ("Update installed. Close and reopen the
   application."). The old process keeps working from its in-memory image.
8. The outer `catch (Exception ex)` (check failures, `GitHubReleaseClientException`, malformed release)
   shows the Failure SnackBar `…_Failure` with `ex.Message`. A rate-limit exception shows `…_RateLimited`
   with the reset time.

### 3.6 Startup cleanup (`Bootstrap/Steps/CleanupPreviousSystemUpdateStep.cs`)

- Runs **last**. Its `Dependencies` list every other step (`SetUILanguageStep`,
  `EnsureApplicationDirectoriesExistenceStep`, `InitializeLuceneIndexStep`), so a successful run is the
  new version's proof of life. Only then do the rollback leftovers go away.
- Deletes, best-effort, only **direct children** of `AppContext.BaseDirectory` whose names match
  `^.+\.old-\d{14}$` (file or directory) or `^\.update-(probe-)?\d{14}$`. It also deletes the files in
  `{Root}/updates`. It catches `IOException` / `UnauthorizedAccessException` per entry, logs them and
  continues. It **never** fails bootstrap and never shows UI beyond its own progress line.
- The exe leftover can still be locked if the old process has not exited yet. That is fine: the next
  startup deletes it.
- Mirror `EnsureApplicationDirectoriesExistenceStep` for translator use and `IsEnabled` handling.

### 3.7 Texts (`TranslationResources.resx` + `.pt-BR.resx`)

New keys (English shown, the developer adds pt-BR):
- `AppConfigurationPageViewModel_PerformSystemUpdate_TaskRunnerWindow_Title`: "Updating application"
- `…_PerformSystemUpdate_UpToDate`: "You are running the latest version ({0})."
- `…_PerformSystemUpdate_Confirm_Title`: "Update available"
- `…_PerformSystemUpdate_Confirm_Message`: "Version {1} is available (you have {0}). Download and install it now? The application will restart. Release notes: {2}"
- `…_PerformSystemUpdate_Failure`: "Could not update the application: {0}"
- `…_PerformSystemUpdate_RateLimited`: "GitHub's request limit was reached. Try again after {0}."
- `…_PerformSystemUpdate_RelaunchFailed`: "The update was installed, but the application could not restart: {0}. Close and reopen it."
- `PerformSystemUpdateRequestHandler_HandleAsync_{Starting|CheckingPrerequisites|DownloadingChecksum|DownloadingPackage|VerifyingPackage|ExtractingPackage|ValidatingPackage|ClosingDatabaseConnection|ClosingLuceneIndex|SwappingFiles|VerifyingInstallation|Success}`
  and a `_Failure` variant for each step and for `RollingBack`, plus `_SelfHeal_Failure`, `_RolledBack` and
  `_RolledBack_Cancelled`. The restore handler has the same set.
- `CleanupPreviousSystemUpdateStep_*` progress line(s).

## 4. Alternatives considered (inside this design)

| Choice | Picked | Alternative | Why not |
|---|---|---|---|
| Swap granularity | Per top-level entry | Rename the whole install directory (the brief) | Verified to fail with the open Serilog log and with CWD inside the directory (§1.1). Also needs write access to the parent directory. |
| Release lookup | `GET /releases` + filter `winapp-v*` + highest SemVer | `GET /releases/latest` | Returns the newest release of the *repository*, which can be a library `v*` release. |
| Integrity | `.sha256` sidecar asset | The asset `digest` field GitHub's API exposes | The sidecar is under the workflow's control, is checked in CI and is easy to verify by hand. Both come from the same origin, so neither adds authenticity. Revisit if the sidecar becomes a burden. |
| Version source | Informational version → `ApplicationContext:Version` arg | Read the assembly attribute directly in the check handler | Two sources would show one version and compare against another. |
| Dev-build guard | `AssemblyMetadata` marker set only by CI | Heuristics (`bin\Debug` in path, single-file detection via `Assembly.Location`) | Heuristics are fragile, and `Assembly.Location` triggers single-file analyzer warnings (build fails on warnings). |
| Staging location | `{Install}\.update-{ts}` | `{Root}\temporary\{ts}` (like restore) | Root is under ProgramData and may be on another volume. `Directory.Move` cannot cross volumes, and a copy is not atomic. |
| Cleanup timing | Last bootstrap step of the new version | Delete `.old-*` in the commit tail | The running exe cannot be deleted, and deleting the rest early destroys the only manual recovery path before the new version has started. |
| Check UX | Direct call, button disabled | TaskRunner window for the check too | Two windows in a row for a 1-request check. |
| Retries | None (user re-clicks) | Polly retry via `ResilienceHostFeature` | The action is manual and user-triggered. Retries would multiply calls against a 60/hour limit. |

## 5. Quality attributes and fitness functions

1. **Recoverability.** Nothing in the install directory is touched until the package is downloaded,
   hash-verified, extracted and validated. The commit point comes after a post-swap verification. Rollback
   reverses renames only, with no copies. Leftovers are kept until the new version boots.
   - *Fitness:* an `[IntegrationTest]` that copies `C:\Windows\System32\PING.EXE` into a temp "install"
     directory, starts it, runs the swap against it, and asserts both success and full rollback (by forcing
     a failure on the last move). This pins the OS behavior the whole design depends on (§1.1). If a
     Windows change breaks it, CI fails instead of users' installs.
2. **Integrity / security.** Covered in §6.
   - *Fitness:* the producer checks in the release workflow (contract §5). A unit test pins the asset name
     builder and tag parser to the exact strings in the contract (`winapp-v1.4.0` ↔
     `Nameless.WinApp-1.4.0-win-x64.zip` / `.zip.sha256`).
3. **Operability.** Every step publishes progress and step-specific failures. Rollback failures are
   reported per entry. The commit tail logs and never throws.
   - *Fitness:* a unit test for the cleanup regex. It must match `x.old-20261103120000` and
     `.update-20261103120000`, and must **not** match `logs`, `App_Data`, `foo.old-2026`, `foo.old-20261103120000.txt`
     or `Nameless.WinApp.exe`.
4. **Evolvability.** The release contract lives in one spec. Workflow and app are both checked against it.

## 6. Security notes (light threat model)

- **New entry points:** one outbound HTTPS call to `api.github.com`, two downloads, and a write into the
  install directory. There is no new inbound surface.
- **Spoofing / MITM:** HTTPS only. Download URLs must start with the exact repository download prefix
  (contract §4.1). HTTPS→HTTP redirects are refused (the .NET default, which must stay on).
- **Tampering:** SHA-256 catches corruption and truncation. It does **not** prove authenticity: anyone with
  write access to `marcoaoteixeira/Nameless` can publish a matching package+sidecar that runs as code on
  every updating machine. **Accepted** for a sample app (see the ADR). Follow-up if it ever ships to real
  users: Authenticode-sign the exe in CI and check the signature (`WinVerifyTrust`, pinned publisher)
  in step 6.
- **Downgrade:** only strictly higher SemVer is offered, so a lower-numbered release cannot be forced.
- **Zip handling:** `ZipFile.ExtractToDirectoryAsync` rejects path traversal. Forbidden root names are
  rejected. The size cap is checked before download, and the length is checked after.
- **Elevation:** the manifest is `asInvoker`. The update never elevates. A non-writable install directory
  fails at step 1 with guidance.
- **Destructive cleanup:** it only touches direct children of the install directory with the strict
  patterns in §3.6. It never deletes by a looser glob or outside those two directories.
- **Secrets / PII:** none. There is no token. `User-Agent` carries only the app version. Logs carry
  versions, URLs and paths.
- **DoS:** handles the unauthenticated limit (60/hour per IP) with a readable message. There is no
  automatic polling. Checks are user-triggered only.

## 7. Work split and tasks (BACKEND-ONLY, `backend-developer-dotnet`)

Build order:

1. **Version source.** csproj properties and the marker attribute, plus the `App.xaml.cs` arg (§3.1).
   Serves: the Version card shows the real version, and the comparison is correct.
2. **Constants and GitHub client.** `Constants.SystemUpdate`, `Infrastructure/GitHub/*`,
   `SystemUpdateHostFeature`, registration in `ApplicationHostFeature` (§3.2). Unit tests with a fake
   `HttpMessageHandler`.
3. **`CheckSystemUpdate` feature** (§3.3) and its unit tests.
4. **`PerformSystemUpdate` feature** (§3.4): environment seam, event, outcome, handler. Unit tests on temp
   directories plus the running-exe `[IntegrationTest]`.
5. **View model wiring** (§3.5) and texts in both resx files (§3.7).
6. **`CleanupPreviousSystemUpdateStep`** (§3.6) and tests.
7. **Release workflow** `.github/workflows/winapp-release.yml` per contract §2 and §5.
8. **CHANGELOG** `[Unreleased]` → `Added` entry (self-update, the release workflow, the version now shown
   correctly).

Rollout note: installs older than this feature cannot update themselves. The first release that contains
it must be installed by hand.

## 8. Test strategy

Every test is categorized (`[UnitTest]` / `[IntegrationTest]`), and there is no `#nullable` in the test
project. Tests go under `samples/windows/Nameless.WinApp.Tests/Features/{CheckSystemUpdate,PerformSystemUpdate}/`
and `…/Bootstrap/Steps/`.

| Behavior | Level | Key cases |
|---|---|---|
| Release selection | Unit (fake HTTP) | Mixed `v*` + `winapp-v*` tags. Drafts and prereleases ignored. Highest SemVer wins over the list order. Equal → up to date. Lower → up to date. Unparsable tag skipped. Empty list. |
| Asset contract | Unit | Missing package, missing sidecar, duplicate asset, oversize, zero size, wrong URL prefix, `http://` URL → malformed. Exact-name pinning test (fitness). |
| HTTP errors | Unit | 403 with `x-ratelimit-remaining: 0` and 429 → rate limited with reset time. 500 → status in message. Timeout. Unknown JSON fields tolerated. |
| Sidecar parsing | Unit | Valid. Uppercase hex. BOM. No trailing newline. Wrong name token. 63/65 chars. Non-hex. Empty. |
| Handler happy path | Unit (temp install dir, fake env/client) | Entries replaced, `.old-{ts}` siblings present, `logs/` untouched, staging and package removed, `Completed`, `Success` published last. |
| Pre-swap failures | Unit | Marker off. Install dir not writable. Checksum mismatch. Length mismatch. Forbidden entry. Exe missing. Version mismatch in the staged exe. For each: install dir byte-identical before and after, `RolledBack`, staging and package removed. |
| Swap failure + rollback | Unit | Fail on the *n*th move (lock the target with `FileShare.None`). Every earlier entry restored, the self-heal path is exercised, and a rollback failure on one entry still processes the others. |
| Cancellation | Unit | Cancel during download → `RolledBack` + `_RolledBack_Cancelled`, nothing touched. |
| Commit tail | Unit | The `Success` publish throws after the commit → still `Completed`, no rollback, error logged. (Lesson from #163.) |
| Running-exe swap | Integration | The PING.EXE fitness test (§5.1): success and forced rollback against a live process. |
| View model | Unit | Up to date → info SnackBar, no dialog. Dialog No → no request sent. Completed → relaunch + shutdown seam called. Relaunch throws → `RelaunchFailed`, no shutdown. Check throws → `Failure`. |
| Cleanup step | Unit | Deletes only the matching direct children and the `updates/*` files. A locked file is skipped and does not throw. Regex pinning test (fitness). |
| Release workflow | CI | Contract §5 checks run on every tag. |
| End to end | Manual (E2E) | Install `winapp-v0.0.1-test` into a user folder, publish `winapp-v0.0.2-test`, run the update from the UI, confirm the relaunch, confirm the `.old-*` leftovers are gone after the second start, confirm `logs/` and data are intact. Use a fork or temporary prerelease-free tags and delete them afterwards. |

The view-model relaunch and shutdown are not testable as written today (`Process.Start` /
`Application.Current` are called directly in the restore path too). Either accept manual coverage for that
branch, as restore does, or add a tiny `IApplicationRelauncher` seam. Developer's call. Keep it consistent
with whatever restore does.

## 9. Risks for QA to probe

- The install directory is under `Program Files` or another non-writable location. Expect a clear step-1
  failure and no changes.
- The install directory is inside a OneDrive-synced or antivirus-scanned folder. A scanner holding a handle
  on the freshly extracted exe can make the move fail. Expect a clean rollback. Note how often this happens.
- A second app instance is running from the same install directory during the update. Its exe is also
  renamed aside. It keeps running on the old image, and its `.old` leftover is cleaned once it exits and the
  app starts again.
- Relaunch race: the new process starts while the old one is still exiting. Check that Lucene `write.lock`,
  SQLite and the shutdown-time `app.config` save all behave. (`app.config` uses the atomic write from #162.
  Lucene was closed at step 7.)
- The app is launched with a working directory other than the install directory (shortcut "Start in").
  The update must still work (file-level swap), but Serilog's relative `logs/` lands elsewhere. That is
  existing behavior, not a regression.
- Network drops mid-download, disk is full during extract, cancel at each step.
- A new version that crashes on start. There is no watchdog (accepted in the ADR). The manual recovery is
  to delete the new entries and rename the `*.old-*` ones back. Write this down in the README or release
  notes.
- Long paths or non-ASCII characters in the install path.
- GitHub rate limit (61 checks in an hour from one IP).

## 10. Open decisions

None blocking. Items the developer confirms during implementation (§3.2): whether `HttpClient.Timeout`
covers the body read under `ResponseHeadersRead`, and whether `Microsoft.Extensions.Http` flows
transitively.

Follow-ups (ticket candidates):
- Authenticode signing in CI plus signature verification before the swap, if the app ever ships to real users.
- Pagination of `/releases` if the repository's release count makes the newest WinApp release fall past
  entry 30.
- Move Serilog's log folder to the app data root, so `logs/` stops living in the install directory. That is
  a separate concern, and this design no longer depends on it.
- Optional automatic "check on startup" with a notification. It is out of scope here and would need
  rate-limit thought.
