using Nameless.Results;

namespace Nameless.Result;

public class ErrorTests {
    [Test]
    public void WhenCreateError_ThenRetrieveNewInstanceWithCorrectType() {
        // arrange & act
        foreach (var type in Enum.GetValues<ErrorType>()) {
            var error = type switch {
                ErrorType.Validation => Error.Validation(type.ToString(), new Exception(type.ToString()), new Dictionary<string, object> { { nameof(type), type} }),
                ErrorType.Missing => Error.Missing(type.ToString(), new Exception(type.ToString()), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Conflict => Error.Conflict(type.ToString(), new Exception(type.ToString()), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Failure => Error.Failure(type.ToString(), new Exception(type.ToString()), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Forbidden => Error.Forbidden(type.ToString(), new Exception(type.ToString()), new Dictionary<string, object> { { nameof(type), type } }),
                ErrorType.Unauthorized => Error.Unauthorized(type.ToString(), new Exception(type.ToString()), new Dictionary<string, object> { { nameof(type), type } }),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Missing ErrorType case")
            };

            // assert
            Assert.Multiple(() => {
                Assert.That(error.Description, Is.EqualTo(type.ToString()));
                Assert.That(error.Type, Is.EqualTo(type));
                Assert.That(error.Exception, Is.Not.Null);
                Assert.That(error.Exception.Message, Is.EqualTo(type.ToString()));
                Assert.That(error.Metadata, Is.Not.Empty);
                Assert.That(error.Metadata.First().Value, Is.EqualTo(type));
            });
        }
    }
}