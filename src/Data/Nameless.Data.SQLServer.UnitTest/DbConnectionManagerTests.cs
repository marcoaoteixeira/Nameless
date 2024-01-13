using Microsoft.Data.SqlClient;

namespace Nameless.Data.SQLServer {
    public class DbConnectionManagerTests {
        [Test]
        public void GetDbConnection_Should_Return_A_SqlConnection() {
            // arrange
            var sut = new DbConnectionFactory(
                SQLServerOptions.Default
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
                SQLServerOptions.Default
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
    }
}
