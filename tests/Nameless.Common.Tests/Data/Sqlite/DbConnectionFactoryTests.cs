using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Testing.Tools.Helpers;

namespace Nameless.Data.Sqlite;

[UnitTest]
public class DbConnectionFactoryTests {
    private static DbConnectionFactory CreateSut(SqliteOptions options, IConfiguration? configuration = null) {
        configuration ??= new Mock<IConfiguration>().Object;

        var optionsMock = new Mock<IOptions<SqliteOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        return new DbConnectionFactory(configuration, optionsMock.Object);
    }

    [Fact]
    public void ProviderName_ReturnsSqlite() {
        // arrange
        var sut = CreateSut(new SqliteOptions { UseInMemory = true });

        // act
        var actual = sut.ProviderName;

        // assert
        Assert.Equal("Sqlite", actual);
    }

    [Fact]
    public void CreateDbConnection_WithConnectionStringName_UsesConfiguration() {
        // arrange
        const string ConnStringName = "MyDb";
        const string ConnString = "Data Source=named.db";

        var configuration = ConfigurationHelper.CreateConfiguration(new Dictionary<string, string?> {
            [$"ConnectionStrings:{ConnStringName}"] = ConnString
        });

        var options = new SqliteOptions {
            ConnectionStringName = ConnStringName
        };

        var sut = CreateSut(options, configuration);

        // act
        var connection = sut.CreateDbConnection();

        // assert
        Assert.Multiple(
            () => Assert.NotNull(connection),
            () => Assert.Equal(ConnectionState.Closed, connection.State)
        );

        connection.Dispose();
    }

    [Fact]
    public void CreateDbConnection_WithCredentials_CreatesConnectionWithPassword() {
        // arrange
        var sut = CreateSut(new SqliteOptions {
            UseInMemory = false,
            DatabaseFilePath = "protected.db",
            Password = "s3cr3t"
        });

        // act
        var connection = sut.CreateDbConnection();

        // assert
        Assert.Multiple(
            () => Assert.NotNull(connection),
            () => Assert.Contains("Password=s3cr3t", connection.ConnectionString)
        );

        connection.Dispose();
    }

    [Fact]
    public void CreateDbConnection_WithInMemoryOption_ReturnsOpenableConnection() {
        // arrange
        var sut = CreateSut(new SqliteOptions { UseInMemory = true });

        // act
        var connection = sut.CreateDbConnection();
        var exception = Record.Exception(connection.Open);

        // assert
        Assert.Multiple(
            () => Assert.NotNull(connection),
            () => Assert.Null(exception),
            () => Assert.Equal(ConnectionState.Open, connection.State)
        );

        // cleanup
        connection.Dispose();
    }
}
