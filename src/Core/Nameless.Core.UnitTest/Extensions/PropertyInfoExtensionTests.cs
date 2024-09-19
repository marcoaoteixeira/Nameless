using Nameless.Fixtures;

namespace Nameless;

public class PropertyInfoExtensionTests {
    [Test]
    public void When_Property_Has_Valid_DescriptionAttribute_Then_Return_Description() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        const string expected = "Name of something";

        // act
        var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithDescriptionAttribute());

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void When_Property_Does_Not_Have_DescriptionAttribute_And_FallbackToName_Is_True_Then_Return_PropertyName() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        const string expected = nameof(PropertyAnnotatedClass.LastName);

        // act
        var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithoutDescriptionAttribute());

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void When_Property_Has_DescriptionAttribute_But_Is_Empty_And_FallbackToName_Is_True_Then_Return_PropertyName() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        const string expected = nameof(PropertyAnnotatedClass.Age);

        // act
        var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithEmptyDescriptionAttribute());

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void When_Property_Has_DescriptionAttribute_But_Is_Empty_And_FallbackToName_Is_False_Then_Return_String_Empty() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        
        // act
        var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithEmptyDescriptionAttribute(), fallbackToName: false);

        // assert
        Assert.That(actual, Is.EqualTo(string.Empty));
    }

    [Test]
    public void When_Property_Does_Not_Have_DescriptionAttribute_And_FallbackToName_Is_False_Then_Return_String_Empty() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        
        // act
        var actual = PropertyInfoExtension.GetDescription(instance.GetPropertyWithoutDescriptionAttribute(), fallbackToName: false);

        // assert
        Assert.That(actual, Is.EqualTo(string.Empty));
    }
}