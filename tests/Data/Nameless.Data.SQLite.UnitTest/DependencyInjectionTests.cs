﻿using Microsoft.Extensions.DependencyInjection;
using Nameless.Mockers;

namespace Nameless.Data.SQLite;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Dependency_Injection() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<Database>().Build());
        services.RegisterSqliteDatabase(_ => { });
        using var provider = services.BuildServiceProvider();

        // act
        var database = provider.GetService<IDatabase>();

        // assert
        Assert.That(database, Is.InstanceOf<Database>());
    }
}