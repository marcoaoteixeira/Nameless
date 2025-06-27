﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.MongoDB;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_MongoDB_Services() {
        // arrange
        var mongoDbOptionsLogger = Mock.Of<ILogger<MongoOptions>>();
        var services = new ServiceCollection();
        services.AddSingleton(mongoDbOptionsLogger);
        services.ConfigureMongoServices(opts => {
            opts.DatabaseName = "sample";
            opts.Assemblies = [typeof(DependencyInjectionTests).Assembly];
        });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMongoCollectionProvider>();

        // assert
        Assert.IsType<MongoCollectionProvider>(service);
    }
}