using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Nameless.Data.SQLite;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Dependency_Injection() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterDatabaseService();
        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabaseService>();

        // assert
        Assert.That(database, Is.InstanceOf<DatabaseService>());
    }
}