namespace Nameless;

public class PreventArgumentNullOrEmptyTests {
    private readonly IEnumerable<object> _emptyEnumerable = [];
    private readonly IEnumerable<object> _nonEmptyEnumerable = [1, 2, 3];
    private readonly IEnumerable<object> _nullEnumerable = null;

    [Fact]
    public void WhenNonNullOrEmptyParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        var value = Throws.When.NullOrEmpty(_nonEmptyEnumerable);

        Assert.NotNull(value);
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty(_nullEnumerable));
    }

    [Fact]
    public void WhenEmptyParamValue_ThenThrowsArgumentException() {
        // arrange & act & assert
        Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(_emptyEnumerable));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string Message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() =>
                    Throws.When.NullOrEmpty(_nullEnumerable, message: Message));
            Assert.NotNull(exception);
            Assert.Contains(Message, exception.Message);
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string Message = "Parameter is empty";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentException>(() =>
                    Throws.When.NullOrEmpty(_emptyEnumerable, message: Message));
            Assert.NotNull(exception);
            Assert.Contains(Message, exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Throws.When.NullOrEmpty(_nullEnumerable,
                exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenEmptyParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Throws.When.NullOrEmpty(_emptyEnumerable,
                exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty(_nullEnumerable));
            Assert.NotNull(exception);
            Assert.Contains(nameof(_nullEnumerable), exception.Message);
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(_emptyEnumerable));
            Assert.NotNull(exception);
            Assert.Contains(nameof(_emptyEnumerable), exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        const string ParamName = "test_parameter";

        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Throws.When.NullOrEmpty(_nullEnumerable, ParamName));
            Assert.NotNull(exception);
            Assert.Contains(ParamName, exception.Message);
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        const string ParamName = "test_parameter";

        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentException>(() => Throws.When.NullOrEmpty(_emptyEnumerable, ParamName));
            Assert.NotNull(exception);
            Assert.Contains(ParamName, exception.Message);
        });
    }
}