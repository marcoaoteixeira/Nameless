using System.Data;
using Nameless.Data.Requests;

namespace Nameless;

public class RequestsTests {
    // --- RequestBase ---

    [Fact]
    public void RequestBase_WithText_SetsText() {
        // act
        var request = new ConcreteRequest { Text = "SELECT 1" };

        // assert
        Assert.Equal("SELECT 1", request.Text);
    }

    [Fact]
    public void RequestBase_WithType_SetsType() {
        // act
        var request = new ConcreteRequest { Text = "SELECT 1", Type = CommandType.StoredProcedure };

        // assert
        Assert.Equal(CommandType.StoredProcedure, request.Type);
    }

    [Fact]
    public void RequestBase_DefaultParameters_IsEmpty() {
        // act
        var request = new ConcreteRequest { Text = "SELECT 1" };

        // assert
        Assert.Empty(request.Parameters);
    }

    // --- ExecuteReaderRequest<T> ---

    [Fact]
    public void ExecuteReaderRequest_WithMapper_SetsMapper() {
        // arrange
        Func<IDataRecord, string> mapper = _ => "row";

        // act
        var request = new ExecuteReaderRequest<string> { Text = "SELECT 1", Mapper = mapper };

        // assert
        Assert.Same(mapper, request.Mapper);
    }

    // --- test doubles ---

    private sealed record ConcreteRequest : RequestBase;
}
