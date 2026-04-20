using Nameless.Attributes;

namespace Nameless;

public class ConfigurationSectionNameAttributeTests {
    // ─── GetSectionName<T>() ─────────────────────────────────────────────────

    [Fact]
    public void GetSectionNameGeneric_TypeWithAttribute_ReturnsAttributeSpecifiedName() {
        // act
        var name = ConfigurationSectionNameAttribute.GetSectionName<TypeWithAttribute>();

        // assert
        Assert.Equal("Custom", name);
    }

    [Fact]
    public void GetSectionNameGeneric_TypeWithoutAttribute_ReturnsTypeName() {
        // act
        var name = ConfigurationSectionNameAttribute.GetSectionName<TypeWithoutAttribute>();

        // assert
        Assert.Equal(nameof(TypeWithoutAttribute), name);
    }

    // ─── GetSectionName(Type) ────────────────────────────────────────────────

    [Fact]
    public void GetSectionName_TypeWithAttribute_ReturnsAttributeSpecifiedName() {
        // act
        var name = ConfigurationSectionNameAttribute.GetSectionName(typeof(TypeWithAttribute));

        // assert
        Assert.Equal("Custom", name);
    }

    [Fact]
    public void GetSectionName_TypeWithoutAttribute_ReturnsTypeName() {
        // act
        var name = ConfigurationSectionNameAttribute.GetSectionName(typeof(TypeWithoutAttribute));

        // assert
        Assert.Equal(nameof(TypeWithoutAttribute), name);
    }

    // ─── Constructor validation ───────────────────────────────────────────────

    [Fact]
    public void Constructor_WithNullName_Throws() {
        // act & assert
        Assert.ThrowsAny<Exception>(() => new ConfigurationSectionNameAttribute(null!));
    }

    [Fact]
    public void Constructor_WithWhitespaceName_Throws() {
        // act & assert
        Assert.ThrowsAny<Exception>(() => new ConfigurationSectionNameAttribute("   "));
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    [ConfigurationSectionName("Custom")]
    private sealed class TypeWithAttribute { }

    private sealed class TypeWithoutAttribute { }
}
