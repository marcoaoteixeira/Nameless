using Microsoft.Data.SqlClient;

namespace Nameless.Data.SQLServer {
    public class DbConnectionManagerTests {
        [Test]
        public void GetDbConnection_Should_Return_A_SqlConnection() {
            // arrange
            var sut = new DbConnectionManager();

            // act
            var actual = sut.GetDbConnection();

            // assert
            Assert.That(actual, Is.InstanceOf<SqlConnection>());
        }

        [Test]
        public void Two_Calls_To_GetDbConnection_Should_Return_Same_SqlConnection() {
            // arrange
            var sut = new DbConnectionManager();

            // act
            var first = sut.GetDbConnection();
            var second = sut.GetDbConnection();

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
            var sut = new DbConnectionManager();
            var dbConnection = sut.GetDbConnection() as SqlConnection;
            dbConnection.Disposed += (sender, e)
                => disposed = true;

            // act
            sut.Dispose();

            // assert
            Assert.That(disposed, Is.True);
        }
    }
}
