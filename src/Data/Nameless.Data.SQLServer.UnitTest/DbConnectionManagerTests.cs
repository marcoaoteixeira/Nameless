using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Data.SQLServer {
    public class DbConnectionManagerTests {
        [Test]
        public void GetDbConnection_Should_Return_A_SqlConnection() {
            // arrange
            var sut = new DbConnectionFactory(
                SQLServerOptions.Default,
                Mock.Of<ILogger>()
            );

            // act
            var actual = sut.CreateDbConnection();

            // assert
            Assert.That(actual, Is.InstanceOf<SqlConnection>());
        }

        [Test]
        public void Two_Calls_To_GetDbConnection_Should_Return_Same_SqlConnection() {
            // arrange
            var sut = new DbConnectionFactory(
                SQLServerOptions.Default,
                Mock.Of<ILogger>()
            );

            // act
            var first = sut.CreateDbConnection();
            var second = sut.CreateDbConnection();

            // assert
            Assert.Multiple(() => {
                Assert.That(first, Is.InstanceOf<SqlConnection>());
                Assert.That(second, Is.InstanceOf<SqlConnection>());
                Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
            });
        }

        [Test]
        public void Dispose_Should_Dispose_SqlConnection() {
            // arrange
            var disposed = false;
            var sut = new DbConnectionFactory(
                SQLServerOptions.Default,
                Mock.Of<ILogger>()
            );

            if (sut.CreateDbConnection() is not SqlConnection dbConnection) {
                Assert.Fail("dbConnection is not of type SqlConnection");
                return;
            }

            dbConnection.Disposed += (sender, e)
                => disposed = true;

            // act
            sut.Dispose();

            // assert
            Assert.That(disposed, Is.True);
        }
    }
}
