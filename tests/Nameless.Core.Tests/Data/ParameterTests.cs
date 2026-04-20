using System.Data;
using Nameless.Data;

namespace Nameless;

public class ParameterTests {
    // --- Constructor ---

    [Fact]
    public void Constructor_WithNameAndValue_SetsProperties() {
        // act
        var param = new Parameter("myParam", 42, DbType.Int32);

        // assert
        Assert.Multiple(() => {
            Assert.Equal("myParam", param.Name);
            Assert.Equal(42, param.Value);
            Assert.Equal(DbType.Int32, param.Type);
        });
    }

    [Fact]
    public void Constructor_WithNullName_UsesEmptyString() {
        // act
        var param = new Parameter(null, "value");

        // assert
        Assert.Equal(string.Empty, param.Name);
    }

    [Fact]
    public void Constructor_WithNullValue_AllowsNull() {
        // act
        var param = new Parameter("name", null);

        // assert
        Assert.Null(param.Value);
    }

    [Fact]
    public void Constructor_DefaultType_IsString() {
        // act
        var param = new Parameter("name", "value");

        // assert
        Assert.Equal(DbType.String, param.Type);
    }
}
