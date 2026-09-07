using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Data.SqlServer;

[UnitTest]
public class DbConnectionFactoryTests {
    private static DbConnectionFactory CreateSut(SqlServerOptions options, IConfiguration? configuration = null) {
        configuration ??= new Mock<IConfiguration>().Object;

        var optionsMock = new Mock<IOptions<SqlServerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        return new DbConnectionFactory(configuration, optionsMock.Object);
    }

    [Fact]
    public void ProviderName_ReturnsExpectedValue() {
        // arrange
        var sut = CreateSut(new SqlServerOptions());

        // act
        var actual = sut.ProviderName;

        // assert
        Assert.Equal("Microsoft SQL Server", actual);
    }

    [Fact]
    public void CreateDbConnection_WithConnectionStringName_UsesConfiguration() {
        // arrange
        const string ConnStringName = "MyDb";
        const string ConnString = "Server=(localdb)\\MSSQLLocalDB;Database=TestDb;Integrated Security=true;";

        // ConfigurationHelper builds a real IConfiguration that properly
        // implements GetConnectionString (backed by the "ConnectionStrings" section).
        var configuration = ConfigurationHelper.CreateConfiguration(new Dictionary<string, string?> {
            [$"ConnectionStrings:{ConnStringName}"] = ConnString
        });

        var options = new SqlServerOptions {
            ConnectionStringName = ConnStringName
        };

        var sut = CreateSut(options, configuration);

        // act — do not open; just verify the connection object is non-null
        var connection = sut.CreateDbConnection();

        // assert
        Assert.NotNull(connection);
        Assert.Equal(ConnectionState.Closed, connection.State);

        connection.Dispose();
    }

    [Fact]
    public void CreateDbConnection_WithBuiltConnectionString_CreatesConnection() {
        // arrange
        var options = new SqlServerOptions {
            Server = "(localdb)\\MSSQLLocalDB",
            Database = "master",
            UseIntegratedSecurity = true
        };

        var sut = CreateSut(options);

        // act — only check the connection is returned; do NOT open (no live SQL Server)
        var connection = sut.CreateDbConnection();

        // assert
        Assert.NotNull(connection);
        Assert.Equal(ConnectionState.Closed, connection.State);

        connection.Dispose();
    }

    [Fact]
    public void CreateDbConnection_WithCredentials_CreatesConnection() {
        // arrange
        var options = new SqlServerOptions {
            Server = "(localdb)\\MSSQLLocalDB",
            Database = "master",
            Username = "sa",
            Password = "Password1!"
        };

        var sut = CreateSut(options);

        // act
        var connection = sut.CreateDbConnection();

        // assert
        Assert.NotNull(connection);

        connection.Dispose();
    }

    [Fact]
    public void CreateDbConnection_WithAttachedDb_CreatesConnection() {
        // arrange
        var options = new SqlServerOptions {
            Server = "(localdb)\\MSSQLLocalDB",
            Database = "C:\\Data\\mydb.mdf",
            UseAttachedDb = true,
            UseIntegratedSecurity = true
        };

        var sut = CreateSut(options);

        // act
        var connection = sut.CreateDbConnection();

        // assert
        Assert.NotNull(connection);

        connection.Dispose();
    }
}
