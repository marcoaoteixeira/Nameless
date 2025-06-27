using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers;
using NHibernate;
using NHibernate.Impl;

namespace Nameless.NHibernate;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_NHibernate_Module() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<ConfigurationFactory>().Build());
        services.AddSingleton(new ApplicationContextMocker().WithAppDataFolderPath(Path.GetTempPath())
                                                            .Build());
        services.ConfigureNHibernateServices(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var session = provider.GetRequiredService<ISession>();

        // assert
        Assert.IsType<SessionImpl>(session);
    }
}