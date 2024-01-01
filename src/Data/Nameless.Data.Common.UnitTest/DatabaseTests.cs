using System.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Data.Fixtures;

namespace Nameless.Data {
    public class DatabaseTests {
        private Mock<IDataParameterCollection> CreateDataParameterCollectionMock() {
            var dataParameterCollectionMock = new Mock<IDataParameterCollection>();
            dataParameterCollectionMock
                .Setup(mock => mock.Add(It.IsAny<object>()))
                .Returns(1);

            return dataParameterCollectionMock;
        }

        private Mock<IDbCommand> CreateDbCommandMock(IDataParameterCollection dataParameterCollection) {
            var dbCommandMock = new Mock<IDbCommand>();
            dbCommandMock
                .Setup(mock => mock.CreateParameter())
                .Returns(Mock.Of<IDbDataParameter>());

            dbCommandMock
                .Setup(mock => mock.Parameters)
                .Returns(dataParameterCollection);

            return dbCommandMock;
        }

        private Mock<IDbConnection> CreateDbConnectionMock(IDbCommand? dbCommand = null) {
            var dbConnectionMock = new Mock<IDbConnection>();

            if (dbCommand is not null) {
                dbConnectionMock
                    .Setup(mock => mock.CreateCommand())
                    .Returns(dbCommand);
            }

            return dbConnectionMock;
        }

        private Mock<ILogger> CreateLoggerMock(LogLevel enabledLevel = LogLevel.Error) {
            var loggerMock = new Mock<ILogger>();

            loggerMock
                .Setup(mock => mock.IsEnabled(enabledLevel))
                .Returns(true);

            return loggerMock;
        }

        private Mock<IDataReader> CreateDataReaderMock() {
            return new Mock<IDataReader>();
        }

        [Test]
        public void ExecuteNonQuery_Should_Query_Against_Database() {
            // arrange
            var dataParameterCollectionMock = CreateDataParameterCollectionMock();
            var dbCommandMock = CreateDbCommandMock(dataParameterCollectionMock.Object);

            dbCommandMock
                .Setup(mock => mock.ExecuteNonQuery())
                .Returns(1);

            var dbConnectionMock = CreateDbConnectionMock(dbCommandMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            var actual = sut.ExecuteNonQuery("STATEMENT", CommandType.Text, [
                new Parameter("Param", 1, DbType.Int32)
            ]);

            // assert
            Assert.That(actual, Is.EqualTo(1));
            dbCommandMock.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMock.Verify(mock => mock.Add(It.IsAny<object>()));
        }

        [Test]
        public void ExecuteNonQuery_On_Error_Should_Log() {
            // arrange
            var dataParameterCollectionMock = CreateDataParameterCollectionMock();
            var dbCommandMock = CreateDbCommandMock(dataParameterCollectionMock.Object);

            dbCommandMock
                .Setup(mock => mock.ExecuteNonQuery())
                .Throws<InvalidOperationException>();

            var dbConnectionMock = CreateDbConnectionMock(dbCommandMock.Object);
            var loggerMock = CreateLoggerMock();

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: loggerMock.Object
            );

            // act

            // assert
            Assert.Throws<InvalidOperationException>(
                code: () => sut.ExecuteNonQuery("STATEMENT", CommandType.Text, [
                    new Parameter("Param", 1, DbType.Int32)
                ])
            );
            dbCommandMock.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMock.Verify(mock => mock.Add(It.IsAny<object>()));
            loggerMock
                .Verify(mock => mock.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((obj, type) => type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ));
        }

        [Test]
        public void ExecuteScalar_Should_Query_Against_Database() {
            // arrange
            var dataParameterCollectionMock = CreateDataParameterCollectionMock();
            var dbCommandMock = CreateDbCommandMock(dataParameterCollectionMock.Object);

            dbCommandMock
                .Setup(mock => mock.ExecuteScalar())
                .Returns("Field");

            var dbConnectionMock = CreateDbConnectionMock(dbCommandMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            var actual = sut.ExecuteScalar<string>("STATEMENT", CommandType.Text, [
                new Parameter("Param", 1, DbType.Int32)
            ]);

            // assert
            Assert.That(actual, Is.EqualTo("Field"));
            dbCommandMock.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMock.Verify(mock => mock.Add(It.IsAny<object>()));
        }

        [Test]
        public void ExecuteScalar_On_Error_Should_Log() {
            // arrange
            var dataParameterCollectionMock = CreateDataParameterCollectionMock();
            var dbCommandMock = CreateDbCommandMock(dataParameterCollectionMock.Object);

            dbCommandMock
                .Setup(mock => mock.ExecuteScalar())
                .Throws<InvalidOperationException>();

            var dbConnectionMock = CreateDbConnectionMock(dbCommandMock.Object);
            var loggerMock = CreateLoggerMock();

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: loggerMock.Object
            );

            // act

            // assert
            Assert.Throws<InvalidOperationException>(
                code: () => sut.ExecuteScalar<object>("STATEMENT", CommandType.Text, [
                    new Parameter("Param", 1, DbType.Int32)
                ])
            );
            dbCommandMock.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMock.Verify(mock => mock.Add(It.IsAny<object>()));
            loggerMock
                .Verify(mock => mock.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((obj, type) => type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ));
        }

        [Test]
        public void ExecuteReader_Should_Query_Against_Database() {
            // arrange
            var dataParameterCollectionMock = CreateDataParameterCollectionMock();
            var dbCommandMock = CreateDbCommandMock(dataParameterCollectionMock.Object);
            var dataReaderMock = CreateDataReaderMock();

            dataReaderMock
                .SetupSequence(mock => mock.Read())
                .Returns(true)
                .Returns(false);

            dbCommandMock
                .Setup(mock => mock.ExecuteReader())
                .Returns(dataReaderMock.Object);

            var dbConnectionMock = CreateDbConnectionMock(dbCommandMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );
            var expected = new Animal { Name = "Dog" };
            static Animal mapper(IDataRecord reader) {
                return new Animal { Name = "Dog" };
            }

            // act
            var actual = sut.ExecuteReader("STATEMENT", mapper, CommandType.Text, [
                new Parameter("Param", 1, DbType.Int32)
            ]).ToArray();

            // assert
            Assert.That(actual.First(), Is.EqualTo(expected));
            dbCommandMock.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMock.Verify(mock => mock.Add(It.IsAny<object>()));
        }

        [Test]
        public void ExecuteReader_On_Error_Should_Log() {
            // arrange
            var dataParameterCollectionMock = CreateDataParameterCollectionMock();
            var dbCommandMock = CreateDbCommandMock(dataParameterCollectionMock.Object);

            dbCommandMock
                .Setup(mock => mock.ExecuteReader())
                .Throws<InvalidOperationException>();

            var dbConnectionMock = CreateDbConnectionMock(dbCommandMock.Object);
            var loggerMock = CreateLoggerMock();

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: loggerMock.Object
            );

            // act

            // assert
            Assert.Throws<InvalidOperationException>(
                code: () => sut.ExecuteReader("STATEMENT", (record) => new Animal { Name = "ERROR" }, CommandType.Text, [
                    new Parameter("Param", 1, DbType.Int32)
                ]).ToArray()
            );
            dbCommandMock.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMock.Verify(mock => mock.Add(It.IsAny<object>()));
            loggerMock
                .Verify(mock => mock.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((obj, type) => type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ));
        }

        [Test]
        public void StartTransaction_Should_Start_A_New_Transaction() {
            // arrange
            var dbConnectionMock = CreateDbConnectionMock();
            dbConnectionMock
                .Setup(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(Mock.Of<IDbTransaction>());

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            sut.StartTransaction();

            // assert
            dbConnectionMock.Verify(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()));
        }

        [Test]
        public void CommitTransaction_Should_Commit_Current_Transaction() {
            // arrange
            var dbTransactioMock = new Mock<IDbTransaction>();

            var dbConnectionMock = CreateDbConnectionMock();
            dbConnectionMock
                .Setup(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(dbTransactioMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            sut.StartTransaction();
            sut.CommitTransaction();

            // assert
            dbConnectionMock.Verify(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()));
            dbTransactioMock.Verify(mock => mock.Commit());
        }

        [Test]
        public void RollbackTransaction_Should_Rollback_Current_Transaction() {
            // arrange
            var dbTransactioMock = new Mock<IDbTransaction>();

            var dbConnectionMock = CreateDbConnectionMock();
            dbConnectionMock
                .Setup(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(dbTransactioMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            sut.StartTransaction();
            sut.RollbackTransaction();

            // assert
            dbConnectionMock.Verify(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()));
            dbTransactioMock.Verify(mock => mock.Rollback());
        }

        [Test]
        public void Call_Dispose_Should_Rollback_And_Dispose_Current_Transaction_If_Exists() {
            // arrange
            var dbTransactioMock = new Mock<IDbTransaction>();

            var dbConnectionMock = CreateDbConnectionMock();
            dbConnectionMock
                .Setup(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(dbTransactioMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            sut.StartTransaction();
            sut.Dispose();

            // assert
            dbConnectionMock.Verify(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()));
            dbTransactioMock.Verify(mock => mock.Rollback());
            dbTransactioMock.Verify(mock => mock.Dispose());
        }

        [Test]
        public void Call_Two_Times_Dispose_Should_Rollback_And_Dispose_Current_Transaction_Just_Once() {
            // arrange
            var dbTransactioMock = new Mock<IDbTransaction>();

            var dbConnectionMock = CreateDbConnectionMock();
            dbConnectionMock
                .Setup(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(dbTransactioMock.Object);

            var sut = new Database(
                dbConnection: dbConnectionMock.Object,
                logger: Mock.Of<ILogger>()
            );

            // act
            sut.StartTransaction();
            sut.Dispose();
            sut.Dispose();

            // assert
            dbConnectionMock.Verify(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()));
            dbTransactioMock.Verify(mock => mock.Rollback(), Times.Once);
            dbTransactioMock.Verify(mock => mock.Dispose(), Times.Once);
        }
    }
}
