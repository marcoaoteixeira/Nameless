namespace Nameless.Results;

public class ErrorTests {
    [Fact]
    public void WhenCreateError_ThenRetrieveNewInstanceWithCorrectType() {
        // arrange & act
        foreach (var type in Enum.GetValues<ErrorType>()) {
            var error = type switch {
                ErrorType.Validation => Error.Validation(type.ToString()),
                ErrorType.Missing => Error.Missing(type.ToString()),
                ErrorType.Conflict => Error.Conflict(type.ToString()),
                ErrorType.Failure => Error.Failure(type.ToString()),
                ErrorType.Forbidden => Error.Forbidden(type.ToString()),
                ErrorType.Unauthorized => Error.Unauthorized(type.ToString()),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, message: "Missing ErrorType case")
            };

            // assert
            Assert.Multiple(() => {
                Assert.Equal(type.ToString(), error.Description);
                Assert.Equal(type, error.Type);
            });
        }
    }

    [Fact]
    public void WhenCallingParameterlessConstructor_ThenThrowsInvalidOperationException() {
        // arrange & act & assert
        Assert.Throws<InvalidOperationException>(() => new Error());
    }
}