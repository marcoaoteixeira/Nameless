using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Data.Sqlite;

[UnitTest]
public class DbConnectionFactoryTests {
    private static DbConnectionFactory CreateSut(SqliteOptions options) {
        var configurationMock = new Mock<IConfiguration>();
        var optionsMock = new Mock<IOptions<SqliteOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        return new DbConnectionFactory(configurationMock.Object, optionsMock.Object);
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
    public void CreateDbConnection_WithInMemoryOption_ReturnsOpenableConnection() {
        // arrange
        var sut = CreateSut(new SqliteOptions { UseInMemory = true });

        // act
        var connection = sut.CreateDbConnection();
        var exception = Record.Exception(connection.Open);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(connection);
            Assert.Null(exception);
            Assert.Equal(ConnectionState.Open, connection.State);
        });

        // cleanup
        connection.Dispose();
    }
}
