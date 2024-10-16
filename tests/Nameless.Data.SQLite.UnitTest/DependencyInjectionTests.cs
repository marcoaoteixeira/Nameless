﻿using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Data.SQLite;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Dependency_Injection() {
        // arrange
        var services = new ServiceCollection();
        services.AddSQLite();
        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabaseService>();

        // assert
        Assert.That(database, Is.InstanceOf<DatabaseService>());
    }
}