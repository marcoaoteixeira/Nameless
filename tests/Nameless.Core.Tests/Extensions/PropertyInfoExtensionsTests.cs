using System.ComponentModel;
using System.Reflection;

namespace Nameless;

public class PropertyInfoExtensionsTests {
    // ─── GetDescription ──────────────────────────────────────────────────────

    [Fact]
    public void GetDescription_WhenDescriptionAttributePresent_ReturnsDescription() {
        // arrange
        var property = typeof(SampleClass).GetProperty(nameof(SampleClass.WithDescription))!;

        // act
        var description = property.GetDescription();

        // assert
        Assert.Equal("A described property", description);
    }

    [Fact]
    public void GetDescription_WhenNoAttribute_FallbackTrue_ReturnsPropertyName() {
        // arrange
        var property = typeof(SampleClass).GetProperty(nameof(SampleClass.WithoutDescription))!;

        // act
        var description = property.GetDescription(fallbackToName: true);

        // assert
        Assert.Equal(nameof(SampleClass.WithoutDescription), description);
    }

    [Fact]
    public void GetDescription_WhenNoAttribute_FallbackFalse_ReturnsEmpty() {
        // arrange
        var property = typeof(SampleClass).GetProperty(nameof(SampleClass.WithoutDescription))!;

        // act
        var description = property.GetDescription(fallbackToName: false);

        // assert
        Assert.Equal(string.Empty, description);
    }

    [Fact]
    public void GetDescription_WhenEmptyDescription_FallbackTrue_ReturnsPropertyName() {
        // arrange
        var property = typeof(SampleClass).GetProperty(nameof(SampleClass.WithEmptyDescription))!;

        // act
        var description = property.GetDescription(fallbackToName: true);

        // assert
        Assert.Equal(nameof(SampleClass.WithEmptyDescription), description);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class SampleClass {
        [Description("A described property")]
        public string WithDescription { get; set; } = string.Empty;

        public string WithoutDescription { get; set; } = string.Empty;

        [Description("")]
        public string WithEmptyDescription { get; set; } = string.Empty;
    }
}
