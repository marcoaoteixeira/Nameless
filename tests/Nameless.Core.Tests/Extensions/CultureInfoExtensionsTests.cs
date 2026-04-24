using System.Globalization;

namespace Nameless.Extensions;

public class CultureInfoExtensionsTests {
    // ─── GetParents ─────────────────────────────────────────────────────────

    [Fact]
    public void GetParents_WithSpecificCulture_IncludesSelfAndParents() {
        // arrange
        var culture = new CultureInfo("en-US");

        // act
        var parents = culture.GetParents().ToList();

        // assert
        Assert.Multiple(() => {
            Assert.NotEmpty(parents);
            Assert.Contains(parents, c => c.Name == "en-US");
        });
    }

    [Fact]
    public void GetParents_WithSpecificCulture_IncludesNeutralParent() {
        // arrange
        var culture = new CultureInfo("en-US");

        // act
        var parents = culture.GetParents().ToList();

        // assert
        Assert.Contains(parents, c => c.Name == "en");
    }

    [Fact]
    public void GetParents_WithNeutralCulture_ReturnsCultureOnly() {
        // arrange
        var culture = new CultureInfo("en");

        // act
        var parents = culture.GetParents().ToList();

        // assert
        Assert.Single(parents);
        Assert.Equal("en", parents[0].Name);
    }
}
