namespace Nameless;

public class VersionExtensionsTests {
    // ─── ToSemanticVersion ───────────────────────────────────────────────────

    [Fact]
    public void ToSemanticVersion_WithStandardVersion_ReturnsSemVer() {
        // arrange
        var version = new Version(1, 2, 3);

        // act
        var semVer = version.ToSemanticVersion();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(1, semVer.Major);
            Assert.Equal(2, semVer.Minor);
            Assert.Equal(3, semVer.Patch);
        });
    }

    [Fact]
    public void ToSemanticVersion_WithZeroVersion_ReturnsZeroSemVer() {
        // arrange
        var version = new Version(0, 0, 0);

        // act
        var semVer = version.ToSemanticVersion();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(0, semVer.Major);
            Assert.Equal(0, semVer.Minor);
            Assert.Equal(0, semVer.Patch);
        });
    }
}
