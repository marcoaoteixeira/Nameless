using FluentValidation;
using FluentValidation.Results;
using Nameless.ErrorHandling;

namespace Nameless.FluentValidation {
    public class ValidationResultExtensionTests {
        [Test]
        public void Success_Should_Return_True_If_ValidationResult_Is_Valid() {
            // arrange
            var validationResult = new ValidationResult();

            // act
            var actual = ValidationResultExtension.Success(validationResult);

            // assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Failure_Should_Return_True_If_ValidationResult_Is_Not_Valid() {
            // arrange
            var validationResult = new ValidationResult([
                new ValidationFailure("Property", "Error")
            ]);

            // act
            var actual = ValidationResultExtension.Failure(validationResult);

            // assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Success_Should_Return_False_If_ValidationResult_Is_Not_Valid() {
            // arrange
            var validationResult = new ValidationResult([
                new ValidationFailure("Property", "Error")
            ]);

            // act
            var actual = ValidationResultExtension.Success(validationResult);

            // assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Failure_Should_Return_False_If_ValidationResult_Is_Valid() {
            // arrange
            var validationResult = new ValidationResult();

            // act
            var actual = ValidationResultExtension.Failure(validationResult);

            // assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void ToErrorCollection_Should_Return_ErrorCollection_Instance() {
            // arrange
            var errorCollection = new ErrorCollection(new Dictionary<string, string[]> {
                { "Property", ["Error"] }
            });
            var validationResult = new ValidationResult([
                new ValidationFailure("Property", "Error")
            ]);

            // act
            var result = ValidationResultExtension.ToErrorCollection(validationResult);

            var actual = result.First();
            var expected = errorCollection.First();

            // assert
            Assert.Multiple(() => {
                Assert.That(actual.Code, Is.EqualTo(expected.Code));
                Assert.That(actual.Problems, Is.EquivalentTo(expected.Problems));
            });
        }

        [Test]
        public void ToDictionary_Should_Return_Dictionary_Instance() {
            // arrange
            var errorDictionary = new Dictionary<string, string[]> {
                { "Property", ["Error"] }
            };
            var validationResult = new ValidationResult([
                new ValidationFailure("Property", "Error")
            ]);

            // act
            var result = ValidationResultExtension.ToDictionary(validationResult);

            var actual = result.First();
            var expected = errorDictionary.First();

            // assert
            Assert.Multiple(() => {
                Assert.That(actual.Key, Is.EqualTo(expected.Key));
                Assert.That(actual.Value, Is.EquivalentTo(expected.Value));
            });
        }

        [Test]
        public void ToDictionary_Should_Return_Dictionary_Instance_From_ValidationException() {
            // arrange
            var errorDictionary = new Dictionary<string, string[]> {
                { "Property", ["Error"] }
            };
            var validationException = new ValidationException(
                [new ValidationFailure("Property", "Error")]
            );

            // act
            var result = ValidationResultExtension.ToDictionary(validationException);

            var actual = result.First();
            var expected = errorDictionary.First();

            // assert
            Assert.Multiple(() => {
                Assert.That(actual.Key, Is.EqualTo(expected.Key));
                Assert.That(actual.Value, Is.EquivalentTo(expected.Value));
            });
        }
    }
}
