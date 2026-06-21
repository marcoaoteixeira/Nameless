namespace Nameless.ComponentModel;

public class AssemblyTypeConverterTests {
    private readonly AssemblyTypeConverter _converter = new();

    // --- CanConvertFrom ---

    [Fact]
    public void CanConvertFrom_WithStringType_ReturnsTrue() {
        // act
        var result = _converter.CanConvertFrom(context: null, typeof(string));

        // assert
        Assert.True(result);
    }

    [Fact]
    public void CanConvertFrom_WithIntType_ReturnsFalse() {
        // act
        var result = _converter.CanConvertFrom(context: null, typeof(int));

        // assert
        Assert.False(result);
    }

    // --- ConvertTo ---

    [Fact]
    public void ConvertTo_WithNonStringValue_ReturnsNull() {
        // act
        var result = _converter.ConvertTo(context: null, culture: null, value: 42, typeof(System.Reflection.Assembly));

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertTo_WithValidAssemblyName_ReturnsAssembly() {
        // arrange
        var assemblyName = typeof(object).Assembly.FullName!;

        // act
        var result = _converter.ConvertTo(context: null, culture: null, value: assemblyName, typeof(System.Reflection.Assembly));

        // assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ConvertTo_WithInvalidAssemblyName_ReturnsNull() {
        // act
        var result = _converter.ConvertTo(context: null, culture: null, value: "NonExistent.Assembly.XYZ", typeof(System.Reflection.Assembly));

        // assert
        Assert.Null(result);
    }
}
