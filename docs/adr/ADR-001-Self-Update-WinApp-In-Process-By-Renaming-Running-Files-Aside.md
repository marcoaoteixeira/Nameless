# Self-update Nameless.WinApp in-process from GitHub Releases by renaming its running files aside

* **Status:** Accepted
* **Group:** Integration
* **Deciders:** Marco Teixeira (chose the mechanism in the self-update brainstorm and delegated the design)
* **Date:** 2026-11-03

## Context and Problem Statement

`Nameless.WinApp` has an "update" button on the AppConfigurationPage Version card, but no update mechanism
behind it. Releases are going to be published as GitHub Release assets, built by GitHub Actions. The app
must find the newest release through GitHub's public REST API and replace its own binaries while it is
running. Windows does not allow a running executable to be overwritten in place. So the update needs either
a second actor (a helper process or the OS) or a trick that works around the lock from inside the process.
This decision also brings in GitHub's Releases API as the app's first external API dependency.

**Assumptions:**

* The app ships as a self-contained **single-file** `win-x64` publish, so the install directory holds one exe
  and a few loose content files.
* It is installed in a folder the user can write to (not `Program Files`). It runs `asInvoker` and never
  elevates.
* It is a desktop **sample** app. It is not safety-critical, has no SLA, and a user can reinstall it by hand.
* The repository `marcoaoteixeira/Nameless` stays public, so the Releases API needs no token. One user
  clicking the button stays far below the unauthenticated limit of 60 requests/hour per IP.
* On NTFS, a running image can be renamed and a new file put at its old path (verified on Windows 11 for
  `ping.exe` and for a .NET 10 single-file bundle, see the plan §1.1).

## Decision Drivers

* Reuse the relaunch pattern the app already has (`Process.Start(Environment.ProcessPath!)` +
  `Application.Current.Shutdown()` in the restore flow) instead of adding new process orchestration.
* No new executable, packaging step, signing surface or OS registration to build, ship and keep in sync.
* A failure before the commit point must leave the installed app exactly as it was. A failure after it must
  never be reported as a rollback. This is the same discipline as the restore handler (#162, #163).
* It must work with what holds files open in the install directory during a normal run. The Serilog log file
  and the process working directory both do, and this was measured.
* Keep the cost proportional to a sample app.

## Considered Options

* **Option 1: In-process rename trick, per top-level entry.** Download and verify the package, then extract
  it into a staging folder inside the install directory. Rename each install-directory entry that the package
  replaces (including the running exe) to `{name}.old-{timestamp}`. Move the new entry into place, verify,
  commit, relaunch the new exe and shut down. The new version deletes the `.old-*` leftovers once it has
  booted.
  * Pros: no second executable. All logic is in one testable handler. Rollback is plain renames. Only needs
    write access to the install directory. Leaves `logs/` and `App_Data/` alone.
  * Cons: relies on Windows rename semantics for running images. No watchdog after the relaunch. Stale files
    from an older version stay behind unless a later package replaces them.
* **Option 1a: In-process rename trick on the whole install directory** (the brainstorm's original wording).
  Rename the install directory to a `.old-{timestamp}` sibling and move the new directory in.
  * Pros: one rename. Old and new are cleanly separated.
  * Cons: Windows refuses to rename a directory while any file in it is open without share-delete. That
    happens on every normal run, because Serilog holds `logs/app-*.log`. It also fails when the directory is
    the process's working directory, which is the case for an Explorer launch. It needs write access to the
    parent directory.
  * Rejected because: tested, and it fails in the app's ordinary configuration (plan §1.1). Option 1 keeps
    the same mechanism at a granularity that works.
* **Option 2: Separate updater helper process.** A small second exe waits for the app to exit, replaces the
  files, then starts the app.
  * Pros: the industry-standard shape (Squirrel/Velopack, Chrome). It can watch the relaunch and roll back
    if the new version fails to start.
  * Cons: a second executable to build, publish, version, sign and keep compatible with the app. It also
    has to update *itself*. It adds a new project and packaging surface to a sample app.
  * Rejected because: its main benefit, a watchdog, does not justify a new shipped artifact for a
    non-critical sample app (Decision Drivers 2 and 5).
* **Option 3: OS scheduled task or batch-script bounce.** Write a `.cmd`/PowerShell script or register a
  scheduled task that waits for the app to exit, copies the files and relaunches.
  * Pros: no new compiled artifact.
  * Cons: logic in an untested script outside the codebase's conventions. Fragile quoting and paths.
    Scheduled tasks need extra permissions and leave OS state behind. Antivirus heuristics often flag
    "exe writes and runs a script". Errors are invisible to the app's progress and SnackBar UI.
  * Rejected because: it moves the riskiest step out of the tested, observable code path (Driver 3).
* **Option 4: MSIX / ClickOnce / a third-party updater framework.**
  * Pros: OS-managed updates and signing built in.
  * Cons: changes how the app is packaged and installed, adds certificate and vendor setup, and is far
    beyond a sample's needs.
  * Rejected because: it is a packaging-model change, not a feature. It was out of the brainstorm's scope.

## Decision Outcome

**Chosen option:** "Option 1: In-process rename trick, per top-level entry". It reuses the relaunch flow the
app already has and the commit-point and rollback discipline of the restore handler (Drivers 1 and 3). It
adds no second shipped artifact (Driver 2). It is the only in-process variant that works while the app's
own log file is open (Driver 4). Implementation stays inside one feature folder, one bootstrap step and one
CI workflow, which a single developer can build and test with real files (Driver 5).

GitHub Releases is consumed through the unauthenticated REST API (`GET /repos/{owner}/{repo}/releases`,
filtered by the `winapp-v` tag prefix). Integrity is checked with a SHA-256 sidecar asset. The asset and
version contract is in `docs/specs/winapp-self-update-release-contract.md`.

### Consequences

* **Good:** the update is a single tested handler that publishes progress. Pre-commit failures roll back to
  the exact previous install, and the user sees step-level failure messages.
* **Good:** no updater exe, no script and no OS registration to maintain. Adding the release workflow also
  makes the app show its real version (it shows `v1.0.0` today).
* **Bad (accepted risk):** there is no watchdog. If the new version is committed but fails to start (a
  crash on boot, or a broken release that still passed the checksum and version checks), nothing rolls it
  back. The app stays broken until someone fixes it by hand: delete the new entries and rename the
  `*.old-*` entries back. Those leftovers are deleted only after the new version completes bootstrap, so
  the manual recovery stays possible in exactly that case. A failed `Process.Start` itself is *not* fatal:
  the old process is still alive, tells the user to reopen, and the next launch runs the new version.
* **Bad:** the checksum gives integrity, not authenticity. Anyone with write access to the repository can
  publish an update that every updating install will run. This is accepted for a sample app. There is no
  code signing.
* **Bad:** the design depends on Windows letting a running image be renamed. That holds today, and a
  `[IntegrationTest]` against a live process pins it.
* **Bad:** files a newer package no longer contains are not removed (stale leftovers). This is negligible
  with a single-file publish.
* **Constraint:** the release must be a self-contained single-file publish. A multi-file publish would let
  the old process lazily load the new version's DLLs from the install directory after the swap.
* **Constraint:** the app reads its version only from `IApplicationContext.Version`, fed from the assembly
  informational version stamped by `-p:Version`. `AssemblyVersion` (pinned to `1.0.0` repo-wide) must not be
  used for update decisions.
* **Constraint:** WinApp releases use the `winapp-v{semver}` tag series, separate from the library `v*` tags.
  The asset names and the sidecar format in the contract spec are fixed. Changing them is a breaking change
  for every installed version.
* **Constraint:** runtime data must never be put into the release package (`logs`, `App_Data` are rejected).
  The updater only replaces entries the package contains.
* **Follow-up:** create `.github/workflows/winapp-release.yml` with the producer checks in the contract.
* **Follow-up:** if the app is ever distributed beyond a sample, revisit this ADR. Add Authenticode signing
  with signature verification, and consider Option 2 for a watchdog.
* **Follow-up:** installs that predate this feature cannot update themselves. The first release containing
  it must be installed by hand.

## Related

* **Requirement:** self-update from the AppConfigurationPage Version card (`PerformSystemUpdateCommand`).
  There is no GitHub issue yet.
* **Decisions:** first ADR in this repository. It mirrors the commit-point discipline from #162 and #163
  (restore handler). It does not supersede anything.
* **Artifacts:** `docs/plans/winapp-self-update.md`, `docs/specs/winapp-self-update-release-contract.md`.
