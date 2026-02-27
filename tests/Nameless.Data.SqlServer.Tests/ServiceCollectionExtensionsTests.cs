using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Data.SqlServer;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringServices_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterSqlServer(_ => { });
        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();

        // assert
        Assert.IsType<Database>(database);
    }

    [Fact]
    public void WhenRegisteringServices_WhenConfigureOptionsIsNull_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterSqlServer(configure: null);
        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();

        // assert
        Assert.IsType<Database>(database);
    }
}