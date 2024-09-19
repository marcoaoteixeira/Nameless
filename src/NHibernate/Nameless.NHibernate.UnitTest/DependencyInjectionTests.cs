using Microsoft.Extensions.DependencyInjection;
using Moq;
using Nameless.Infrastructure;
using NHibernate;
using NHibernate.Impl;

namespace Nameless.NHibernate;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_NHibernate_Module() {
        // arrange
        var services = new ServiceCollection();
        services.AddNHibernate();

        var applicationContextMock = new Mock<IApplicationContext>();
        applicationContextMock
            .Setup(mock => mock.ApplicationDataFolderPath)
            .Returns(Path.GetTempPath());

        services.AddSingleton(applicationContextMock.Object);

        using var provider = services.BuildServiceProvider();

        // act
        var session = provider.GetRequiredService<ISession>();

        // assert
        Assert.That(session, Is.InstanceOf<SessionImpl>());
    }
}