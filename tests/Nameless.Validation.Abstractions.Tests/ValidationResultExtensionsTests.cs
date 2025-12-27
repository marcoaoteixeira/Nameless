namespace Nameless.Validation;

public class ValidationResultExtensionsTests {
    [Fact]
    public void WhenCreatingDictionary_WithSucceededResult_ThenReturnsEmptyDictionary() {
        // arrange
        var result = ValidationResult.Success();

        // act
        var dictionary = result.ToDictionary();

        // assert
        Assert.Empty(dictionary);
    }

    [Fact]
    public void WhenCreatingDictionary_WithFailureResult_ThenReturnsErrorDictionary() {
        // arrange
        const string ErrorMessage = "Error Message";
        const string MemberName = "Member Name";
        var error = new ValidationError(ErrorMessage, code: "Error Code", MemberName);
        var result = ValidationResult.Failure([error]);

        // act
        var dictionary = result.ToDictionary();

        // assert
        Assert.Multiple(() => {
            Assert.Single(dictionary);
            Assert.Contains(dictionary, item => item.Key == MemberName && item.Value.Contains(ErrorMessage));
        });
    }

    [Fact]
    public void WhenCreatingDictionary_WithMultipleFailureResult_ThenReturnsErrorDictionaryGroupedByMemberName() {
        // arrange
        const string ErrorMessage = "Error Message";
        const string MemberNameX = "Member Name X";
        const string MemberNameY = "Member Name Y";
        var error = new ValidationError(ErrorMessage, code: "Error Code", MemberNameX);
        var result = ValidationResult.Failure([
            new ValidationError(ErrorMessage, code: "Error Code", MemberNameX),
            new ValidationError(ErrorMessage, code: "Error Code", MemberNameX),
            new ValidationError(ErrorMessage, code: "Error Code", MemberNameY)
        ]);

        // act
        var dictionary = result.ToDictionary();

        // assert
        Assert.Multiple(() => {
            Assert.Contains(dictionary, item => item.Key == MemberNameX && item.Value.Length == 2);
            Assert.Contains(dictionary, item => item.Key == MemberNameY && item.Value.Length == 1);
        });
    }
}