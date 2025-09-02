namespace Nameless.Data.Sqlite;

public class SqliteOptionsTests {
    [Fact]
    public void WhenInitializing_ThenCanRetrieveCorrectPropertyValues() {
        // arrange
        const bool UseInMemory = true;
        const string DatabasePath = "/temp/sqlite.db";
        const string Password = "Pass";

        // act
        var actual = new SqliteOptions {
            UseInMemory = UseInMemory,
            DatabaseFilePath = DatabasePath,
            Password = Password
        };

        // assert
        Assert.Multiple(() => {
            Assert.Equal(UseInMemory, actual.UseInMemory);
            Assert.Equal(DatabasePath, actual.DatabaseFilePath);
            Assert.Equal(Password, actual.Password);
        });
    }
}
