using System.ComponentModel;

namespace Nameless;

public class ObjectExtensionsTests {
    // ─── IsAnonymous ─────────────────────────────────────────────────────────

    [Fact]
    public void IsAnonymous_WithAnonymousObject_ReturnsTrue() {
        // arrange
        var obj = new { Name = "test", Value = 42 };

        // act & assert
        Assert.True(obj.IsAnonymous());
    }

    [Fact]
    public void IsAnonymous_WithNamedClass_ReturnsFalse() {
        // arrange
        var obj = new NamedClass();

        // act & assert
        Assert.False(obj.IsAnonymous());
    }

    [Fact]
    public void IsAnonymous_WithStringObject_ReturnsFalse() {
        // arrange
        object obj = "a string";

        // act & assert
        Assert.False(obj.IsAnonymous());
    }

    // ─── HasAttribute ────────────────────────────────────────────────────────

    [Fact]
    public void HasAttribute_WhenTypeHasAttribute_ReturnsTrue() {
        // arrange
        var obj = new AnnotatedClass();

        // act & assert
        Assert.True(obj.HasAttribute<DescriptionAttribute>());
    }

    [Fact]
    public void HasAttribute_WhenTypeDoesNotHaveAttribute_ReturnsFalse() {
        // arrange
        var obj = new NamedClass();

        // act & assert
        Assert.False(obj.HasAttribute<DescriptionAttribute>());
    }

    [Fact]
    public void HasAttribute_WithInheritTrue_FindsInheritedAttribute() {
        // arrange
        var obj = new DerivedAnnotatedClass();

        // act & assert
        Assert.True(obj.HasAttribute<DescriptionAttribute>(inherit: true));
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class NamedClass { }

    [Description("annotated")]
    private class AnnotatedClass { }

    private sealed class DerivedAnnotatedClass : AnnotatedClass { }
}
