namespace Nameless.Reporting;

[UnitTest]
public class StatusUpdateTests {
    [Fact]
    public void Constructor_WithMinimalArguments_UsesDefaults() {
        // act
        var sut = new StatusUpdate("svc", "hello");

        // assert
        Assert.Multiple(
            () => Assert.Equal("svc", sut.ServiceName),
            () => Assert.Equal("hello", sut.Message),
            () => Assert.Null(sut.ChannelKey),
            () => Assert.Equal(StatusLevel.Info, sut.Level),
            () => Assert.Equal(default, sut.Timestamp),
            () => Assert.Empty(sut.Metadata)
        );
    }

    [Fact]
    public void Constructor_WithAllArguments_SetsProperties() {
        // arrange
        var timestamp = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);

        // act
        var sut = new StatusUpdate("svc", "hello", "channel", StatusLevel.Warning, timestamp, new Dictionary<string, string> { ["k"] = "v" });

        // assert
        Assert.Multiple(
            () => Assert.Equal("channel", sut.ChannelKey),
            () => Assert.Equal(StatusLevel.Warning, sut.Level),
            () => Assert.Equal(timestamp, sut.Timestamp),
            () => Assert.Equal("v", sut.Metadata["k"])
        );
    }

    [Theory]
    [InlineData(null, "message")]
    [InlineData(" ", "message")]
    [InlineData("svc", null)]
    [InlineData("svc", " ")]
    public void Constructor_WithBlankRequiredArguments_Throws(string? serviceName, string? message) {
        // act & assert
        Assert.ThrowsAny<ArgumentException>(() => new StatusUpdate(serviceName!, message!));
    }

    [Theory]
    [InlineData(StatusLevel.Info)]
    [InlineData(StatusLevel.Warning)]
    [InlineData(StatusLevel.Error)]
    public void WhenLevel_ConstructedWithGivenValue_ThenExposesThatSameValue(StatusLevel level) {
        var update = new StatusUpdate("Worker", "Something happened", level: level, timestamp: DateTimeOffset.UtcNow);

        Assert.Equal(level, update.Level);
    }

    [Fact]
    public void Deconstruct_ReturnsAllProperties() {
        // arrange
        var timestamp = DateTimeOffset.UnixEpoch;
        var sut = new StatusUpdate("svc", "hello", "channel", StatusLevel.Warning, timestamp);

        // act
        var (serviceName, channelKey, message, level, at, metadata) = sut;

        // assert
        Assert.Multiple(
            () => Assert.Equal("svc", serviceName),
            () => Assert.Equal("channel", channelKey),
            () => Assert.Equal("hello", message),
            () => Assert.Equal(StatusLevel.Warning, level),
            () => Assert.Equal(timestamp, at),
            () => Assert.Empty(metadata)
        );
    }
}
