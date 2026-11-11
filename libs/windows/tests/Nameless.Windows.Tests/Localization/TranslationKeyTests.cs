using Nameless.Testing.Tools.Attributes;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="TranslationKey"/>'s type-to-section mapping and key
///     composition rules (ADR-003).
/// </summary>
[UnitTest]
public class TranslationKeyTests {
    private sealed class Nested;

    [Fact]
    public void GetSection_ForPlainType_ReturnsItsSimpleName() {
        // act
        var result = TranslationKey.GetSection(typeof(TranslationKeyTests));

        // assert
        Assert.Equal(nameof(TranslationKeyTests), result);
    }

    [Fact]
    public void GetSection_ForGenericType_StripsArity() {
        // act
        var result = TranslationKey.GetSection(typeof(List<string>));

        // assert
        Assert.Equal("List", result);
    }

    [Fact]
    public void GetSection_ForNestedType_ReturnsItsOwnSimpleName() {
        // act
        var result = TranslationKey.GetSection(typeof(Nested));

        // assert
        Assert.Equal(nameof(Nested), result);
    }

    [Fact]
    public void GetSection_WhenAnchorIsNull_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => TranslationKey.GetSection(anchor: null!));
    }

    [Fact]
    public void Compose_JoinsSectionAndKeyWithSeparator() {
        // act
        var result = TranslationKey.Compose("Section", "Key");

        // assert
        Assert.Equal("Section_Key", result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Compose_WhenSectionIsNullEmptyOrWhiteSpace_Throws(string? section) {
        // act & assert: ArgumentNullException for null, ArgumentException
        // for empty/white-space — both derive from ArgumentException.
        Assert.ThrowsAny<ArgumentException>(() => TranslationKey.Compose(section!, "Key"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Compose_WhenKeyIsNullEmptyOrWhiteSpace_Throws(string? key) {
        // act & assert
        Assert.ThrowsAny<ArgumentException>(() => TranslationKey.Compose("Section", key!));
    }
}
