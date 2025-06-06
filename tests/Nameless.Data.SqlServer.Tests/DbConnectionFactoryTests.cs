using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Nameless.Data.SqlServer;

public class DbConnectionFactoryTests {
    [Fact]
    public void GetDbConnection_Should_Return_A_SqlConnection() {
        // arrange
        var options = Options.Create(new SqlServerOptions());
        var sut = new DbConnectionFactory(options);

        // act
        var actual = sut.CreateDbConnection();

        // assert
        Assert.IsType<SqlConnection>(actual);
    }

    [Fact]
    public void Two_Calls_To_GetDbConnection_Should_Return_Different_SqlConnection() {
        // arrange
        var options = Options.Create(new SqlServerOptions());
        var sut = new DbConnectionFactory(options);

        // act
        var first = sut.CreateDbConnection();
        var second = sut.CreateDbConnection();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<SqlConnection>(first);
            Assert.IsType<SqlConnection>(second);
            Assert.NotSame(first, second);
        });
    }
}