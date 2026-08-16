using Nameless.Generators.Web.Http.Endpoints.Models;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Generators.Web.Http.Endpoints.Infrastructure;

[UnitTest]
public class VersionModelTests {
    [Fact]
    public void V1_ReturnsVersionWithMajorOne()
    {
        var v = VersionModel.V1;

        Assert.Equal(1, v.Major);
        Assert.Null(v.Minor);
        Assert.Null(v.Status);
    }

    [Theory]
    [InlineData("1",        1, null, null)]
    [InlineData("2",        2, null, null)]
    [InlineData("1.0",      1, 0,    null)]
    [InlineData("2.5",      2, 5,    null)]
    [InlineData("1.0-beta", 1, 0,    "beta")]
    [InlineData("3.1-rc2",  3, 1,    "rc2")]
    public void TryParse_ValidInput_ReturnsTrue(string input, int major, int? minor, string? status)
    {
        var parsed = VersionModel.TryParse(input, out var result);

        Assert.True(parsed);
        Assert.Equal(major, result.Major);
        Assert.Equal(minor, result.Minor);
        Assert.Equal(status, result.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("abc")]
    [InlineData("1.2.3")]
    [InlineData("-1")]
    public void TryParse_InvalidInput_ReturnsFalse(string? input)
    {
        var parsed = VersionModel.TryParse(input, out _);

        Assert.False(parsed);
    }

    [Theory]
    [InlineData(1, null, null,   "1")]
    [InlineData(2, 0,    null,   "2.0")]
    [InlineData(1, 0,    "beta", "1.0-beta")]
    [InlineData(3, 5,    "rc1",  "3.5-rc1")]
    public void Format_ReturnsCorrectString(int major, int? minor, string? status, string expected)
    {
        var version = new VersionModel(major, minor, status);

        Assert.Equal(expected, version.Format());
    }
}
