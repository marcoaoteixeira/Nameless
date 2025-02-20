using Microsoft.Extensions.DependencyInjection;
using Nameless.Mockers;
using Nameless.Search.Lucene.Mockers;
using Nameless.Search.Lucene.Options;

namespace Nameless.Search.Lucene;

public class DependencyInjectionTests {
    [Test]
    public void WhenAddsLuceneSearch_ThenResolveServices() {
        // arrange
        var services = new ServiceCollection();

        var applicationContext = new ApplicationContextMocker().Build();
        services.AddSingleton(applicationContext);

        var fileSystem = new FileSystemMocker().Build();
        services.AddSingleton(fileSystem);

        var loggerForIndexProvider = new LoggerMocker<IndexProvider>().Build();
        var loggerForIndex = new LoggerMocker<Index>().Build();
        var loggerFactory = new LoggerFactoryMocker()
            .WithCreateLogger(loggerForIndexProvider)
            .WithCreateLogger(loggerForIndex)
            .Build();
        services.AddSingleton(loggerFactory);

        var options = new OptionsMocker<LuceneOptions>().Build();
        services.AddSingleton(options);

        services.RegisterLuceneSearch(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IIndexProvider>();

        // assert
        Assert.That(service, Is.InstanceOf<IndexProvider>());
    }
}