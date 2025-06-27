namespace Nameless.Data.SqlServer;

public class SqlServerOptionsTests {
    [Fact]
    public void WhenInitializing_ThenCanRetrieveCorrectPropertyValues() {
        // arrange
        const string Server = "Server Location";
        const string Database = "Database Name";
        const string Username = "User";
        const string Password = "Pass";
        const bool UseAttachedDb = true;
        const bool UseIntegratedSecurity = true;

        // act
        var actual = new SqlServerOptions {
            Server = Server,
            Database = Database,
            Username = Username,
            Password = Password,
            UseAttachedDb = UseAttachedDb,
            UseIntegratedSecurity = UseIntegratedSecurity,
        };

        // assert
        Assert.Multiple(() => {
            Assert.Equal(Server, actual.Server);
            Assert.Equal(Database, actual.Database);
            Assert.Equal(Username, actual.Username);
            Assert.Equal(Password, actual.Password);
            Assert.Equal(UseAttachedDb, actual.UseAttachedDb);
            Assert.Equal(UseIntegratedSecurity, actual.UseIntegratedSecurity);
        });
    }

    [Fact]
    public void WhenGettingUseCredentials_WhenUsernamePasswordProvided_WhenNotUseIntegratedSecurity_ThenReturnsTrue() {
        // arrange
        const string Username = "User";
        const string Password = "Pass";
        const bool UseIntegratedSecurity = false;

        // act
        var actual = new SqlServerOptions {
            Username = Username,
            Password = Password,
            UseIntegratedSecurity = UseIntegratedSecurity,
        };

        // assert
        Assert.True(actual.UseCredentials);
    }

    [Fact]
    public void WhenGettingUseCredentials_WhenUsernamePasswordProvided_WhenUseIntegratedSecurity_ThenReturnsFalse() {
        // arrange
        const string Username = "User";
        const string Password = "Pass";
        const bool UseIntegratedSecurity = true;

        // act
        var actual = new SqlServerOptions {
            Username = Username,
            Password = Password,
            UseIntegratedSecurity = UseIntegratedSecurity,
        };

        // assert
        Assert.False(actual.UseCredentials);
    }

    [Fact]
    public void WhenGettingUseCredentials_WhenUsernameOrPasswordNotProvided_ThenReturnsFalse() {
        // arrange
        const string Password = "Pass";
        const bool UseIntegratedSecurity = false;

        // act
        var actual = new SqlServerOptions {
            Password = Password,
            UseIntegratedSecurity = UseIntegratedSecurity,
        };

        // assert
        Assert.False(actual.UseCredentials);
    }
}
