namespace Nameless;

public class PreventArgumentNullClassTests {
    [Fact]
    public void WhenNonNullParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        Assert.DoesNotThrow(() => Prevent.Argument.Null<object>(1));
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<object>(null));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<object>(null, message: message));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Is.EqualTo(message));
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Prevent.Argument.Null<object>(null, exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        object user = null;

        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null(user));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(nameof(user)));
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        object user = null;
        const string paramName = "test_parameter";

        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null(user, paramName));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(paramName));
        });
    }
}