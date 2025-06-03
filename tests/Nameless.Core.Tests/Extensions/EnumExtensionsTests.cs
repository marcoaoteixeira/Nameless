using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Nameless;

public class EnumExtensionsTests {
    public enum Status {
        [Description("Okey-Dokey")] OK,
        [Description("Oh No")] Error,
        Fatal
    }

    [Fact]
    public void GetAttribute_Returns_Attribute_If_Exists() {
        // arrange
        var status = Status.OK;

        // act
        var attr = status.GetAttribute<DescriptionAttribute>();

        // assert
        Assert.That(attr, Is.InstanceOf<DescriptionAttribute>());
    }

    [Fact]
    public void GetAttribute_Returns_Null_If_Enum_Field_Does_Not_Exists() {
        // arrange
        var status = (Status)16;

        // act
        var attr = status.GetAttribute<DescriptionAttribute>();

        // assert
        Assert.That(attr, Is.Null);
    }

    [Fact]
    public void GetAttribute_Returns_Null_If_Attributes_Does_Not_Exists() {
        // arrange
        var status = Status.OK;

        // act
        var attr = status.GetAttribute<AttributeUsageAttribute>();

        // assert
        Assert.That(attr, Is.Null);
    }

    [Fact]
    public void GetDescription_Returns_Value_From_DescriptionAttribute() {
        // arrange
        var status = Status.OK;
        var expected = "Okey-Dokey";

        // act
        var actual = status.GetDescription();

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Fact]
    public void GetDescription_Returns_Enum_Name_If_Attribute_Does_Not_Exists() {
        // arrange
        var status = Status.Fatal;

        // act
        var actual = status.GetDescription();

        // assert
        Assert.That(actual, Is.EqualTo(nameof(Status.Fatal)));
    }
}