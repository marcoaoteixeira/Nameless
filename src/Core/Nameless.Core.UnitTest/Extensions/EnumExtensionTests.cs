using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace Nameless;

public class EnumExtensionTests {
    public enum Status {
        [Description("Okey-Dokey")]
        OK,
        [Description("Oh No")]
        Error,
        Fatal
    }

    [Test]
    public void GetAttribute_Returns_Attribute_If_Exists() {
        // arrange
        var status = Status.OK;

        // act
        var attr = EnumExtension.GetAttribute<DescriptionAttribute>(status);

        // assert
        Assert.That(attr, Is.InstanceOf<DescriptionAttribute>());
    }

    [Test]
    public void GetAttribute_Returns_Null_If_Enum_Field_Does_Not_Exists() {
        // arrange
        var status = (Status)16;

        // act
        var attr = EnumExtension.GetAttribute<DescriptionAttribute>(status);

        // assert
        Assert.That(attr, Is.Null);
    }

    [Test]
    public void GetAttribute_Returns_Null_If_Attributes_Does_Not_Exists() {
        // arrange
        var status = Status.OK;

        // act
        var attr = EnumExtension.GetAttribute<AttributeUsageAttribute>(status);

        // assert
        Assert.That(attr, Is.Null);
    }

    [Test]
    public void GetDescription_Returns_Value_From_DescriptionAttribute() {
        // arrange
        var status = Status.OK;
        var expected = "Okey-Dokey";

        // act
        var actual = EnumExtension.GetDescription(status);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void GetDescription_Returns_Enum_Name_If_Attribute_Does_Not_Exists() {
        // arrange
        var status = Status.Fatal;

        // act
        var actual = EnumExtension.GetDescription(status);

        // assert
        Assert.That(actual, Is.EqualTo(nameof(Status.Fatal)));
    }
}