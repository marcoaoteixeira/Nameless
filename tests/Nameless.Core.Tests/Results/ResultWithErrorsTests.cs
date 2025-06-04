namespace Nameless.Results;

public class ResultWithErrorsTests {
    [Fact]
    public void WhenResultHasValue_ThenHasErrorMustBeFalse_AndValueShouldBeExpected() {
        // arrange
        const int Expected = 123;
        Result<int> result;

        // act
        result = Expected;

        // assert
        Assert.Multiple(() => {
            Assert.False(result.HasErrors);
            Assert.Equal(Expected, result.Value);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsAtLeastOneError() {
        // arrange
        var expected = Error.Failure("Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.HasErrors);
            Assert.Single(result.AsErrors);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsMoreThanOneError() {
        // arrange
        var expected = new[] { Error.Failure("Error"), Error.Conflict("Error") };
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.True(result.HasErrors);
            Assert.Equal(2, result.AsErrors.Length);
        });
    }

    [Fact]
    public void WhenResultIsValid_ThenAccessErrorThrowsInvalidOperationException() {
        // arrange
        const int Expected = 123;
        Result<int> result;

        // act
        result = Expected;

        // assert
        Assert.Multiple(() => {
            Assert.False(result.HasErrors);
            Assert.Throws<InvalidOperationException>(() => _ = result.AsErrors);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenAccessValueThrowsInvalidOperationException() {
        // arrange
        var expected = Error.Failure("Error");
        Result<int> result;

        // act
        result = expected;

        // assert
        Assert.True(result.HasErrors);
    }

    [Fact]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithReturningValue() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.True(match);

        return;

        bool SuccessAction(int value) {
            return true;
        }

        bool FailureAction(Error[] errors) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = 123;

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.True(match);

        return;

        Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(true);
        }

        Task<bool> FailureActionAsync(Error[] errors) {
            return Task.FromResult(false);
        }
    }

    [Fact]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.False(match);

        return;

        bool SuccessAction(int value) {
            return true;
        }

        bool FailureAction(Error[] errors) {
            return false;
        }
    }

    [Fact]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");

        // act
        var match = await result.Match(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.False(match);

        return;

        Task<bool> SuccessActionAsync(int value) {
            return Task.FromResult(true);
        }

        Task<bool> FailureActionAsync(Error[] errors) {
            return Task.FromResult(false);
        }
    }

    [Fact]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithoutReturningValue() {
        // arrange
        Result<int> result = 123;
        object captured = null;

        // act
        result.Switch(SuccessAction, FailureAction);

        // assert
        Assert.True((bool)captured);

        return;

        void SuccessAction(int value) {
            captured = true;
        }

        void FailureAction(Error[] errors) {
            captured = false;
        }
    }

    [Fact]
    public async Task WhenResultIsValid_ThenMatchAsyncShouldAccessSuccessActionAsyncWithoutReturningValue() {
        // arrange
        Result<int> result = 123;
        object captured = null;

        // act
        await result.Switch(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.True((bool)captured);

        return;

        Task SuccessActionAsync(int value) {
            captured = true;

            return Task.CompletedTask;
        }

        Task FailureActionAsync(Error[] errors) {
            captured = false;

            return Task.CompletedTask;
        }
    }

    [Fact]
    public void WhenResultIsError_ThenMatchShouldAccessFailureActionWithoutReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");
        object captured = null;

        // act
        result.Switch(SuccessAction, FailureAction);

        // assert
        Assert.False((bool)captured);

        return;

        void SuccessAction(int value) {
            captured = true;
        }

        void FailureAction(Error[] errors) {
            captured = false;
        }
    }

    [Fact]
    public async Task WhenResultIsError_ThenMatchAsyncShouldAccessFailureActionAsyncWithoutReturningValue() {
        // arrange
        Result<int> result = Error.Failure("Error");
        object captured = null;

        // act
        await result.Switch(SuccessActionAsync, FailureActionAsync);

        // assert
        Assert.False((bool)captured);

        return;

        Task SuccessActionAsync(int value) {
            captured = true;

            return Task.CompletedTask;
        }

        Task FailureActionAsync(Error[] errors) {
            captured = false;

            return Task.CompletedTask;
        }
    }

    [Fact]
    public void WhenResult_ThenMultipleMatches() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction)
                          .Match(FinalSuccessAction, FinalFailureAction);

        // assert
        Assert.False(match);

        return;

        Result<bool> SuccessAction(int value) {
            return value % 2 == 0;
        }

        Result<bool> FailureAction(Error[] errors) {
            if (errors.Length is > 1 and < 3) {
                return true;
            }

            return Error.Failure("Error should be exactly 2");
        }

        bool FinalSuccessAction(bool value) {
            return value;
        }

        bool FinalFailureAction(Error[] errors) {
            return false;
        }
    }

    [Fact]
    public void Result_With_Three_Matches() {
        // arrange
        Result<int> result = 123;

        // act
        var match = result.Match(FirstSuccessAction, FirstFailureAction)
                          .Match(SecondSuccessAction, SecondFailureAction)
                          .Match(FinalSuccessAction, FinalFailureAction);

        // assert
        Assert.True(match);

        return;

        Result<bool> FirstSuccessAction(int value) {
            return Error.Conflict("Conflict");
        }

        Result<bool> FirstFailureAction(Error[] errors) {
            if (errors.Length is > 1 and < 3) {
                return true;
            }

            return Error.Failure("Error should be exactly 2");
        }

        Result<string> SecondSuccessAction(bool value) {
            return value.ToString();
        }

        Result<string> SecondFailureAction(Error[] errors) {
            return errors;
        }

        bool FinalSuccessAction(string value) {
            return value == "Hello world";
        }

        bool FinalFailureAction(Error[] errors) {
            return errors.Length > 0;
        }
    }

    [Fact]
    public void WhenResultHasNullableValue_ThenHasErrorMustBeFalse() {
        // arrange
        Result<int?> result;

        // act
        result = (int?)null;

        // assert
        Assert.Multiple(() => {
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        });
    }
}