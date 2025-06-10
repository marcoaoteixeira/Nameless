using System.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Data.Fixtures;
using Nameless.Data.Mockers;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data;

public class DatabaseTests {
    private static DataParameterCollectionMocker CreateDataParameterCollectionMocker() {
        return new DataParameterCollectionMocker().WithEmptyGetEnumerator();
    }

    private static DbCommandMocker CreateDbCommandMocker(IDataParameterCollection dataParameterCollection) {
        return new DbCommandMocker().WithCreateParameter(Quick.Mock<IDbDataParameter>())
                                    .WithParameters(dataParameterCollection);
    }

    private static Database CreateSut(IDbCommand dbCommand, ILogger<Database> logger = null) {
        var dbConnection = new DbConnectionMocker().WithCreateCommand(dbCommand).Build();
        var dbConnectionFactory = new DbConnectionFactoryMocker().WithCreateDbConnection(dbConnection).Build();

        return new Database(dbConnectionFactory, logger ?? Quick.Mock<ILogger<Database>>());
    }

    [Fact]
    public void ExecuteNonQuery_Should_Query_Against_Database() {
        // arrange
        var dataParameterCollectionMocker = CreateDataParameterCollectionMocker();
        var dbCommandMocker = CreateDbCommandMocker(dataParameterCollectionMocker.Build()).WithExecuteNonQuery(result: 1);
        var sut = CreateSut(dbCommandMocker.Build());

        // act
        var actual = sut.ExecuteNonQuery("STATEMENT", CommandType.Text, new Parameter("Param", 1, DbType.Int32));

        // assert
        Assert.Equal(expected: 1, actual);
        dbCommandMocker.Verify(mock => mock.CreateParameter());
        dataParameterCollectionMocker.Verify(mock => mock.Add(It.IsAny<object>()));
    }

    [Fact]
    public void ExecuteNonQuery_On_Error_Should_Log() {
        // arrange
        var dataParameterCollectionMocker = CreateDataParameterCollectionMocker();
        var dbCommandMocker = CreateDbCommandMocker(dataParameterCollectionMocker.Build()).WithExecuteNonQuery<InvalidOperationException>();
        var loggerMocker = new LoggerMocker<Database>().WithLogLevel(LogLevel.Error);
        var sut = CreateSut(dbCommandMocker.Build(), loggerMocker.Build());

        // act & assert
        Assert.Multiple(() => {
            Assert.Throws<InvalidOperationException>(() => sut.ExecuteNonQuery("STATEMENT", CommandType.Text, new Parameter("Param", 1, DbType.Int32)));
            dbCommandMocker.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMocker.Verify(mock => mock.Add(It.IsAny<object>()));
            loggerMocker.VerifyErrorCall();
        });
    }

    [Fact]
    public void ExecuteScalar_Should_Query_Against_Database() {
        // arrange
        var dataParameterCollectionMocker = CreateDataParameterCollectionMocker();
        var dbCommandMocker = CreateDbCommandMocker(dataParameterCollectionMocker.Build()).WithExecuteScalar("Field");
        var sut = CreateSut(dbCommandMocker.Build());

        // act
        var actual = sut.ExecuteScalar<string>("STATEMENT", CommandType.Text, new Parameter("Param", 1, DbType.Int32));

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: "Field", actual);
            dbCommandMocker.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMocker.Verify(mock => mock.Add(It.IsAny<object>()));
        });
    }

    [Fact]
    public void ExecuteScalar_On_Error_Should_Log() {
        // arrange
        var dataParameterCollectionMocker = CreateDataParameterCollectionMocker();
        var dbCommandMocker = CreateDbCommandMocker(dataParameterCollectionMocker.Build()).WithExecuteScalar<InvalidOperationException>();
        var loggerMocker = new LoggerMocker<Database>().WithLogLevel(LogLevel.Error);
        var sut = CreateSut(dbCommandMocker.Build(), loggerMocker.Build());

        // act

        // assert
        Assert.Multiple(() => {
            Assert.Throws<InvalidOperationException>(
                () => sut.ExecuteScalar<object>("STATEMENT", CommandType.Text, new Parameter("Param", 1, DbType.Int32))
            );
            dbCommandMocker.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMocker.Verify(mock => mock.Add(It.IsAny<object>()));
            loggerMocker.VerifyErrorCall();
        });
    }

    [Fact]
    public void ExecuteReader_Should_Query_Against_Database() {
        // arrange
        var dataReader = new DataReaderMocker().WithReadSequence(true, false).Build();
        var dataParameterCollectionMocker = CreateDataParameterCollectionMocker();
        var dbCommandMocker = CreateDbCommandMocker(dataParameterCollectionMocker.Build()).WithExecuteReader(dataReader);
        var sut = CreateSut(dbCommandMocker.Build());

        var expected = new Animal { Name = "Dog" };

        // act
        var actual = sut.ExecuteReader(
                             "STATEMENT",
                             CommandType.Text,
                             _ => new Animal { Name = "Dog" },
                             new Parameter("Param", 1, DbType.Int32))
                        .ToArray();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected, actual[0]);
            dbCommandMocker.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMocker.Verify(mock => mock.Add(It.IsAny<object>()));
        });
    }

    [Fact]
    public void ExecuteReader_On_Error_Should_Log() {
        // arrange
        var dataParameterCollectionMocker = CreateDataParameterCollectionMocker();
        var dbCommandMocker = CreateDbCommandMocker(dataParameterCollectionMocker.Build()).WithExecuteReader<InvalidOperationException>();
        var loggerMocker = new LoggerMocker<Database>().WithLogLevel(LogLevel.Error);
        var sut = CreateSut(dbCommandMocker.Build(), loggerMocker.Build());

        // act

        // assert
        Assert.Multiple(() => {
            Assert.Throws<InvalidOperationException>(
                () => _ = sut.ExecuteReader("STATEMENT",
                    CommandType.Text,
                    _ => new Animal { Name = "ERROR" },
                    new Parameter("Param", 1, DbType.Int32)).ToArray()
            );
            dbCommandMocker.Verify(mock => mock.CreateParameter());
            dataParameterCollectionMocker.Verify(mock => mock.Add(It.IsAny<object>()));
            loggerMocker.VerifyErrorCall();
        });
    }

    [Fact]
    public void StartTransaction_Should_Start_A_New_Transaction() {
        // arrange
        var dbConnectionMocker = new DbConnectionMocker()
                                .WithBeginTransaction(Quick.Mock<IDbTransaction>());

        var dbConnectionFactory = new DbConnectionFactoryMocker()
                                 .WithCreateDbConnection(dbConnectionMocker.Build())
                                 .Build();

        var sut = new Database(dbConnectionFactory, Mock.Of<ILogger<Database>>());

        // act
        sut.BeginTransaction(IsolationLevel.Unspecified);

        // assert
        dbConnectionMocker.Verify(mock => mock.BeginTransaction(It.IsAny<IsolationLevel>()));
    }
}