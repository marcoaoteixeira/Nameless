namespace Nameless.Results;

public class CustomResultTests {
    [Fact]
    public void WhenResultHasValue_ThenHasErrorMustBeFalse_AndValueShouldBeExpected() {
        // arrange
        const int expected = 123;
        CustomResult result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.False);
            Assert.That(result.Value, Is.EqualTo(expected));
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsAtLeastOneError() {
        // arrange
        var expected = Error.Failure("Error");
        CustomResult result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.True);
            Assert.That(result.AsErrors, Has.Length.AtLeast(1));
        });
    }

    [Fact]
    public void WhenResultIsError_ThenHasErrorMustBeTrue_WhenErrorsContainsMoreThanOneError() {
        // arrange
        var expected = new[] { Error.Failure("Error"), Error.Conflict("Error") };
        CustomResult result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.True);
            Assert.That(result.AsErrors, Has.Length.AtLeast(2));
        });
    }

    [Fact]
    public void WhenResultIsValid_ThenAccessErrorThrowsInvalidOperationException() {
        // arrange
        const int expected = 123;
        CustomResult result;

        // act
        result = expected;

        // assert
        Assert.Multiple(() => {
            Assert.That(result.HasErrors, Is.False);
            Assert.Throws<InvalidOperationException>(() => _ = result.AsErrors);
        });
    }

    [Fact]
    public void WhenResultIsError_ThenAccessValueThrowsInvalidOperationException() {
        // arrange
        var expected = Error.Failure("Error");
        CustomResult result;

        // act
        result = expected;

        // assert
        Assert.That(result.HasErrors, Is.True);
    }

    [Fact]
    public void WhenResultIsValid_ThenMatchShouldAccessSuccessActionWithReturningValue() {
        // arrange
        CustomResult result = 123;

        // act
        var match = result.Match(SuccessAction, FailureAction);

        // assert
        Assert.That(match, Is.True);

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
        Assert.That(match, Is.True);

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
        Assert.That(match, Is.False);

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
        Assert.That(match, Is.False);

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
        Assert.That(captured, Is.True);

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
        Assert.That(captured, Is.True);

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
        Assert.That(captured, Is.False);

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
        Assert.That(captured, Is.False);

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
        Assert.That(match, Is.False);

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

    public class CustomResult : ResultBase<int> {
        private CustomResult(int index, int? arg0 = null, Error[] errors = null)
            : base(index, arg0.GetValueOrDefault(), errors) {
        }

        public static implicit operator CustomResult(int arg0)
            => new(0, arg0);

        public static implicit operator CustomResult(Error error)
            => new(1, errors: [error]);

        public static implicit operator CustomResult(Error[] errors)
            => new(1, errors: errors);
    }
}