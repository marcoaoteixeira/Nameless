using Nameless.Testing.Tools.Attributes;

namespace Nameless.Reporting;

[UnitTest]
public class StatusUpdateTests {
    [Theory]
    [InlineData(StatusLevel.Info)]
    [InlineData(StatusLevel.Warning)]
    [InlineData(StatusLevel.Error)]
    public void WhenLevel_ConstructedWithGivenValue_ThenExposesThatSameValue(StatusLevel level) {
        var update = new StatusUpdate("Worker", "Something happened", level: level, timestamp: DateTimeOffset.UtcNow);

        Assert.Equal(level, update.Level);
    }
}