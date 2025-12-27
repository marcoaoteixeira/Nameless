using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Nameless;

public class EnumExtensionsTests {
    public enum Status {
        [Description(description: "Okey-Dokey")]
        Ok,

        [Description(description: "Oh No")]
        Error,
        Fatal
    }

    [Fact]
    public void GetAttribute_Returns_Attribute_If_Exists() {
        // arrange
        var status = Status.Ok;

        // act
        var attr = status.GetAttribute<DescriptionAttribute>();

        // assert
        Assert.IsType<DescriptionAttribute>(attr);
    }

    [Fact]
    public void GetAttribute_Returns_Null_If_Enum_Field_Does_Not_Exists() {
        // arrange
        var status = (Status)16;

        // act
        var attr = status.GetAttribute<DescriptionAttribute>();

        // assert
        Assert.Null(attr);
    }

    [Fact]
    public void GetAttribute_Returns_Null_If_Attributes_Does_Not_Exists() {
        // arrange
        var status = Status.Ok;

        // act
        var attr = status.GetAttribute<AttributeUsageAttribute>();

        // assert
        Assert.Null(attr);
    }

    [Fact]
    public void GetDescription_Returns_Value_From_DescriptionAttribute() {
        // arrange
        var status = Status.Ok;
        var expected = "Okey-Dokey";

        // act
        var actual = status.GetDescription();

        // assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetDescription_Returns_Enum_Name_If_Attribute_Does_Not_Exists() {
        // arrange
        var status = Status.Fatal;

        // act
        var actual = status.GetDescription();

        // assert
        Assert.Equal(nameof(Status.Fatal), actual);
    }
}