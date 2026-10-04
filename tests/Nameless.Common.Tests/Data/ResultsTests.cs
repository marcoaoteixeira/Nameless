using Nameless.ObjectModel;

namespace Nameless.Data;

[UnitTest]
public class ResultsTests {
    // --- ExecuteNonQueryResponse ---

    [Fact]
    public void ExecuteNonQueryResponse_FromValue_IsSuccess() {
        // act
        ExecuteNonQueryResult result = 5;

        // assert
        Assert.Multiple(
            () => Assert.True(result.Success),
            () => Assert.Equal(5, result.Value)
        );
    }

    [Fact]
    public void ExecuteNonQueryResponse_FromError_IsFailure() {
        // act
        ExecuteNonQueryResult result = Error.Failure("db error");

        // assert
        Assert.Multiple(
            () => Assert.False(result.Success),
            () => Assert.Single(result.Errors)
        );
    }

    // --- ExecuteScalarResponse<T> ---

    [Fact]
    public void ExecuteScalarResponse_FromValue_IsSuccess() {
        // act
        ExecuteScalarResult<int> result = 42;

        // assert
        Assert.Multiple(
            () => Assert.True(result.Success),
            () => Assert.Equal(42, result.Value)
        );
    }

    [Fact]
    public void ExecuteScalarResponse_FromError_IsFailure() {
        // act
        ExecuteScalarResult<int> result = Error.Missing("not found");

        // assert
        Assert.Multiple(
            () => Assert.False(result.Success),
            () => Assert.Single(result.Errors)
        );
    }

    // --- ExecuteReaderResponse<T> ---

    [Fact]
    public void ExecuteReaderResponse_FromValueArray_IsSuccess() {
        // arrange
        var rows = new[] { "row1", "row2" };

        // act
        ExecuteReaderResult<string> result = rows;

        // assert
        Assert.Multiple(
            () => Assert.True(result.Success),
            () => Assert.Equal(2, result.Value.Length)
        );
    }

    [Fact]
    public void ExecuteReaderResponse_FromError_IsFailure() {
        // act
        ExecuteReaderResult<string> result = Error.Failure("err");

        // assert
        Assert.Multiple(
            () => Assert.False(result.Success),
            () => Assert.Single(result.Errors)
        );
    }
}
