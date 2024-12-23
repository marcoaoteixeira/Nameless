using Microsoft.Extensions.DependencyInjection;
using Nameless.Mockers;
using NHibernate;
using NHibernate.Impl;

namespace Nameless.NHibernate;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_NHibernate_Module() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<ConfigurationFactory>().Build());
        services.AddSingleton(new ApplicationContextMocker().WithAppDataFolderPath(Path.GetTempPath())
                                                            .Build());
        services.AddNHibernate(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var session = provider.GetRequiredService<ISession>();

        // assert
        Assert.That(session, Is.InstanceOf<SessionImpl>());
    }
}