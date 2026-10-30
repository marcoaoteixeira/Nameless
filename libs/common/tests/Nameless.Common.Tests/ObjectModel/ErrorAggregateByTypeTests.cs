namespace Nameless.ObjectModel;

public class ErrorAggregateByTypeTests {
    // --- Empty / filtering ---------------------------------------------------

    [Fact]
    public void EmptySequence_Throws() {
        var errors = Array.Empty<Error>();

        Assert.Throws<InvalidOperationException>(() => errors.AggregateByType(ErrorType.Validation));
    }

    [Fact]
    public void NoErrorsOfRequestedType_Throws() {
        Error[] errors = [Error.Failure("boom"), Error.Missing("gone")];

        Assert.Throws<InvalidOperationException>(() => errors.AggregateByType(ErrorType.Validation));
    }

    [Fact]
    public void OnlyErrorsOfRequestedType_AreIncluded() {
        Error[] errors =
        [
            Error.Validation("a"),
            Error.Failure("ignored"),
            Error.Validation("b"),
        ];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Equal("a; b", result.Message);
        Assert.Equal(ErrorType.Validation, result.Type);
    }

    [Fact]
    public void SingleError_IsReturnedUnchanged() {
        var ex = new InvalidOperationException();
        var error = Error.Conflict("dup", "C1", ex);

        var result = new[] { error }.AggregateByType(ErrorType.Conflict);

        Assert.Equal(error, result);
        Assert.Same(ex, result.Exception);
    }

    // --- Message -------------------------------------------------------------

    [Fact]
    public void Message_IsJoinedInOrder() {
        Error[] errors = [Error.Validation("first"), Error.Validation("second"), Error.Validation("third")];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Equal("first; second; third", result.Message);
    }

    // --- Code ----------------------------------------------------------------

    [Fact]
    public void Code_AllPresent_AreJoined() {
        Error[] errors = [Error.Validation("a", "A"), Error.Validation("b", "B")];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Equal("A; B", result.Code);
    }

    [Fact]
    public void Code_NonePresent_IsNull() {
        Error[] errors = [Error.Validation("a"), Error.Validation("b")];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Null(result.Code);
    }

    [Fact]
    public void Code_MissingInMiddle_IsSkipped() {
        Error[] errors = [Error.Validation("a", "A"), Error.Validation("b"), Error.Validation("c", "C")];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Equal("A; C", result.Code);
    }

    [Fact]
    public void Code_FirstErrorWithoutCode_HasNoLeadingSeparator() {
        Error[] errors = [Error.Validation("a"), Error.Validation("b", "B")];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Equal("B", result.Code);
    }

    [Fact]
    public void Code_WhitespaceCodes_AreIgnored() {
        Error[] errors = [Error.Validation("a", "  "), Error.Validation("b", "B"), Error.Validation("c", "")];

        var result = errors.AggregateByType(ErrorType.Validation);

        Assert.Equal("B", result.Code);
    }

    // --- Exception -----------------------------------------------------------

    [Fact]
    public void Exception_NonePresent_IsNull() {
        Error[] errors = [Error.Failure("a"), Error.Failure("b")];

        var result = errors.AggregateByType(ErrorType.Failure);

        Assert.Null(result.Exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Exception_OnlyOnePresent_IsReturnedAsIs(int position) {
        var ex = new InvalidOperationException();
        var errors = Enumerable.Range(0, 3)
            .Select(i => Error.Failure($"e{i}", ex: i == position ? ex : null))
            .ToArray();

        var result = errors.AggregateByType(ErrorType.Failure);

        Assert.Same(ex, result.Exception);
    }

    [Fact]
    public void Exception_TwoPresent_AreWrappedInAggregate() {
        var ex1 = new InvalidOperationException();
        var ex2 = new ArgumentException();
        Error[] errors = [Error.Failure("a", ex: ex1), Error.Failure("b", ex: ex2)];

        var result = errors.AggregateByType(ErrorType.Failure);

        var aggregate = Assert.IsType<AggregateException>(result.Exception);
        Assert.Equal([ex1, ex2], aggregate.InnerExceptions);
    }

    [Fact]
    public void Exception_ManyPresent_ProduceSingleFlatAggregate() {
        var exceptions = Enumerable.Range(0, 4).Select(i => new Exception($"ex{i}")).ToArray();
        var errors = exceptions.Select((ex, i) => Error.Failure($"e{i}", ex: ex)).ToArray();

        var result = errors.AggregateByType(ErrorType.Failure);

        var aggregate = Assert.IsType<AggregateException>(result.Exception);
        Assert.Equal(exceptions, aggregate.InnerExceptions);
    }

    [Fact] // fails on the original implementation: inner exceptions get flattened
    public void Exception_ExistingAggregateException_IsKeptNested() {
        var original = new AggregateException(new Exception("x"), new Exception("y"));
        var other = new InvalidOperationException();
        Error[] errors = [Error.Failure("a", ex: original), Error.Failure("b", ex: other)];

        var result = errors.AggregateByType(ErrorType.Failure);

        var aggregate = Assert.IsType<AggregateException>(result.Exception);
        Assert.Equal([original, other], aggregate.InnerExceptions);
    }
}
