using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers.Configuration;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Data.Sqlite;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringSQLiteServices_ThenResolveMainServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new ConfigurationMocker().Build());
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterSqlite(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();

        // assert
        Assert.IsType<Database>(database);
    }

    [Fact]
    public void WhenRegisteringSQLiteServices_WhenConfigureActionNotDefined_ThenResolveMainServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new ConfigurationMocker().Build());
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterSqlite();

        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();

        // assert
        Assert.IsType<Database>(database);
    }
}