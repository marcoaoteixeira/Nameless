namespace Nameless;

public class PreventArgumentNullStructTests {
    [Fact]
    public void WhenNonNullParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        Assert.DoesNotThrow(() => Prevent.Argument.Null<int>(1));
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(null));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(null, message: message));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(message));
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() {
        // arrange & act & assert
        Assert.Throws<InvalidOperationException>(() =>
            Prevent.Argument.Null<int>(null, exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        int? value = null;

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null(value));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(nameof(value)));
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        const string paramName = "test_parameter";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(null, paramName));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(paramName));
        });
    }
}