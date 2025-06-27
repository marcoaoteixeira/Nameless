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
            DatabasePath = DatabasePath,
            Password = Password
        };

        // assert
        Assert.Multiple(() => {
            Assert.Equal(UseInMemory, actual.UseInMemory);
            Assert.Equal(DatabasePath, actual.DatabasePath);
            Assert.Equal(Password, actual.Password);
        });
    }

    [Fact]
    public void WhenGettingUseCredentials_WhenPasswordProvided_ThenReturnsTrue() {
        // arrange
        const string Password = "Pass";

        // act
        var actual = new SqliteOptions {
            Password = Password
        };

        // assert
        Assert.True(actual.UseCredentials);
    }

    [Fact]
    public void WhenGettingUseCredentials_WhenPasswordNotProvided_ThenReturnsFalse() {
        // arrange
        const string Password = null;

        // act
        var actual = new SqliteOptions {
            Password = Password,
        };

        // assert
        Assert.False(actual.UseCredentials);
    }
}
