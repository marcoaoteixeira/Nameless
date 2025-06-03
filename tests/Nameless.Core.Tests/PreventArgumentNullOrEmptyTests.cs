namespace Nameless;

public class PreventArgumentNullOrEmptyTests {
    private readonly IEnumerable<object> _emptyEnumerable = [];
    private readonly IEnumerable<object> _nonEmptyEnumerable = [1, 2, 3];
    private readonly IEnumerable<object> _nullEnumerable = null;

    [Fact]
    public void WhenNonNullOrEmptyParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        Assert.DoesNotThrow(() => Prevent.Argument.NullOrEmpty(_nonEmptyEnumerable));
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(_nullEnumerable));
    }

    [Fact]
    public void WhenEmptyParamValue_ThenThrowsArgumentException() {
        // arrange & act & assert
        Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(_emptyEnumerable));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() =>
                    Prevent.Argument.NullOrEmpty(_nullEnumerable, message: message));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(message));
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string message = "Parameter is empty";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentException>(() =>
                    Prevent.Argument.NullOrEmpty(_emptyEnumerable, message: message));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(message));
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Prevent.Argument.NullOrEmpty(_nullEnumerable,
                exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenEmptyParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Prevent.Argument.NullOrEmpty(_emptyEnumerable,
                exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(_nullEnumerable));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(nameof(_nullEnumerable)));
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(_emptyEnumerable));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(nameof(_emptyEnumerable)));
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        const string paramName = "test_parameter";

        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Prevent.Argument.NullOrEmpty(_nullEnumerable, paramName));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(paramName));
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        const string paramName = "test_parameter";

        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentException>(() => Prevent.Argument.NullOrEmpty(_emptyEnumerable, paramName));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Message, Contains.Substring(paramName));
        });
    }
}