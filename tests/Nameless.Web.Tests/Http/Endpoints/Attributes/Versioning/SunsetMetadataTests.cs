using Nameless.Testing.Tools.Attributes;

namespace Nameless.Web.Http.Endpoints.Attributes.Versioning;

[UnitTest]
public sealed class SunsetMetadataTests {
    [Fact]
    public void WhenCreatedWithDateTimeOffset_ThenSunsetDateIsStored() {
        var date = new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero);
        var metadata = new SunsetMetadata(date);

        Assert.Equal(date, metadata.SunsetDate);
    }

    [Fact]
    public void WhenCreatedWithValidDateString_ThenSunsetDateIsParsed() {
        var metadata = new SunsetMetadata("2025-12-31T00:00:00+00:00");

        Assert.Equal(2025, metadata.SunsetDate.Year);
        Assert.Equal(12, metadata.SunsetDate.Month);
        Assert.Equal(31, metadata.SunsetDate.Day);
    }

    [Fact]
    public void WhenCreatedWithInvalidDateString_ThenThrowsFormatException() {
        Assert.Throws<FormatException>(() => new SunsetMetadata("not-a-date"));
    }

    [Fact]
    public void WhenTwoInstancesHaveSameSunsetDate_ThenTheyAreEqual() {
        var date = new DateTimeOffset(2025, 12, 31, 0, 0, 0, TimeSpan.Zero);
        var first = new SunsetMetadata(date);
        var second = new SunsetMetadata(date);

        Assert.Equal(first, second);
    }
}
