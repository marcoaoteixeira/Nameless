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
            Assert.False(result.HasErrors);
            Assert.Equal(Expected, result.Value);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsAtLeastOneError() {
        // arrange
        var expected = Error.Failure("Error");

        // act
        CustomResult result = expected;

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

        // act
        CustomResult result = expected;

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

        // act
        CustomResult result = Expected;

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

        // act
        CustomResult result = expected;

        // assert
        Assert.True(result.HasErrors);
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
        CustomResult result = 123;

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
        CustomResult result = Error.Failure("Error");

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
        CustomResult result = Error.Failure("Error");

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
        CustomResult result = 123;
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
        CustomResult result = 123;
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
        CustomResult result = Error.Failure("Error");
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
        CustomResult result = Error.Failure("Error");
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
        CustomResult result = 123;

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
    public void WhenCallingParameterlessConstructor_ThenThrowsInvalidOperationException() {
        // arrange & act & assert
        Assert.Throws<InvalidOperationException>(() => new CustomResult());
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
        var error = Error.Failure("Failure");

        // act
        CustomResult actual = error;

        // assert
        Assert.IsType<Error[]>(actual.Value);
    }

    [Fact]
    public void WhenIncorrectImplementationWithInvalidIndex_WhenGettingValue_ThenThrowsInvalidOperationException() {
        // arrange
        const string Error = "Error Message";

        // act
        CustomResult actual = Error;

        // assert
        Assert.Throws<InvalidOperationException>(() => actual.Value);
    }

    public class CustomResult : ResultBase<int> {
        public CustomResult() { }

        private CustomResult(int index, int? arg0 = null, Error[] errors = null)
            : base(index, arg0.GetValueOrDefault(), errors) {
        }

        public static implicit operator CustomResult(int arg0)
            => new(0, arg0);

        public static implicit operator CustomResult(Error error)
            => new(1, errors: [error]);

        public static implicit operator CustomResult(Error[] errors)
            => new(1, errors: errors);

        // Implementation error, index is out of bounds
        public static implicit operator CustomResult(string error)
            => new(2, errors: [Error.Failure(error)]);
    }
}