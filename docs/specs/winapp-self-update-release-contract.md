# Nameless.WinApp self-update: GitHub release contract

- Ticket: [#167](https://github.com/marcoaoteixeira/Nameless/issues/167) (feature from the self-update
  brainstorm). Plan: [`docs/plans/winapp-self-update.md`](../plans/winapp-self-update.md).
  ADR: [`docs/adr/ADR-001-Self-Update-WinApp-In-Process-By-Renaming-Running-Files-Aside.md`](../adr/ADR-001-Self-Update-WinApp-In-Process-By-Renaming-Running-Files-Aside.md).
- Parties: the release workflow (producer, new file `.github/workflows/winapp-release.yml`) and the app's
  self-update feature (consumer). This contract binds both. Report any deviation; do not change one side alone.
- Compatibility: new contract. There is no earlier one to break. The repository already has `v*` tags
  (`v2.2.6` … `v3.7.4`) for the libraries. The WinApp uses its own tag prefix so the two release series
  never mix.

## 1. Release identity

| Item | Rule | Example |
|---|---|---|
| Git tag | `winapp-v{SEMVER}`. `{SEMVER}` is `MAJOR.MINOR.PATCH` with an optional `-prerelease`. No `+build` metadata. | `winapp-v1.4.0` |
| `{VERSION}` | The tag without the `winapp-v` prefix. | `1.4.0` |
| Release state | `draft: false`. A stable release has `prerelease: false`. A tag with `-prerelease` **must** be published with `prerelease: true`. | |
| Release title | Free text. The app does not read it. | `Nameless.WinApp 1.4.0` |

The app only uses releases where **all** of these are true: the tag starts with `winapp-v`, the rest of the
tag parses as a `SemVersion`, `draft == false` and `prerelease == false`. It ignores every other release,
including the library `v*` releases. This version has no pre-release channel.

## 2. Release assets

Every WinApp release carries **exactly** these two assets. Other assets may exist and the app ignores them.

### 2.1 Package: `Nameless.WinApp-{VERSION}-win-x64.zip`

- Contents: the **root** of the `dotnet publish` output, with no wrapping folder.
- The publish must be self-contained and single-file, so the package is one exe plus loose content files:

  ```
  dotnet publish samples/windows/Nameless.WinApp/Nameless.WinApp.csproj
    -c Release -r win-x64 --self-contained true
    -p:PublishSingleFile=true
    -p:IncludeNativeLibrariesForSelfExtract=true
    -p:EnableCompressionInSingleFile=true
    -p:Version={VERSION}
    -p:SelfUpdateEnabled=true
    -o <publishDir>
  ```

  These flags repeat `win-x64.pubxml` explicitly. That file sits in the project root, not under
  `Properties/PublishProfiles`, so `-p:PublishProfile=win-x64` would not find it. The flags remove any doubt
  about which profile was used.
- Required at the archive root: `Nameless.WinApp.exe`.
- Expected at the archive root: `appsettings.json`. `Nameless.WinApp.pdb` is allowed.
- **Forbidden** entry names at the root (the app rejects the package if it finds them):
  `logs`, `App_Data`, `.update-*`, and any name matching `*.old-*`. The first two are runtime data folders
  that can live in the install directory. The last two are the updater's own working names.
- Path traversal (`..`, absolute paths) is forbidden. The app's extractor rejects it anyway.
- Size: at most **512 MiB**. The app refuses larger assets before it downloads them.

Why single-file is required and not just tidier: during the swap the running process keeps executing from
its renamed exe. A single-file bundle loads every managed assembly from that one open image, so the old
process never lazily loads a DLL from the install directory, where the *new* version's DLLs would now be.
A multi-file publish would allow that version mix. This was verified with a .NET 10 single-file
self-contained test app. It kept running and lazily loading types after its exe was renamed and replaced.

### 2.2 Checksum sidecar: `Nameless.WinApp-{VERSION}-win-x64.zip.sha256`

- One line in GNU `sha256sum` format, UTF-8 (BOM allowed and ignored), with a trailing newline that is
  optional:

  ```
  3f7a…<64 hex chars>…9c1e  Nameless.WinApp-1.4.0-win-x64.zip
  ```

- The app reads the first whitespace-separated token. It must be exactly 64 hex characters (case-insensitive).
  The second token, if present, must equal the package asset name. Anything else is a malformed sidecar,
  and the update fails before the swap.
- Producing it in PowerShell:
  `"{0}  {1}" -f (Get-FileHash $zip -Algorithm SHA256).Hash.ToLowerInvariant(), (Split-Path $zip -Leaf)`.

What the checksum does and does not give: it detects a truncated or corrupted download, and a package that
does not match what the workflow uploaded. It is **not** proof of authenticity. It comes from the same
release as the package, so anyone who can publish a release in this repository can publish a matching pair.
Authenticity depends on GitHub's TLS and on who has write access to the repository. See plan §6.

## 3. Version stamping and mapping

| Where | Value for tag `winapp-v1.4.0` | How |
|---|---|---|
| MSBuild `Version` | `1.4.0` | `-p:Version={VERSION}` |
| `AssemblyInformationalVersion` | `1.4.0` (no `+sha`) | `Nameless.WinApp.csproj` sets `<IncludeSourceRevisionInInformationalVersion>false</IncludeSourceRevisionInInformationalVersion>` |
| Win32 `ProductVersion` of the exe | `1.4.0` | Comes from the informational version. Verified on a single-file bundle. |
| `IApplicationContext.Version` | `v1.4.0` | `App.xaml.cs` adds `--ApplicationContext:Version={informational version}` to the host args. `ApplicationContext` formats it with the `v` prefix. |
| `AssemblyVersion` | stays `1.0.0` | Pinned in `Directory.Build.props` for every project. The update **must not** read it. |
| Self-update marker | `[assembly: AssemblyMetadata("Nameless.WinApp.SelfUpdate", "Enabled")]` | Emitted only when `-p:SelfUpdateEnabled=true` (conditional `AssemblyAttribute` item in the csproj). |

Comparison rule: `current = SemVersion.Parse(IApplicationContext.Version)`.
`candidate = SemVersion.Parse(tag without "winapp-")`. `SemVersion` already accepts the `v` prefix.
An update is available iff `candidate.CompareTo(current) > 0`. Among the eligible releases (§1), the
candidate is the one with the **highest** `SemVersion`, not the newest by date and not the first in the list.

Today `ApplicationContext:Version` is never set, so every build shows `v1.0.0` (`SemVersion.V1`, the
fallback). After this change a local dev build shows `v1.0.0` (the SDK default `Version`). It therefore
always sees an available update. The self-update marker keeps a dev build from *applying* one (plan §3.4).

## 4. GitHub REST API calls (consumer side)

No authentication (public repository). Base URL `https://api.github.com`.

### 4.1 List releases

```
GET /repos/marcoaoteixeira/Nameless/releases?per_page=30&page=1
Accept: application/vnd.github+json
X-GitHub-Api-Version: 2022-11-28
User-Agent: Nameless.WinApp/{current version}
```

Why `/releases` and not `/releases/latest`: `latest` returns the newest non-draft, non-prerelease release
of the **whole repository**. That can be a library `v*` release. One page of 30 is enough because WinApp and
library releases together will not push the newest WinApp release past 30 entries soon. If that changes,
paginate (Follow-up in the plan).

Fields read (everything else is ignored, and unknown fields must not fail deserialization):

| Field | Type | Use |
|---|---|---|
| `tag_name` | string | Prefix filter and version parse |
| `draft` | bool | Must be `false` |
| `prerelease` | bool | Must be `false` |
| `html_url` | string (https URL) | Shown as "release notes" link text in the confirm dialog (optional, display only) |
| `assets[].name` | string | Exact match for the package and sidecar names (§2) |
| `assets[].size` | long | Package size cap (§2.1) and post-download length check |
| `assets[].browser_download_url` | string (URL) | Download URL. Must pass the URL rule below. |

Download URL rule: it must be `https` and start with
`https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v{VERSION}/`. GitHub then redirects
to its asset CDN host, also over HTTPS. The HTTP client follows redirects but never follows one from
HTTPS to HTTP (the .NET default).

### 4.2 Download assets

`GET {browser_download_url}` for the sidecar first, then for the package, with the same `User-Agent`
header. The package is streamed to disk (never buffered in memory). It is cancellable through the
TaskRunner token. After the download, the file length must equal `assets[].size`.

### 4.3 Error mapping

| Condition | Outcome in the app |
|---|---|
| Network failure / DNS / TLS / timeout on the list call (timeout 15 s) | Check fails. Failure SnackBar "could not reach GitHub". |
| `403` or `429` with `x-ratelimit-remaining: 0` | Check fails. Failure SnackBar "GitHub rate limit reached, try again after {x-ratelimit-reset as local time}". Unauthenticated limit: 60 requests/hour per IP. |
| Any other non-2xx | Check fails. Failure SnackBar with the status code. |
| No eligible release, or highest eligible ≤ current | Information SnackBar "you are on the latest version ({current})". |
| Eligible release is missing the package or sidecar, or has more than one of either | Check fails: "release {tag} is malformed". The app does not fall back to an older release. |
| Download URL fails the URL rule | Same as malformed. |
| Download non-2xx, length mismatch, checksum mismatch, malformed sidecar | Update fails before any file is touched (`RolledBack` with nothing to roll back). |

## 5. Producer checks (release workflow fitness function)

Before `gh release create`, the workflow must fail the job if any of these is false:

1. The tag matches `^winapp-v\d+\.\d+\.\d+(-[0-9A-Za-z.-]+)?$`.
2. `Nameless.WinApp.exe` is at the zip root, and no forbidden root entry is present (§2.1).
3. `(Get-Item <publishDir>\Nameless.WinApp.exe).VersionInfo.ProductVersion -eq $VERSION`.
4. Re-hashing the zip gives the hash in the sidecar.
5. The release is created with `--prerelease` iff `$VERSION` contains `-`.

Workflow outline (the developer writes the YAML): trigger `push: tags: ['winapp-v*']`, `runs-on: windows-latest`,
`permissions: contents: write`, `actions/setup-dotnet` (10.0.x), `dotnet test` for
`samples/windows/Nameless.WinApp.Tests`, publish (§2.1), zip with `Compress-Archive -Path <publishDir>\*`,
write the sidecar, run the checks above, then
`gh release create $TAG <zip> <sha256> --title "Nameless.WinApp $VERSION" --generate-notes`.
