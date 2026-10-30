using Microsoft.Extensions.Logging;

namespace Nameless.Testing.Tools.Logging;

[UnitTest]
public class InMemoryLoggerTests {
    private static string Formatter(string state, Exception? exception) => state;

    // ─── IsEnabled ──────────────────────────────────────────────────────────

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void IsEnabled_ReturnsTrue_ForAnyLevelOtherThanNone(LogLevel level) {
        var sut = new InMemoryLogger();

        var result = sut.IsEnabled(level);

        Assert.True(result);
    }

    [Fact]
    public void IsEnabled_ReturnsFalse_ForNone() {
        var sut = new InMemoryLogger();

        var result = sut.IsEnabled(LogLevel.None);

        Assert.False(result);
    }

    // ─── Log ────────────────────────────────────────────────────────────────

    [Fact]
    public void Log_CapturesLevelEventIdMessageAndState() {
        var sut = new InMemoryLogger();
        var eventId = new EventId(id: 42, name: "SomethingHappened");

        sut.Log(LogLevel.Information, eventId, "hello world", exception: null, Formatter);

        var entry = Assert.Single(sut.Entries);
        Assert.Multiple(
            () => Assert.Equal(LogLevel.Information, entry.Level),
            () => Assert.Equal(eventId, entry.EventId),
            () => Assert.Equal("hello world", entry.Message),
            () => Assert.Equal("hello world", entry.State),
            () => Assert.Null(entry.Exception)
        );
    }

    [Fact]
    public void Log_CapturesException() {
        var sut = new InMemoryLogger();
        var exception = new InvalidOperationException("boom");

        sut.Log(LogLevel.Error, eventId: default, "oops", exception, Formatter);

        var entry = Assert.Single(sut.Entries);
        Assert.Same(exception, entry.Exception);
    }

    [Fact]
    public void Log_WithNullFormatter_Throws() {
        var sut = new InMemoryLogger();

        Assert.Throws<ArgumentNullException>(() => sut.Log<string>(LogLevel.Information, default, "state", exception: null, formatter: null!));
    }

    [Fact]
    public void Log_MultipleCalls_AreCapturedInOrder() {
        var sut = new InMemoryLogger();

        sut.Log(LogLevel.Information, default, "first", exception: null, Formatter);
        sut.Log(LogLevel.Warning, default, "second", exception: null, Formatter);

        Assert.Collection(sut.Entries,
            first => Assert.Equal("first", first.Message),
            second => Assert.Equal("second", second.Message));
    }

    // ─── Scopes ─────────────────────────────────────────────────────────────

    [Fact]
    public void BeginScope_AddsStateToActiveAndCapturedScopes() {
        var sut = new InMemoryLogger();

        using var _ = sut.BeginScope("scope-a");

        Assert.Multiple(
            () => Assert.Equal(["scope-a"], sut.ActiveScopes),
            () => Assert.Equal(["scope-a"], sut.CapturedScopes)
        );
    }

    [Fact]
    public void DisposingScope_RemovesItFromActiveScopes_ButNotFromCapturedScopes() {
        var sut = new InMemoryLogger();

        var scope = sut.BeginScope("scope-a");
        scope.Dispose();

        Assert.Multiple(
            () => Assert.Empty(sut.ActiveScopes),
            () => Assert.Equal(["scope-a"], sut.CapturedScopes)
        );
    }

    [Fact]
    public void DisposingScope_Twice_DoesNotThrowOrDuplicateRemoval() {
        var sut = new InMemoryLogger();

        var scope = sut.BeginScope("scope-a");
        scope.Dispose();
        scope.Dispose();

        Assert.Empty(sut.ActiveScopes);
    }

    [Fact]
    public void NestedScopes_AreStackedOuterToInner() {
        var sut = new InMemoryLogger();

        using var outer = sut.BeginScope("outer");
        using var inner = sut.BeginScope("inner");

        Assert.Equal(["outer", "inner"], sut.ActiveScopes);
    }

    [Fact]
    public void DisposingOuterScope_WhileInnerStillActive_RemovesOnlyOuter() {
        var sut = new InMemoryLogger();

        var outer = sut.BeginScope("outer");
        using var inner = sut.BeginScope("inner");

        outer.Dispose();

        Assert.Equal(["inner"], sut.ActiveScopes);
    }

    [Fact]
    public void Log_CapturesSnapshotOfActiveScopesAtLogTime() {
        var sut = new InMemoryLogger();

        using (sut.BeginScope("outer")) {
            sut.Log(LogLevel.Information, default, "inside outer", exception: null, Formatter);

            using (sut.BeginScope("inner")) {
                sut.Log(LogLevel.Information, default, "inside inner", exception: null, Formatter);
            }

            sut.Log(LogLevel.Information, default, "back to outer", exception: null, Formatter);
        }

        sut.Log(LogLevel.Information, default, "no scope", exception: null, Formatter);

        Assert.Collection(sut.Entries,
            entry => Assert.Equal(["outer"], entry.Scopes),
            entry => Assert.Equal(["outer", "inner"], entry.Scopes),
            entry => Assert.Equal(["outer"], entry.Scopes),
            entry => Assert.Empty(entry.Scopes));
    }

    // ─── Clear ──────────────────────────────────────────────────────────────

    [Fact]
    public void Clear_RemovesEntriesAndScopes() {
        var sut = new InMemoryLogger();
        using var _ = sut.BeginScope("scope-a");
        sut.Log(LogLevel.Information, default, "message", exception: null, Formatter);

        sut.Clear();

        Assert.Multiple(
            () => Assert.Empty(sut.Entries),
            () => Assert.Empty(sut.ActiveScopes),
            () => Assert.Empty(sut.CapturedScopes)
        );
    }

    // ─── Verify* ────────────────────────────────────────────────────────────

    [Fact]
    public void VerifyInformation_WithMatchingCall_DoesNotThrow() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Information, default, "hello", exception: null, Formatter);

        sut.VerifyInformation();
    }

    [Fact]
    public void VerifyInformation_WithNoMatchingCall_Throws() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Warning, default, "hello", exception: null, Formatter);

        var exception = Assert.Throws<MockVerificationException>(() => sut.VerifyInformation());
        Assert.Multiple(
            () => Assert.Equal(1, exception.ExpectedCallsNumber),
            () => Assert.Equal(0, exception.ActualCallsNumber)
        );
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void VerifyLevel_MatchesOnlyItsOwnLevel(LogLevel level) {
        var sut = new InMemoryLogger();
        foreach (var otherLevel in Enum.GetValues<LogLevel>()) {
            if (otherLevel == LogLevel.None || otherLevel == level) { continue; }

            sut.Log(otherLevel, default, "noise", exception: null, Formatter);
        }

        sut.Log(level, default, "the one", exception: null, Formatter);

        Verify(sut, level, assertMessage: null, times: 1, exactly: true);
    }

    [Fact]
    public void VerifyWithAssertMessage_FiltersByMessageContent() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Error, default, "db connection failed", exception: null, Formatter);
        sut.Log(LogLevel.Error, default, "cache miss", exception: null, Formatter);

        sut.VerifyError(message => message.Contains("db"));
    }

    [Fact]
    public void VerifyWithAssertMessage_WhenNoneMatch_Throws() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Error, default, "cache miss", exception: null, Formatter);

        Assert.Throws<MockVerificationException>(() => sut.VerifyError(message => message.Contains("db")));
    }

    [Fact]
    public void VerifyWithExactly_WhenCountDiffers_Throws() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Warning, default, "first", exception: null, Formatter);
        sut.Log(LogLevel.Warning, default, "second", exception: null, Formatter);

        var exception = Assert.Throws<MockVerificationException>(() => sut.VerifyWarning(times: 1, exactly: true));
        Assert.Equal(2, exception.ActualCallsNumber);
    }

    [Fact]
    public void VerifyWithAtLeast_WhenCountMeetsMinimum_DoesNotThrow() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Warning, default, "first", exception: null, Formatter);
        sut.Log(LogLevel.Warning, default, "second", exception: null, Formatter);

        sut.VerifyWarning(times: 1, exactly: false);
    }

    [Fact]
    public void VerifyWithAtLeast_WhenCountBelowMinimum_Throws() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Warning, default, "first", exception: null, Formatter);

        Assert.Throws<MockVerificationException>(() => sut.VerifyWarning(times: 2, exactly: false));
    }

    [Fact]
    public void Verify_ReturnsSameInstance_ForChaining() {
        var sut = new InMemoryLogger();
        sut.Log(LogLevel.Information, default, "hello", exception: null, Formatter);

        var result = sut.VerifyInformation();

        Assert.Same(sut, result);
    }

    private static void Verify(InMemoryLogger sut, LogLevel level, Func<string, bool>? assertMessage, int times, bool exactly) {
        switch (level) {
            case LogLevel.Trace: sut.VerifyTrace(assertMessage, times, exactly); break;
            case LogLevel.Debug: sut.VerifyDebug(assertMessage, times, exactly); break;
            case LogLevel.Information: sut.VerifyInformation(assertMessage, times, exactly); break;
            case LogLevel.Warning: sut.VerifyWarning(assertMessage, times, exactly); break;
            case LogLevel.Error: sut.VerifyError(assertMessage, times, exactly); break;
            case LogLevel.Critical: sut.VerifyCritical(assertMessage, times, exactly); break;
            default: throw new ArgumentOutOfRangeException(nameof(level));
        }
    }

    // ─── Generic category ─────────────────────────────────────────────────────

    [Fact]
    public void GenericLogger_IsAssignableToClosedLoggerInterface() {
        var sut = new InMemoryLogger<InMemoryLoggerTests>();

        ILogger<InMemoryLoggerTests> logger = sut;
        logger.Log(LogLevel.Information, default, "typed", exception: null, Formatter);

        Assert.Single(sut.Entries);
    }
}
