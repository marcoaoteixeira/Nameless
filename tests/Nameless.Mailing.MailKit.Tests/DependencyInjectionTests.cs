using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Mailing.MailKit;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<MailingService>().Build());
        services.RegisterMailingServices(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMailingService>();

        // assert
        Assert.That(service, Is.InstanceOf<MailingService>());
    }
}