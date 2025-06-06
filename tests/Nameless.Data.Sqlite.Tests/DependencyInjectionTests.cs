using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Data.Sqlite;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Dependency_Injection() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterDataServices(_ => { });
        using var provider = services.BuildServiceProvider();

        // act
        var sut = provider.GetService<IDatabase>();

        // assert
        Assert.IsAssignableFrom<IDatabase>(sut);
    }
}