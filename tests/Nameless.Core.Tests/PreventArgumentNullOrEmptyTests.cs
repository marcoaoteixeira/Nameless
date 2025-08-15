namespace Nameless;

public class PreventArgumentNullOrEmptyTests {
    private readonly IEnumerable<object> _emptyEnumerable = [];
    private readonly IEnumerable<object> _nonEmptyEnumerable = [1, 2, 3];
    private readonly IEnumerable<object> _nullEnumerable = null;

    [Fact]
    public void WhenNonNullOrEmptyParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        var value = Guard.Against.NullOrEmpty(_nonEmptyEnumerable);

        Assert.NotNull(value);
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Guard.Against.NullOrEmpty(_nullEnumerable));
    }

    [Fact]
    public void WhenEmptyParamValue_ThenThrowsArgumentException() {
        // arrange & act & assert
        Assert.Throws<ArgumentException>(() => Guard.Against.NullOrEmpty(_emptyEnumerable));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string Message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() =>
                    Guard.Against.NullOrEmpty(_nullEnumerable, message: Message));
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
                    Guard.Against.NullOrEmpty(_emptyEnumerable, message: Message));
            Assert.NotNull(exception);
            Assert.Contains(Message, exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Guard.Against.NullOrEmpty(_nullEnumerable,
                exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenEmptyParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Guard.Against.NullOrEmpty(_emptyEnumerable,
                exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Guard.Against.NullOrEmpty(_nullEnumerable));
            Assert.NotNull(exception);
            Assert.Contains(nameof(_nullEnumerable), exception.Message);
        });
    }

    [Fact]
    public void WhenEmptyParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentException>(() => Guard.Against.NullOrEmpty(_emptyEnumerable));
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
                Assert.Throws<ArgumentNullException>(() => Guard.Against.NullOrEmpty(_nullEnumerable, ParamName));
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
                Assert.Throws<ArgumentException>(() => Guard.Against.NullOrEmpty(_emptyEnumerable, ParamName));
            Assert.NotNull(exception);
            Assert.Contains(ParamName, exception.Message);
        });
    }
}