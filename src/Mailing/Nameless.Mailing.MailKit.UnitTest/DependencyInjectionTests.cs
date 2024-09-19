using Microsoft.Extensions.DependencyInjection;
using Nameless.Mailing.MailKit.Impl;

namespace Nameless.Mailing.MailKit;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddMailing();

        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IMailingService>();

        // assert
        Assert.That(service, Is.InstanceOf<MailingService>());
    }
}