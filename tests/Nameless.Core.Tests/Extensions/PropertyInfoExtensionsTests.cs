using Nameless.Fixtures;

namespace Nameless;

public class PropertyInfoExtensionsTests {
    [Fact]
    public void When_Property_Has_Valid_DescriptionAttribute_Then_Return_Description() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        const string Expected = "Name of something";

        // act
        var actual = instance.GetPropertyWithDescriptionAttribute().GetDescription();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void When_Property_Does_Not_Have_DescriptionAttribute_And_FallbackToName_Is_True_Then_Return_PropertyName() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        const string Expected = nameof(PropertyAnnotatedClass.LastName);

        // act
        var actual = instance.GetPropertyWithoutDescriptionAttribute().GetDescription();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void
        When_Property_Has_DescriptionAttribute_But_Is_Empty_And_FallbackToName_Is_True_Then_Return_PropertyName() {
        // arrange
        var instance = new PropertyAnnotatedClass();
        const string Expected = nameof(PropertyAnnotatedClass.Age);

        // act
        var actual = instance.GetPropertyWithEmptyDescriptionAttribute().GetDescription();

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void
        When_Property_Has_DescriptionAttribute_But_Is_Empty_And_FallbackToName_Is_False_Then_Return_String_Empty() {
        // arrange
        var instance = new PropertyAnnotatedClass();

        // act
        var actual = instance.GetPropertyWithEmptyDescriptionAttribute().GetDescription(false);

        // assert
        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void
        When_Property_Does_Not_Have_DescriptionAttribute_And_FallbackToName_Is_False_Then_Return_String_Empty() {
        // arrange
        var instance = new PropertyAnnotatedClass();

        // act
        var actual = instance.GetPropertyWithoutDescriptionAttribute().GetDescription(false);

        // assert
        Assert.Equal(string.Empty, actual);
    }
}