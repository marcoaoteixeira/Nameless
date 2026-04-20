using Nameless.Data.Responses;
using Nameless.ObjectModel;

namespace Nameless;

public class ResponsesTests {
    // --- ExecuteNonQueryResponse ---

    [Fact]
    public void ExecuteNonQueryResponse_FromValue_IsSuccess() {
        // act
        ExecuteNonQueryResponse response = 5;

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(5, response.Value);
        });
    }

    [Fact]
    public void ExecuteNonQueryResponse_FromError_IsFailure() {
        // act
        ExecuteNonQueryResponse response = Error.Failure("db error");

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Single(response.Errors);
        });
    }

    // --- ExecuteScalarResponse<T> ---

    [Fact]
    public void ExecuteScalarResponse_FromValue_IsSuccess() {
        // act
        ExecuteScalarResponse<int> response = 42;

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(42, response.Value);
        });
    }

    [Fact]
    public void ExecuteScalarResponse_FromError_IsFailure() {
        // act
        ExecuteScalarResponse<int> response = Error.Missing("not found");

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Single(response.Errors);
        });
    }

    // --- ExecuteReaderResponse<T> ---

    [Fact]
    public void ExecuteReaderResponse_FromValueArray_IsSuccess() {
        // arrange
        var rows = new[] { "row1", "row2" };

        // act
        ExecuteReaderResponse<string> response = rows;

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(2, response.Value.Length);
        });
    }

    [Fact]
    public void ExecuteReaderResponse_FromError_IsFailure() {
        // act
        ExecuteReaderResponse<string> response = Error.Failure("err");

        // assert
        Assert.Multiple(() => {
            Assert.False(response.Success);
            Assert.Single(response.Errors);
        });
    }
}
