using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.SqlServer;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    [Fact]
    public void RegisterSqlServerDatabase_RegistersRequiredServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();

        var configuration = new ConfigurationBuilder().Build();
        services.AddSingleton<IConfiguration>(configuration);

        // act
        var returned = services.RegisterSqlServerDatabase(configuration);

        // assert
        Assert.Same(services, returned);

        var provider = services.BuildServiceProvider();

        Assert.Multiple(
            () => Assert.IsType<DbConnectionFactory>(provider.GetRequiredService<IDbConnectionFactory>()),
            () => Assert.IsType<Database>(provider.GetRequiredService<IDatabase>())
        );
    }

    [Fact]
    public void RegisterSqlServerDatabase_WithNullConfiguration_StillRegistersServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());

        // act
        services.RegisterSqlServerDatabase(configuration: null);

        // assert
        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IDbConnectionFactory>());
    }
}
