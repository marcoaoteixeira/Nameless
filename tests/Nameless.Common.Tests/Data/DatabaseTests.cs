using System.Data;
using Moq;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Data;

[IntegrationTest]
public class DatabaseTests {
    private const string CREATE_TABLE_SQL =
        "CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)";

    private static Database CreateSut(string dbFileName) {
        var connection = SqliteHelper.CreateDbConnection(dbFileName);
        connection.Open();

        // initialise schema before the Database instance touches the connection
        using var cmd = connection.CreateCommand();
        cmd.CommandText = CREATE_TABLE_SQL;
        cmd.ExecuteNonQuery();
        connection.Close();

        var capturedConnection = connection;
        var factoryMock = new Mock<IDbConnectionFactory>();
        factoryMock
            .Setup(f => f.CreateDbConnection())
            .Returns(capturedConnection);

        var logger = new LoggerMocker<Database>()
            .WithAnyLogLevel()
            .Build();

        return new Database(factoryMock.Object, logger);
    }

    [Fact]
    public void ExecuteNonQuery_Insert_ReturnsAffectedRowCount() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");
        const string Sql = "INSERT INTO Items (Name) VALUES ('Widget')";

        // act
        var response = sut.ExecuteNonQuery(Sql);

        // assert
        Assert.Multiple(
            () => Assert.True(response.Success),
            () => Assert.Equal(1, response.Value)
        );
    }

    [Fact]
    public void ExecuteNonQuery_WithParameters_BindsValuesCorrectly() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");
        const string InsertSql = "INSERT INTO Items (Name) VALUES (@name)";
        var insertParameters = new ParameterCollection([
            new Parameter("@name", "Gadget")
        ]);

        const string SelectSql = "SELECT Name FROM Items WHERE Name = @name";
        var selectParameters = new ParameterCollection([
            new Parameter("@name", "Gadget")
        ]);

        // act
        sut.ExecuteNonQuery(InsertSql, parameters: insertParameters);
        var readResponse = sut.ExecuteReader(SelectSql, ReaderMapper, parameters: selectParameters);

        // assert
        Assert.Multiple(
            () => Assert.True(readResponse.Success),
            () => Assert.Single(readResponse.Value),
            () => Assert.Equal("Gadget", readResponse.Value[0])
        );
    }

    [Fact]
    public void ExecuteReader_Select_ReturnsMappedResults() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");

        const string NonQuerySql = "INSERT INTO Items (Name) VALUES ('Alpha'), ('Beta'), ('Gamma')";

        sut.ExecuteNonQuery(NonQuerySql);

        const string ReaderSql = "SELECT Name FROM Items ORDER BY Name";

        // act
        var response = sut.ExecuteReader(ReaderSql, ReaderMapper);

        // assert
        Assert.Multiple(
            () => Assert.True(response.Success),
            () => Assert.Equal(3, response.Value.Length),
            () => Assert.Equal(["Alpha", "Beta", "Gamma"], response.Value)
        );
    }

    [Fact]
    public void ExecuteScalar_Count_ReturnsValue() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");

        const string NonQuerySql = "INSERT INTO Items (Name) VALUES ('One'), ('Two')";

        sut.ExecuteNonQuery(NonQuerySql);

        const string ScalarSql = "SELECT COUNT(*) FROM Items";

        // act
        var response = sut.ExecuteScalar<long>(ScalarSql);

        // assert
        Assert.Multiple(
            () => Assert.True(response.Success),
            () => Assert.Equal(2L, response.Value)
        );
    }

    [Fact]
    public void BeginTransaction_ReturnsUsableTransaction() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");

        // act
        using var transaction = sut.BeginTransaction(IsolationLevel.ReadCommitted);

        // assert
        Assert.NotNull(transaction);
    }

    [Fact]
    public void ExecuteNonQuery_WithInvalidSql_ReturnsFailureResponse() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");
        const string NonQuerySql = "INSERT INTO NonExistentTable (Name) VALUES ('Widget')";

        // act
        var response = sut.ExecuteNonQuery(NonQuerySql);

        // assert
        Assert.False(response.Success);
    }

    [Fact]
    public void ExecuteReader_WithInvalidSql_ReturnsFailureResponse() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");
        const string ReaderSql = "SELECT Name FROM NonExistentTable";

        // act
        var response = sut.ExecuteReader(ReaderSql, ReaderMapper);

        // assert
        Assert.False(response.Success);
    }

    [Fact]
    public void ExecuteScalar_WithInvalidSql_ReturnsFailureResponse() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db");
        const string ScalarSql = "SELECT COUNT(*) FROM NonExistentTable";

        // act
        var response = sut.ExecuteScalar<long>(ScalarSql);

        // assert
        Assert.False(response.Success);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes() {
        // arrange
        var sut = CreateSut($"{Guid.CreateVersion7():N}.db");

        // act
        var exception = Record.Exception(() => {
            sut.Dispose();
            sut.Dispose();
        });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void AfterDispose_ThrowsObjectDisposedException() {
        // arrange
        var sut = CreateSut($"{Guid.CreateVersion7():N}.db");
        sut.Dispose();

        const string NonQuerySql = "SELECT 1";

        // act & assert
        Assert.Throws<ObjectDisposedException>(() => sut.ExecuteNonQuery(NonQuerySql));
    }

    private static string ReaderMapper(IDataRecord record) {
        return record.GetString(0);
    }
}
