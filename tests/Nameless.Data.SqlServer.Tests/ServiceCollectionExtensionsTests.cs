using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.SqlServer;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringServices_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterDatabase(_ => { });
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
    public void WhenRegisteringServices_WhenConfigureOptionsIsNull_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterDatabase(configure: null);
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