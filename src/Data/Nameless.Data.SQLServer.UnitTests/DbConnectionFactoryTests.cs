using Microsoft.Data.SqlClient;

namespace Nameless.Data.SqlServer;

public class DbConnectionFactoryTests {
    [Test]
    public void GetDbConnection_Should_Return_A_SqlConnection() {
        // arrange
        var options = Microsoft.Extensions.Options.Options.Create(new SqlServerOptions());
        var sut = new DbConnectionFactory(options);

        // act
        var actual = sut.CreateDbConnection();

        // assert
        Assert.That(actual, Is.InstanceOf<SqlConnection>());
    }

    [Test]
    public void Two_Calls_To_GetDbConnection_Should_Return_Different_SqlConnection() {
        // arrange
        var options = Microsoft.Extensions.Options.Options.Create(new SqlServerOptions());
        var sut = new DbConnectionFactory(options);

        // act
        var first = sut.CreateDbConnection();
        var second = sut.CreateDbConnection();

        // assert
        Assert.Multiple(() => {
            Assert.That(first, Is.InstanceOf<SqlConnection>());
            Assert.That(second, Is.InstanceOf<SqlConnection>());
            Assert.That(first.GetHashCode(), Is.Not.EqualTo(second.GetHashCode()));
        });
    }
}