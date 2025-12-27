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
            UseIntegratedSecurity = UseIntegratedSecurity
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
}