using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Mailing.MailKit;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Service() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<MailingImpl>().Build());
        services.RegisterMailing(_ => { });

        using var provider = services.BuildServiceProvider();

        // act
        var sut = provider.GetService<IMailing>();

        // assert
        Assert.IsType<MailingImpl>(sut);
    }
}