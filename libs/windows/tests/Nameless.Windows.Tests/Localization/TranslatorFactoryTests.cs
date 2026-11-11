using System.ComponentModel;
using System.Globalization;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="TranslatorFactory"/>'s section resolution and
///     per-section caching (ADR-003).
/// </summary>
[UnitTest]
public class TranslatorFactoryTests {
    private sealed class FakeTranslator : ITranslator {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public string this[string key] => key;

        public string this[string key, params object[] parameters] => key;

        public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) { }

        public void SetCulture(string cultureName) { }
    }

    [Fact]
    public void Constructor_WhenRootIsNull_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new TranslatorFactory(root: null!));
    }

    [Fact]
    public void Create_ByType_ResolvesTheMappedSection() {
        // arrange
        var sut = new TranslatorFactory(new FakeTranslator());

        // act
        var result = sut.Create(typeof(TranslatorFactoryTests))["Key"];

        // assert
        Assert.Equal("TranslatorFactoryTests_Key", result);
    }

    [Fact]
    public void Create_BySameSectionTwice_ReturnsTheSameCachedInstance() {
        // arrange: proves repeated resolution for the same section cannot
        // leak a new subscription on the root — see SectionTranslatorTests
        // for the subscription-leak assertion itself.
        var sut = new TranslatorFactory(new FakeTranslator());

        // act
        var first = sut.Create("Section");
        var second = sut.Create("Section");

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void Create_ByTypeAndBySection_ReturnsTheSameCachedInstance() {
        // arrange
        var sut = new TranslatorFactory(new FakeTranslator());

        // act
        var byType = sut.Create(typeof(TranslatorFactoryTests));
        var bySection = sut.Create(nameof(TranslatorFactoryTests));

        // assert
        Assert.Same(byType, bySection);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_BySection_WhenSectionIsNullEmptyOrWhiteSpace_Throws(string? section) {
        // arrange
        var sut = new TranslatorFactory(new FakeTranslator());

        // act & assert: ArgumentNullException for null, ArgumentException
        // for empty/white-space — both derive from ArgumentException.
        Assert.ThrowsAny<ArgumentException>(() => sut.Create(section!));
    }

    [Fact]
    public void Create_ByType_WhenAnchorIsNull_Throws() {
        // arrange
        var sut = new TranslatorFactory(new FakeTranslator());

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.Create(anchor: null!));
    }

    [Fact]
    public void CreateOfT_ReturnsATranslatorOfTScopedToItsSection() {
        // arrange
        var sut = new TranslatorFactory(new FakeTranslator());

        // act
        var result = sut.Create<TranslatorFactoryTests>();

        // assert
        Assert.IsType<Translator<TranslatorFactoryTests>>(result);
        Assert.Equal("TranslatorFactoryTests_Key", result["Key"]);
    }
}
