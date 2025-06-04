namespace Nameless;

public class PreventArgumentNullStructTests {
    [Fact]
    public void WhenNonNullParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        var value = Prevent.Argument.Null<int>(1);

        Assert.Equal(1, value);
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(null));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string Message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(null, message: Message));
            Assert.NotNull(exception);
            Assert.Contains(Message, exception.Message);
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
            Assert.NotNull(exception);
            Assert.Contains(nameof(value), exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        const string ParamName = "test_parameter";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Prevent.Argument.Null<int>(null, ParamName));
            Assert.NotNull(exception);
            Assert.Contains(ParamName, exception.Message);
        });
    }
}