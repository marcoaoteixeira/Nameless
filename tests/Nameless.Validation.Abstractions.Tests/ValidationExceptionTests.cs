namespace Nameless.Validation;

public class ValidationExceptionTests {
    [Fact]
    public void WhenInitializingWithResult_ThenRetrieveValueFromProperty() {
        // arrange
        var error = new ValidationError(error: "Error Message", code: "Error Code", memberName: "Member Name");
        var result = ValidationResult.Create(error);
        var sut = new ValidationException(result);

        // act
        var actual = sut.Result;

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);
            Assert.NotEmpty(actual.Errors);
        });
    }

    [Fact]
    public void WhenInitializingWithResult_WhenResultIsNull_ThenThrowsException() {
        // arrange
        ValidationResult result = null;

        // act
        var exception = Record.Exception(() => new ValidationException(result));

        // assert
        Assert.IsType<ArgumentNullException>(exception);
    }

    [Fact]
    public void WhenInitializingWithAllParameters_ThenRetrieveAllProperties() {
        // arrange
        var error = new ValidationError(error: "Error Message", code: "Error Code", memberName: "Member Name");
        var result = ValidationResult.Create(error);
        const string Message = "Validation Error";
        var innerException = new Exception(message: "Inner Exception");

        // act
        var actual = new ValidationException(result, Message, innerException);

        // assert
        Assert.Multiple(() => {
            Assert.Equivalent(result.Errors, actual.Result.Errors);
            Assert.Equal(Message, actual.Message);
            Assert.Equal(innerException, actual.InnerException);
            Assert.Equal(innerException.Message, actual.InnerException.Message);
        });
    }

    [Fact]
    public void WhenInitializingWithoutInnerException_ThenInnerExceptionIsNull() {
        // arrange
        var error = new ValidationError(error: "Error Message", code: "Error Code", memberName: "Member Name");
        var result = ValidationResult.Create(error);
        const string Message = "Validation Error";

        // act
        var actual = new ValidationException(result, Message);

        // assert
        Assert.Multiple(() => {
            Assert.Equivalent(result.Errors, actual.Result.Errors);
            Assert.Equal(Message, actual.Message);
            Assert.Null(actual.InnerException);
        });
    }
}