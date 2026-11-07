using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Nameless.Testing.Tools.IO;
using Nameless.WinApp.Tests.Tooling;

namespace Nameless.WinApp.Infrastructure.GitHub;

public class GitHubReleaseClientTests {
    private static GitHubReleaseClient CreateSut(Func<HttpRequestMessage, HttpResponseMessage> handler, TimeSpan? timeout = null) {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(handler)) {
            BaseAddress = new Uri("https://api.github.com/"),
            Timeout = timeout ?? TimeSpan.FromSeconds(15)
        };

        return new GitHubReleaseClient(httpClient);
    }

    private static GitHubReleaseClient CreateSut(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler, TimeSpan? timeout = null) {
        var httpClient = new HttpClient(new FakeHttpMessageHandler(handler)) {
            BaseAddress = new Uri("https://api.github.com/"),
            Timeout = timeout ?? TimeSpan.FromSeconds(15)
        };

        return new GitHubReleaseClient(httpClient);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string json) {
        return new HttpResponseMessage(statusCode) {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_WhenSuccessful_IgnoresUnknownJsonFields() {
        // arrange: GitHub's real payload has dozens of fields this feature
        // never reads; unknown-member handling must not throw.
        const string json = """
        [
          {
            "tag_name": "winapp-v1.4.0",
            "draft": false,
            "prerelease": false,
            "html_url": "https://github.com/marcoaoteixeira/Nameless/releases/tag/winapp-v1.4.0",
            "node_id": "RE_some_opaque_id",
            "assets": [
              { "name": "Nameless.WinApp-1.4.0-win-x64.zip", "size": 123, "browser_download_url": "https://github.com/marcoaoteixeira/Nameless/releases/download/winapp-v1.4.0/Nameless.WinApp-1.4.0-win-x64.zip", "unknown_field": 42 }
            ]
          }
        ]
        """;

        var sut = CreateSut(_ => JsonResponse(HttpStatusCode.OK, json));

        // act
        var releases = await sut.ListReleasesAsync(TestContext.Current.CancellationToken);

        // assert
        Assert.Single(releases);
        Assert.Equal("winapp-v1.4.0", releases[0].TagName);
        Assert.False(releases[0].Draft);
        Assert.False(releases[0].Prerelease);
        Assert.Single(releases[0].Assets);
        Assert.Equal(123, releases[0].Assets[0].Size);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_WhenEmpty_ReturnsEmptyList() {
        var sut = CreateSut(_ => JsonResponse(HttpStatusCode.OK, "[]"));

        var releases = await sut.ListReleasesAsync(TestContext.Current.CancellationToken);

        Assert.Empty(releases);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_When403WithRateLimitRemainingZero_ThrowsRateLimitedException() {
        var sut = CreateSut(_ => {
            var response = new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("rate limited") };
            response.Headers.Add("x-ratelimit-remaining", "0");
            response.Headers.Add("x-ratelimit-reset", "1800000000");

            return response;
        });

        var ex = await Assert.ThrowsAsync<GitHubReleaseClientException>(
            () => sut.ListReleasesAsync(TestContext.Current.CancellationToken)
        );

        Assert.True(ex.IsRateLimited);
        Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1800000000), ex.RateLimitResetsAt);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_When429WithRateLimitRemainingZero_ThrowsRateLimitedException() {
        var sut = CreateSut(_ => {
            var response = new HttpResponseMessage((HttpStatusCode)429) { Content = new StringContent("too many requests") };
            response.Headers.Add("x-ratelimit-remaining", "0");

            return response;
        });

        var ex = await Assert.ThrowsAsync<GitHubReleaseClientException>(
            () => sut.ListReleasesAsync(TestContext.Current.CancellationToken)
        );

        Assert.True(ex.IsRateLimited);
        Assert.Null(ex.RateLimitResetsAt);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_When403WithoutRateLimitHeader_ThrowsNonRateLimitedException() {
        // A plain 403 (e.g. blocked IP) must not be mistaken for a rate limit.
        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("forbidden") });

        var ex = await Assert.ThrowsAsync<GitHubReleaseClientException>(
            () => sut.ListReleasesAsync(TestContext.Current.CancellationToken)
        );

        Assert.False(ex.IsRateLimited);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_WhenServerError_ThrowsExceptionWithStatusCodeInMessage() {
        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("boom") });

        var ex = await Assert.ThrowsAsync<GitHubReleaseClientException>(
            () => sut.ListReleasesAsync(TestContext.Current.CancellationToken)
        );

        Assert.False(ex.IsRateLimited);
        Assert.Contains("500", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_WhenRequestTimesOut_ThrowsDistinctFailureInsteadOfPropagatingCancellation() {
        var sut = CreateSut(async (_, cancellationToken) => {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }, timeout: TimeSpan.FromMilliseconds(50));

        var ex = await Assert.ThrowsAsync<GitHubReleaseClientException>(
            () => sut.ListReleasesAsync(TestContext.Current.CancellationToken)
        );

        Assert.False(ex.IsRateLimited);
    }

    [Fact]
    [UnitTest]
    public async Task ListReleasesAsync_WhenCallerCancels_PropagatesCancellationInsteadOfWrapping() {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.OK));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => sut.ListReleasesAsync(cts.Token)
        );
    }

    [Fact]
    [UnitTest]
    public async Task DownloadTextAsync_WhenWithinCap_ReturnsContent() {
        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("hello") });

        var text = await sut.DownloadTextAsync(new Uri("https://example.test/a.sha256"), maxBytes: 1024, TestContext.Current.CancellationToken);

        Assert.Equal("hello", text);
    }

    [Fact]
    [UnitTest]
    public async Task DownloadTextAsync_WhenBodyExceedsCap_ThrowsInvalidDataExceptionBeforeAnyFileIsTouched() {
        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(new string('a', 2048))
        });

        await Assert.ThrowsAsync<InvalidDataException>(
            () => sut.DownloadTextAsync(new Uri("https://example.test/a.sha256"), maxBytes: 1024, TestContext.Current.CancellationToken)
        );
    }

    [Fact]
    [UnitTest]
    public async Task DownloadToFileAsync_StreamsBodyToDestinationFile() {
        using var root = new TempDirectory(nameof(GitHubReleaseClientTests));
        var destinationPath = SysPath.Combine(root.Path, "package.zip");
        var body = "zip-bytes-stand-in";

        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(body)
        });

        await sut.DownloadToFileAsync(new Uri("https://example.test/package.zip"), destinationPath, maxBytes: body.Length, TestContext.Current.CancellationToken);

        Assert.Equal(body, SysFile.ReadAllText(destinationPath));
    }

    [Fact]
    [UnitTest]
    public async Task DownloadToFileAsync_WhenBodyExceedsCap_AbortsMidCopyInsteadOfOnlyCheckingLengthAfterward() {
        // A wrong/hostile response could otherwise fill the disk before any
        // length check runs: the copy must abort as soon as the cap is
        // crossed, not just be checked once the whole body is written. The
        // body here is far larger than the cap so that a post-loop-only
        // length check (checking after the whole body was copied) would
        // still throw, but would also have written the full body to disk
        // first — the file length assertion below is what distinguishes
        // the two.
        using var root = new TempDirectory(nameof(GitHubReleaseClientTests));
        var destinationPath = SysPath.Combine(root.Path, "package.zip");
        // Large enough relative to the cap that several read/write chunks
        // happen before the cap is crossed — a trivial single-chunk body
        // would abort before any bytes were written at all, which would
        // not distinguish "aborts mid-copy" from "aborts before copying".
        const long maxBytes = 200_000;
        const long bodyLength = 10 * maxBytes;

        var sut = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(new string('a', (int)bodyLength))
        });

        await Assert.ThrowsAsync<InvalidDataException>(
            () => sut.DownloadToFileAsync(new Uri("https://example.test/package.zip"), destinationPath, maxBytes, TestContext.Current.CancellationToken)
        );

        var writtenLength = new FileInfo(destinationPath).Length;

        Assert.True(writtenLength > 0, "Expected some bytes to have been written before the cap was crossed.");
        Assert.True(
            writtenLength <= maxBytes,
            $"Expected the partial file to be capped at {maxBytes} bytes (proving the copy aborted mid-transfer), but it was {writtenLength} bytes."
        );
        Assert.True(writtenLength < bodyLength, "The full body was written before the length check ran.");
    }
}
