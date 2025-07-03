using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Sqlite;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringSQLiteServices_ThenResolveMainServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterDatabaseServices(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();
        var dbConnectionFactory = provider.GetService<IDbConnectionFactory>();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<Database>(database);
            Assert.IsType<DbConnectionFactory>(dbConnectionFactory);
        });
    }

    [Fact]
    public void WhenRegisteringSQLiteServices_WhenConfigureActionNotDefined_ThenResolveMainServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterDatabaseServices();

        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();
        var dbConnectionFactory = provider.GetService<IDbConnectionFactory>();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<Database>(database);
            Assert.IsType<DbConnectionFactory>(dbConnectionFactory);
        });
    }
}