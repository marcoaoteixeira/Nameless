using Nameless.ObjectModel;

namespace Nameless.Results;

public class CustomResultTests {
    [Fact]
    public void WhenResultHasValue_ThenHasErrorMustBeFalse_AndValueShouldBeExpected() {
        // arrange
        const int Expected = 123;

        // act
        CustomResult result = Expected;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.Success);
            Assert.Equal(Expected, result.Value);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsAtLeastOneError() {
        // arrange
        const string Message = "Error";
        var expected = Error.Failure(Message);

        // act
        CustomResult result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.False(result.Success);
            Assert.Equal(expected, result.Errors[0]);
        });
    }

    [Fact]
    public void WhenResultIsValid_ThenAccessErrorThrowsInvalidOperationException() {
        // arrange
        const int Expected = 123;

        // act
        CustomResult result = Expected;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.Success);
            Assert.Throws<InvalidOperationException>(() => _ = result.Errors);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenAccessValueThrowsInvalidOperationException() {
        // arrange
        var expected = Error.Failure(message: "Error");

        // act
        CustomResult result = expected;

        // assert
        Assert.False(result.Success);
    }

    [Fact]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithReturningValue() {
        // arrange
        CustomResult result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.True(match);

        return;

        static bool SuccessAction(int value) {
            return true;
        }

        static bool FailureAction(Error[] error) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithReturningValue() {
        // arrange
        CustomResult result = 123;

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.True(match);

        return;

        static Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(result: true);
        }

        static Task<bool> FailureActionAsync(Error[] error) {
            return Task.FromResult(result: false);
        }
    }

    [Fact]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithReturningValue() {
        // arrange
        CustomResult result = Error.Failure(message: "Error");

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.False(match);

        return;

        static bool SuccessAction(int value) {
            return true;
        }

        static bool FailureAction(Error[] error) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithReturningValue() {
        // arrange
        CustomResult result = Error.Failure(message: "Error");

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.False(match);

        return;

        static Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(result: true);
        }

        static Task<bool> FailureActionAsync(Error[] error) {
            return Task.FromResult(result: false);
        }
    }

    [Fact]
    public void WhenResult_ThenMultipleMatches() {
        // arrange
        CustomResult result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction)
                          .Match(FinalSuccessAction, FinalFailureAction);

        // assert
        Assert.False(match);

        return;

        static Result<bool> SuccessAction(int value) {
            return value % 2 == 0;
        }

        static Result<bool> FailureAction(Error[] error) {
            return true;
        }

        static bool FinalSuccessAction(bool value) {
            return value;
        }

        static bool FinalFailureAction(Error[] error) {
            return false;
        }
    }

    [Fact]
    public void WhenArg0IsPresent_ThenValueReturnsCorrectValue() {
        // arrange
        const int Value = 1234;

        // act
        CustomResult actual = Value;

        // assert
        Assert.IsType<int>(actual.Value);
    }

    [Fact]
    public void WhenErrorIsPresent_ThenValueReturnsError() {
        // arrange
        var error = Error.Failure(message: "Failure");

        // act
        CustomResult actual = error;

        // assert
        Assert.IsType<Error>(actual.Value);
    }

    public class CustomResult : Result<int> {
        private CustomResult(int value, Error[] errors)
            : base(value, errors) {
        }

        public static implicit operator CustomResult(int value) {
            return new CustomResult(value: value, errors: []);
        }

        public static implicit operator CustomResult(Error error) {
            return new CustomResult(value: 0, errors: [error]);
        }

        public static implicit operator CustomResult(Error[] errors) {
            return new CustomResult(value: 0, errors);
        }
    }
}