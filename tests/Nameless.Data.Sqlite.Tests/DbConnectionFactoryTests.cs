using System.Data.SQLite;
using Microsoft.Extensions.Options;
using Microsoft.Testing.Platform.Builder;
using Nameless.Testing.Tools.Mockers.Configuration;

namespace Nameless.Data.Sqlite;

public class DbConnectionFactoryTests {
    [Fact]
    public void WhenCreatingDbConnectionFactory_ProviderShouldBeMsSqlServer() {
        // arrange
        const string Expected = "Sqlite";
        var options = Options.Create(new SqliteOptions());
        var configuration = new ConfigurationMocker().Build();
        var sut = new DbConnectionFactory(configuration, options);

        // act
        var actual = sut.ProviderName;

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public void GetDbConnection_Should_Return_A_SqlConnection() {
        // arrange
        var options = Options.Create(new SqliteOptions());
        var configuration = new ConfigurationMocker().Build();
        var sut = new DbConnectionFactory(configuration, options);

        // act
        var actual = sut.CreateDbConnection();

        // assert
        Assert.IsType<SQLiteConnection>(actual);
    }

    [Fact]
    public void Two_Calls_To_GetDbConnection_Should_Return_Different_SqlConnection() {
        // arrange
        var options = Options.Create(new SqliteOptions());
        var configuration = new ConfigurationMocker().Build();
        var sut = new DbConnectionFactory(configuration, options);

        // act
        var first = sut.CreateDbConnection();
        var second = sut.CreateDbConnection();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<SQLiteConnection>(first);
            Assert.IsType<SQLiteConnection>(second);
            Assert.NotSame(first, second);
        });
    }
}