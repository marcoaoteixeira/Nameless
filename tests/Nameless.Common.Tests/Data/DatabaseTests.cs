using System.Data;
using Microsoft.Extensions.Logging;
using Moq;
using Nameless.Data;
using Nameless.Data.Requests;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Helpers;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Data;

[IntegrationTest]
public class DatabaseTests {
    private const string CreateTableSql =
        "CREATE TABLE IF NOT EXISTS Items (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)";

    private static Database CreateSut(string dbFileName, out IDbConnection connection) {
        connection = SqliteHelper.CreateDbConnection(dbFileName);
        connection.Open();

        // initialise schema before the Database instance touches the connection
        using var cmd = connection.CreateCommand();
        cmd.CommandText = CreateTableSql;
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
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db", out _);
        var request = new ExecuteNonQueryRequest {
            Text = "INSERT INTO Items (Name) VALUES ('Widget')",
            Type = CommandType.Text
        };

        // act
        var response = sut.ExecuteNonQuery(request);

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(1, response.Value);
        });
    }

    [Fact]
    public void ExecuteNonQuery_WithParameters_BindsValuesCorrectly() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db", out _);
        var insertRequest = new ExecuteNonQueryRequest {
            Text = "INSERT INTO Items (Name) VALUES (@name)",
            Type = CommandType.Text,
            Parameters = new ParameterCollection([
                new Parameter("@name", "Gadget", DbType.String)
            ])
        };

        var selectRequest = new ExecuteReaderRequest<string> {
            Text = "SELECT Name FROM Items WHERE Name = @name",
            Type = CommandType.Text,
            Parameters = new ParameterCollection([
                new Parameter("@name", "Gadget", DbType.String)
            ]),
            Mapper = record => record.GetString(0)
        };

        // act
        sut.ExecuteNonQuery(insertRequest);
        var readResponse = sut.ExecuteReader(selectRequest);

        // assert
        Assert.Multiple(() => {
            Assert.True(readResponse.Success);
            Assert.Single(readResponse.Value);
            Assert.Equal("Gadget", readResponse.Value[0]);
        });
    }

    [Fact]
    public void ExecuteReader_Select_ReturnsMappedResults() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db", out _);

        sut.ExecuteNonQuery(new ExecuteNonQueryRequest {
            Text = "INSERT INTO Items (Name) VALUES ('Alpha'), ('Beta'), ('Gamma')",
            Type = CommandType.Text
        });

        var request = new ExecuteReaderRequest<string> {
            Text = "SELECT Name FROM Items ORDER BY Name",
            Type = CommandType.Text,
            Mapper = record => record.GetString(0)
        };

        // act
        var response = sut.ExecuteReader(request);

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(3, response.Value.Length);
            Assert.Equal(["Alpha", "Beta", "Gamma"], response.Value);
        });
    }

    [Fact]
    public void ExecuteScalar_Count_ReturnsValue() {
        // arrange
        using var sut = CreateSut($"{Guid.CreateVersion7():N}.db", out _);

        sut.ExecuteNonQuery(new ExecuteNonQueryRequest {
            Text = "INSERT INTO Items (Name) VALUES ('One'), ('Two')",
            Type = CommandType.Text
        });

        var request = new ExecuteScalarRequest {
            Text = "SELECT COUNT(*) FROM Items",
            Type = CommandType.Text
        };

        // act
        var response = sut.ExecuteScalar<long>(request);

        // assert
        Assert.Multiple(() => {
            Assert.True(response.Success);
            Assert.Equal(2L, response.Value);
        });
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes() {
        // arrange
        var sut = CreateSut($"{Guid.CreateVersion7():N}.db", out _);

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
        var sut = CreateSut($"{Guid.CreateVersion7():N}.db", out _);
        sut.Dispose();

        var request = new ExecuteNonQueryRequest {
            Text = "SELECT 1",
            Type = CommandType.Text
        };

        // act & assert
        Assert.Throws<ObjectDisposedException>(() => sut.ExecuteNonQuery(request));
    }
}
