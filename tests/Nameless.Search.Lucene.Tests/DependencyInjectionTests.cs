using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Search.Lucene;

public class DependencyInjectionTests {
    [Fact]
    public void WhenAddsLuceneSearch_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();

        var applicationContext = new ApplicationContextMocker().WithAppDataFolderPath("\\Temp")
                                                               .Build();
        services.AddSingleton(applicationContext);

        var loggerForIndexProvider = new LoggerMocker<IndexManager>().Build();
        var loggerForIndex = new LoggerMocker<Index>().Build();
        var loggerFactory = new LoggerFactoryMocker()
                           .WithCreateLogger(loggerForIndexProvider)
                           .WithCreateLogger(loggerForIndex)
                           .Build();
        services.AddSingleton(loggerFactory);

        var options = OptionsHelper.Create<SearchOptions>();
        services.AddSingleton(options);

        services.RegisterSearch(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IIndexManager>();

        // assert
        Assert.IsType<IndexManager>(service);
    }
}