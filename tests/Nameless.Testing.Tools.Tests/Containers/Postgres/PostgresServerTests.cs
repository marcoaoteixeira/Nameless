using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Containers.Postgres;

namespace Nameless.Common.Testing.Tools.Containers.Postgres;

[UnitTest]
public class PostgresServerTests
{
    [Fact]
    public async Task Can_Open_Postgres_Connection_Container()
    {
        // arrange & act
        var exception = await Record.ExceptionAsync(async () =>
        {
            await using var sut = new PostgresServer();
            await sut.InitializeAsync();

            await using var conn = sut.GetDbConnection();
            await conn.OpenAsync(TestContext.Current.CancellationToken);
        });

        // assert
        Assert.Null(exception);
    }
}
