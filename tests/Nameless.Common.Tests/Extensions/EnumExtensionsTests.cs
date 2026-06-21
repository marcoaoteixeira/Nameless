using System.ComponentModel;

namespace Nameless.Extensions;

public class EnumExtensionsTests {
    // ─── GetAttribute ────────────────────────────────────────────────────────

    [Fact]
    public void GetAttribute_WhenAttributePresent_ReturnsAttribute() {
        // act
        var attr = TestEnum.WithDescription.GetAttribute<DescriptionAttribute>();

        // assert
        Assert.NotNull(attr);
        Assert.Equal("Human-readable value", attr.Description);
    }

    [Fact]
    public void GetAttribute_WhenAttributeAbsent_ReturnsNull() {
        // act
        var attr = TestEnum.NoDescription.GetAttribute<DescriptionAttribute>();

        // assert
        Assert.Null(attr);
    }

    // ─── GetDescription ─────────────────────────────────────────────────────

    [Fact]
    public void GetDescription_WhenDescriptionAttributePresent_ReturnsDescription() {
        // act
        var description = TestEnum.WithDescription.GetDescription();

        // assert
        Assert.Equal("Human-readable value", description);
    }

    [Fact]
    public void GetDescription_WhenNoAttribute_FallbackTrue_ReturnsEnumName() {
        // act
        var description = TestEnum.NoDescription.GetDescription(fallbackToString: true);

        // assert
        Assert.Equal(nameof(TestEnum.NoDescription), description);
    }

    [Fact]
    public void GetDescription_WhenNoAttribute_FallbackFalse_ReturnsEmpty() {
        // act
        var description = TestEnum.NoDescription.GetDescription(fallbackToString: false);

        // assert
        Assert.Equal(string.Empty, description);
    }

    [Fact]
    public void GetDescription_WhenEmptyDescription_FallbackTrue_ReturnsEnumName() {
        // act
        var description = TestEnum.EmptyDescription.GetDescription(fallbackToString: true);

        // assert
        Assert.Equal(nameof(TestEnum.EmptyDescription), description);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private enum TestEnum {
        [Description("Human-readable value")]
        WithDescription,

        NoDescription,

        [Description("")]
        EmptyDescription
    }
}
