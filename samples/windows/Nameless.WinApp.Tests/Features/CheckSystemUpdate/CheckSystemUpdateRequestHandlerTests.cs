using System.IO;
using Moq;
using Nameless.Application;
using Nameless.WinApp.Infrastructure.GitHub;

namespace Nameless.WinApp.Features.CheckSystemUpdate;

public class CheckSystemUpdateRequestHandlerTests {
    private static GitHubReleaseAsset PackageAsset(string version, long size = 1024, string? urlOverride = null) {
        var name = $"Nameless.WinApp-{version}-win-x64.zip";
        var url = urlOverride ?? $"https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v{version}/{name}";

        return new GitHubReleaseAsset(name, size, new Uri(url));
    }

    private static GitHubReleaseAsset ChecksumAsset(string version, string? urlOverride = null) {
        var name = $"Nameless.WinApp-{version}-win-x64.zip.sha256";
        var url = urlOverride ?? $"https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v{version}/{name}";

        return new GitHubReleaseAsset(name, 128, new Uri(url));
    }

    private static GitHubRelease ValidRelease(string version, bool draft = false, bool prerelease = false, IReadOnlyList<GitHubReleaseAsset>? assets = null) {
        return new GitHubRelease(
            TagName: $"winapp-v{version}",
            Draft: draft,
            Prerelease: prerelease,
            HtmlUrl: new Uri($"https://github.com/marcoaoteixeira/Nameless/releases/tag/winapp-v{version}"),
            Assets: assets ?? [PackageAsset(version), ChecksumAsset(version)]
        );
    }

    private static (CheckSystemUpdateRequestHandler Handler, Mock<IGitHubReleaseClient> ClientMock) CreateSut(
        IReadOnlyList<GitHubRelease> releases, string currentVersion = "v1.0.0") {
        var clientMock = new Mock<IGitHubReleaseClient>();
        clientMock.Setup(x => x.ListReleasesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(releases);

        var applicationContextMock = new Mock<IApplicationContext>();
        applicationContextMock.SetupGet(x => x.Version).Returns(currentVersion);

        var handler = new CheckSystemUpdateRequestHandler(clientMock.Object, applicationContextMock.Object);

        return (handler, clientMock);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenRequestIsNull_ThrowsArgumentNullException() {
        var (handler, _) = CreateSut([]);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => handler.HandleAsync(null!, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenNoReleases_ReturnsNoUpdateAvailable() {
        var (handler, _) = CreateSut([]);

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.False(result.IsUpdateAvailable);
        Assert.Null(result.Release);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenHighestEligibleIsNotFirstInList_PicksHighestSemVerRegardlessOfOrder() {
        var releases = new[] {
            ValidRelease("1.1.0"),
            ValidRelease("1.5.0"), // highest, but in the middle of the list
            ValidRelease("1.2.0")
        };

        var (handler, _) = CreateSut(releases);

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.True(result.IsUpdateAvailable);
        Assert.Equal("winapp-v1.5.0", result.Release!.TagName);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_IgnoresLibraryTagsAndDraftsAndPrereleases() {
        var libraryRelease = new GitHubRelease("v9.9.9", Draft: false, Prerelease: false, HtmlUrl: null, Assets: []);
        var draftRelease = ValidRelease("2.0.0", draft: true);
        var prereleaseRelease = ValidRelease("3.0.0", prerelease: true);
        var eligibleRelease = ValidRelease("1.2.0");

        var (handler, _) = CreateSut([libraryRelease, draftRelease, prereleaseRelease, eligibleRelease]);

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.True(result.IsUpdateAvailable);
        Assert.Equal("winapp-v1.2.0", result.Release!.TagName);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenTagIsUnparsable_SkipsItInsteadOfThrowing() {
        var unparsable = new GitHubRelease("winapp-v-not-a-version", Draft: false, Prerelease: false, HtmlUrl: null, Assets: []);
        var eligibleRelease = ValidRelease("1.2.0");

        var (handler, _) = CreateSut([unparsable, eligibleRelease]);

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.True(result.IsUpdateAvailable);
        Assert.Equal("winapp-v1.2.0", result.Release!.TagName);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenHighestEligibleEqualsCurrent_ReturnsNoUpdateAvailable() {
        var (handler, _) = CreateSut([ValidRelease("1.0.0")], currentVersion: "v1.0.0");

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.False(result.IsUpdateAvailable);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenHighestEligibleIsLowerThanCurrent_ReturnsNoUpdateAvailable() {
        var (handler, _) = CreateSut([ValidRelease("0.9.0")], currentVersion: "v1.0.0");

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.False(result.IsUpdateAvailable);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenPackageAssetIsMissing_ThrowsInvalidDataExceptionWithoutFallingBackToOlderRelease() {
        var malformedNewest = ValidRelease("2.0.0", assets: [ChecksumAsset("2.0.0")]);
        var olderValid = ValidRelease("1.0.0");

        var (handler, _) = CreateSut([malformedNewest, olderValid]);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );

        Assert.Contains("malformed", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenChecksumAssetIsMissing_ThrowsInvalidDataException() {
        var malformed = ValidRelease("2.0.0", assets: [PackageAsset("2.0.0")]);

        var (handler, _) = CreateSut([malformed]);

        await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenPackageAssetAppearsTwice_ThrowsInvalidDataException() {
        var malformed = ValidRelease("2.0.0", assets: [PackageAsset("2.0.0"), PackageAsset("2.0.0"), ChecksumAsset("2.0.0")]);

        var (handler, _) = CreateSut([malformed]);

        await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenPackageSizeIsZero_ThrowsInvalidDataException() {
        var malformed = ValidRelease("2.0.0", assets: [PackageAsset("2.0.0", size: 0), ChecksumAsset("2.0.0")]);

        var (handler, _) = CreateSut([malformed]);

        await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenPackageSizeExceedsCap_ThrowsInvalidDataException() {
        var malformed = ValidRelease("2.0.0", assets: [PackageAsset("2.0.0", size: 512L * 1024 * 1024 + 1), ChecksumAsset("2.0.0")]);

        var (handler, _) = CreateSut([malformed]);

        await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenDownloadUrlDoesNotMatchRepositoryPrefix_ThrowsInvalidDataException() {
        var malformed = ValidRelease("2.0.0", assets: [
            PackageAsset("2.0.0", urlOverride: "https://evil.example.com/Nameless.WinApp-2.0.0-win-x64.zip"),
            ChecksumAsset("2.0.0")
        ]);

        var (handler, _) = CreateSut([malformed]);

        await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenDownloadUrlIsPlainHttp_ThrowsInvalidDataException() {
        var malformed = ValidRelease("2.0.0", assets: [
            PackageAsset("2.0.0", urlOverride: "http://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v2.0.0/Nameless.WinApp-2.0.0-win-x64.zip"),
            ChecksumAsset("2.0.0")
        ]);

        var (handler, _) = CreateSut([malformed]);

        await Assert.ThrowsAsync<InvalidDataException>(
            () => handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task HandleAsync_WhenReleaseIsWellFormed_MapsExactAssetNamesPerContract() {
        // Fitness test (plan §5.2): pins the exact mapping between the tag
        // and the asset names a release workflow must produce.
        var (handler, _) = CreateSut([ValidRelease("1.4.0")]);

        var result = await handler.HandleAsync(new CheckSystemUpdateRequest(), TestContext.Current.CancellationToken);

        Assert.True(result.IsUpdateAvailable);
        Assert.Equal("winapp-v1.4.0", result.Release!.TagName);
        Assert.Equal("Nameless.WinApp-1.4.0-win-x64.zip", result.Release.Package.Name);
        Assert.Equal("Nameless.WinApp-1.4.0-win-x64.zip.sha256", result.Release.Checksum.Name);
    }
}
