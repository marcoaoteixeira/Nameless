namespace Nameless;

public class PreventArgumentNullStructTests {
    [Fact]
    public void WhenNonNullParamValue_ThenDoNotThrows() {
        // arrange & act & assert
        var value = Throws.When.Null<int>(paramValue: 1);

        Assert.Equal(expected: 1, value);
    }

    [Fact]
    public void WhenNullParamValue_ThenThrowsArgumentNullException() {
        // arrange & act & assert
        Assert.Throws<ArgumentNullException>(() => Throws.When.Null<int>(paramValue: null));
    }

    [Fact]
    public void WhenNullParamValue_WithCustomMessage_ThenThrowsWithCustomMessage() {
        const string Message = "Parameter is null";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Throws.When.Null<int>(paramValue: null, message: Message));
            Assert.NotNull(exception);
            Assert.Contains(Message, exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithCustomExceptionCreator_ThenThrowsCustomException() {
        // arrange & act & assert
        Assert.Throws<InvalidOperationException>(() =>
            Throws.When.Null<int>(paramValue: null, exceptionCreator: () => new InvalidOperationException()));
    }

    [Fact]
    public void WhenNullParamValue_WithOmittedParamName_ThenExceptionMessageContainsCallerParamName() {
        int? value = null;

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception = Assert.Throws<ArgumentNullException>(() => Throws.When.Null(value));
            Assert.NotNull(exception);
            Assert.Contains(nameof(value), exception.Message);
        });
    }

    [Fact]
    public void WhenNullParamValue_WithParamName_ThenExceptionMessageContainsParamName() {
        const string ParamName = "test_parameter";

        // arrange & act & assert
        Assert.Multiple(() => {
            var exception =
                Assert.Throws<ArgumentNullException>(() => Throws.When.Null<int>(paramValue: null, ParamName));
            Assert.NotNull(exception);
            Assert.Contains(ParamName, exception.Message);
        });
    }
}