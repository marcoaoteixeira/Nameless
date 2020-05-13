using System.Data;
using System.Data.SQLite;
using Nameless.Data.SQLite.Test.Fixtures;
using Xunit;
using Moq;

namespace Nameless.Data.SQLite.Test {
    public class DatabaseTest {

        private static void CreateData (SQLiteConnection conn) {
            if (conn.State != ConnectionState.Open) {
                conn.Open ();
            }


            // Create table
            var cmdCreateTable = conn.CreateCommand ();
            cmdCreateTable.CommandText = "CREATE TABLE Animals (ID INTEGER PRIMARY KEY ASC, Name TEXT, MaxAge INTEGER)";
            cmdCreateTable.ExecuteNonQuery ();

            // Insert data
            var cmdCreateRecord = conn.CreateCommand ();
            cmdCreateRecord.CommandText = "INSERT INTO Animals VALUES (@ID, @Name, @MaxAge)";
            var idParam = cmdCreateRecord.CreateParameter ();
            idParam.ParameterName = "ID";
            idParam.DbType = DbType.Int32;
            cmdCreateRecord.Parameters.Add (idParam);
            var nameParam = cmdCreateRecord.CreateParameter ();
            nameParam.ParameterName = "Name";
            nameParam.DbType = DbType.String;
            cmdCreateRecord.Parameters.Add (nameParam);
            var maxAgeParam = cmdCreateRecord.CreateParameter ();
            maxAgeParam.ParameterName = "MaxAge";
            maxAgeParam.DbType = DbType.Int32;
            cmdCreateRecord.Parameters.Add (maxAgeParam);

            // Horse
            cmdCreateRecord.Parameters[0].Value = 1;
            cmdCreateRecord.Parameters[1].Value = "Horse";
            cmdCreateRecord.Parameters[2].Value = 25;
            cmdCreateRecord.ExecuteNonQuery ();

            // Dog
            cmdCreateRecord.Parameters[0].Value = 2;
            cmdCreateRecord.Parameters[1].Value = "Dog";
            cmdCreateRecord.Parameters[2].Value = 12;
            cmdCreateRecord.ExecuteNonQuery ();

            // Cat
            cmdCreateRecord.Parameters[0].Value = 3;
            cmdCreateRecord.Parameters[1].Value = "Cat";
            cmdCreateRecord.Parameters[2].Value = 15;
            cmdCreateRecord.ExecuteNonQuery ();

            // Whale
            cmdCreateRecord.Parameters[0].Value = 4;
            cmdCreateRecord.Parameters[1].Value = "Whale";
            cmdCreateRecord.Parameters[2].Value = 200;
            cmdCreateRecord.ExecuteNonQuery ();

            // Tortoise
            cmdCreateRecord.Parameters[0].Value = 5;
            cmdCreateRecord.Parameters[1].Value = "Tortoise";
            cmdCreateRecord.Parameters[2].Value = 150;
            cmdCreateRecord.ExecuteNonQuery ();
        }

        [Fact]
        public void Can_Create () {
            // arrange
            var factory = new Mock<IDbConnectionFactory> ();
            IDatabase database;

            // act
            database = new Database (factory.Object);

            // assert
            Assert.NotNull (database);
        }

        // [Fact]
        // public async void Can_Execute_Reader () {
        //     // arrange
        //     var connection = new SQLiteConnection (Constants.SQLite_ConnStr_InMemory);

        //     CreateData (connection);

        //     var factory = new Mock<IDbConnectionFactory> ();
        //         factory.Setup (_ => _.Create ()).Returns (connection);
        //     var database = new Database (factory.Object);

        //     // act
        //     var result = await database
        //         .ExecuteReaderAsync ("SELECT * FROM Animals", mapper: Animal.Map)
        //         .ToArrayAsync ();

        //     Assert.NotNull (result);
        //     Assert.Equal (5, result.Length);
        // }
    }
}
