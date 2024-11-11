using Microsoft.Extensions.DependencyInjection;
using Nameless.Infrastructure;
using Nameless.Mockers;

namespace Nameless.Lucene;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        
        services.AddSingleton(new LoggerMocker<Index>().Build());
        services.AddSingleton(new ApplicationContextMocker().Build());
        services.AddLucene(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IIndexProvider>();

        // assert
        Assert.That(service, Is.InstanceOf<IndexProvider>());
    }
}