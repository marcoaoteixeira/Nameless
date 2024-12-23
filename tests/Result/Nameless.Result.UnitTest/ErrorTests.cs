namespace Nameless.Result;

public class ErrorTests {
    [Test]
    public void WhenCreateError_ThenRetrieveNewInstanceWithCorrectType() {
        // arrange & act
        foreach (var type in Enum.GetValues<ErrorType>()) {
            var error = type switch {
                ErrorType.Validation => Error.Validation(nameof(ErrorType.Validation), new Dictionary<string, object> { { nameof(type), type} }),
                ErrorType.Missing => Error.Missing(nameof(ErrorType.Missing), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Conflict => Error.Conflict(nameof(ErrorType.Conflict), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Failure => Error.Failure(nameof(ErrorType.Failure), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Forbidden => Error.Forbidden(nameof(ErrorType.Forbidden), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Unauthorized => Error.Unauthorized(nameof(ErrorType.Unauthorized), new Dictionary<string, object> { { nameof(type), type } }),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Missing ErrorType case")
            };

            // assert
            Assert.Multiple(() => {
                Assert.That(error.Description, Is.EqualTo(type.ToString()));
                Assert.That(error.Type, Is.EqualTo(type));
                Assert.That(error.Metadata, Is.Not.Empty);
                Assert.That(error.Metadata.First().Value, Is.EqualTo(type));
            });
        }
    }
}