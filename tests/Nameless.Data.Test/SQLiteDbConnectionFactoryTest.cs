using System;
using System.Data.SQLite;
using Nameless.Data.SQLite;
using Xunit;

namespace Nameless.Data.Test {
    public class SQLiteDbConnectionFactoryTest {
        [Fact]
        public void Can_Create () {
            // arrange
            var settings = new DatabaseSettings ();
            IDbConnectionFactory factory;

            // act
            factory = new DbConnectionFactory (settings);

            // assert
            Assert.NotNull (factory);
        }

        [Fact]
        public void ProviderName_Must_Be_SQLITE () {
            // arrange
            var settings = new DatabaseSettings { ConnectionString = Constants.SQLite_ConnStr_InMemory };
            IDbConnectionFactory factory;

            // act
            factory = new DbConnectionFactory (settings);

            // assert
            Assert.Equal ("SQLITE", factory.ProviderName);
        }

        [Fact]
        public void Create_Must_Return_Instance_Of_SQLiteConnection () {
            // arrange
            var settings = new DatabaseSettings { ConnectionString = Constants.SQLite_ConnStr_InMemory };
            IDbConnectionFactory factory = new DbConnectionFactory (settings);

            // act
            var connection = factory.Create ();

            // assert
            Assert.NotNull (connection);
            Assert.IsType<SQLiteConnection> (connection);

            connection.Dispose ();
        }

        [Fact]
        public void Connection_With_Correct_ConnStr () {
            // arrange
            var settings = new DatabaseSettings { ConnectionString = Constants.SQLite_ConnStr_InMemory };
            IDbConnectionFactory factory = new DbConnectionFactory (settings);

            // act
            var connection = factory.Create ();

            // assert
            Assert.NotNull (connection);
            Assert.IsType<SQLiteConnection> (connection);
            Assert.Equal (connection.ConnectionString, settings.ConnectionString);

            connection.Dispose ();
        }

        [Fact]
        public void Throws_ArgumentNullException_If_Settings_Is_Null_Object () {
            // arrange & act & assert
            Assert.Throws<ArgumentNullException> (() => new DbConnectionFactory (null));
        }

        [Fact]
        public void Multiple_Call_To_Create_Returns_Distinct_Connections () {
            // arrange
            var settings = new DatabaseSettings { ConnectionString = Constants.SQLite_ConnStr_InMemory };
            IDbConnectionFactory factory = new DbConnectionFactory (settings);

            // act
            var connection1 = factory.Create ();
            var connection2 = factory.Create ();

            // assert
            Assert.NotNull (connection1);
            Assert.IsType<SQLiteConnection> (connection1);
            Assert.NotNull (connection2);
            Assert.IsType<SQLiteConnection> (connection2);
            Assert.NotEqual (connection1, connection2);

            connection1.Dispose ();
            connection2.Dispose ();
        }
    }
}
