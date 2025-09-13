namespace Nameless.Validation;

public class ValidateAttributeTests {
    [Fact]
    public void WhenCheckIsPresent_WhenTypeIsAnnotatedWithValidateAttribute_ThenReturnsTrue() {
        // arrange
        var entity = new EntityWithValidate();

        // act
        var actual = ValidateAttribute.IsPresent(entity);

        // assert
        Assert.True(actual);
    }

    [Fact]
    public void WhenCheckIsPresent_WhenTypeIsNotAnnotatedWithValidateAttribute_ThenReturnsFalse() {
        // arrange
        var entity = new EntityWithoutValidate();

        // act
        var actual = ValidateAttribute.IsPresent(entity);

        // assert
        Assert.False(actual);
    }

    [Validate]
    public record EntityWithValidate;

    public record EntityWithoutValidate;
}