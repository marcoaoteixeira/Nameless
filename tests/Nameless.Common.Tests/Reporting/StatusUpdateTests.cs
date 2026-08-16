using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class StatusUpdateTests {
    [Fact]
    public void WhenEquals_ComparingRecordsWithSameValues_ThenTheyAreEqual() {
        var timestamp = DateTimeOffset.UtcNow;

        var first = new StatusUpdate("Worker", "Processing", StatusLevel.Info, timestamp);
        var second = new StatusUpdate("Worker", "Processing", StatusLevel.Info, timestamp);

        Assert.Equal(first, second);
    }

    [Fact]
    public void WhenEquals_ComparingRecordsWithDifferentMessages_ThenTheyAreNotEqual() {
        var timestamp = DateTimeOffset.UtcNow;

        var first = new StatusUpdate("Worker", "Processing", StatusLevel.Info, timestamp);
        var second = new StatusUpdate("Worker", "Idle", StatusLevel.Info, timestamp);

        Assert.NotEqual(first, second);
    }

    [Theory]
    [InlineData(StatusLevel.Info)]
    [InlineData(StatusLevel.Warning)]
    [InlineData(StatusLevel.Error)]
    public void WhenLevel_ConstructedWithGivenValue_ThenExposesThatSameValue(StatusLevel level) {
        var update = new StatusUpdate("Worker", "Something happened", level, DateTimeOffset.UtcNow);

        Assert.Equal(level, update.Level);
    }
}