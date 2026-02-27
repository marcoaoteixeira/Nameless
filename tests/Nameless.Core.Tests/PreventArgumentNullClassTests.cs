namespace Nameless;

public class PreventArgumentNullClassTests {
    [Fact]
    public void WhenNonNullParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        var value = Throws.When.Null<object>(paramValue: 1);

        Assert.NotNull(value);
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.Null<object>(paramValue: null));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string Message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() =>
                    Throws.When.Null<object>(paramValue: null, message: Message));
            Assert.NotNull(exception);
            Assert.Contains(Message, exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() // arrange & act & assert
    {
        Assert.Throws<InvalidOperationException>(() =>
            Throws.When.Null<object>(paramValue: null, exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        // arrange & act & assert
        object user = null;

        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Throws.When.Null(user));
            Assert.NotNull(exception);
            Assert.Contains(nameof(user), exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        // arrange & act & assert
        object user = null;
        const string ParamName = "test_parameter";

        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Throws.When.Null(user, ParamName));
            Assert.NotNull(exception);
            Assert.Contains(ParamName, exception.Message);
        });
    }
}