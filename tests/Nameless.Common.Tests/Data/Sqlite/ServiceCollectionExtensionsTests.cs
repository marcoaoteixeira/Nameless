using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.Sqlite;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    [Fact]
    public void RegisterSqliteDatabase_RegistersRequiredServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();

        var configuration = new ConfigurationBuilder().Build();
        services.AddSingleton<IConfiguration>(configuration);

        // act
        var returned = services.RegisterSqliteDatabase(configuration);

        // assert
        Assert.Same(services, returned);

        var provider = services.BuildServiceProvider();

        Assert.Multiple(
            () => Assert.IsType<DbConnectionFactory>(provider.GetRequiredService<IDbConnectionFactory>()),
            () => Assert.IsType<Database>(provider.GetRequiredService<IDatabase>())
        );
    }

    [Fact]
    public void RegisterSqliteDatabase_WithNullConfiguration_StillRegistersServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        // act
        services.RegisterSqliteDatabase(configuration: null);

        // assert
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IDbConnectionFactory>());
    }
}
