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
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Missing ErrorType case")
            };

            // assert
            Assert.Multiple(() => {
                Assert.That(error.Description, Is.EqualTo(type.ToString()));
                Assert.That(error.Type, Is.EqualTo(type));
            });
        }
    }
}