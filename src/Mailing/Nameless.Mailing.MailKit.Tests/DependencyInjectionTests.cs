using Microsoft.Extensions.DependencyInjection;
using Nameless.Mockers;

namespace Nameless.Mailing.MailKit;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<MailingService>().Build());
        services.RegisterMailKitMailingServices(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMailingService>();

        // assert
        Assert.That(service, Is.InstanceOf<MailingService>());
    }
}