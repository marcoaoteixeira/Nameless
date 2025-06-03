using Microsoft.Extensions.DependencyInjection;
using Nameless.Security.Crypto;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Security;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Security_Module_Services() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<RijndaelCryptographicService>().Build());
        services.RegisterSecurityServices();
        using var provider = services.BuildServiceProvider();

        // act
        var cryptographicService = provider.GetService<ICryptographicService>();
        var passwordGenerator = provider.GetService<IPasswordGenerator>();

        // assert
        Assert.Multiple(() => {
            Assert.That(cryptographicService, Is.InstanceOf<RijndaelCryptographicService>());
            Assert.That(passwordGenerator, Is.InstanceOf<RandomPasswordGenerator>());
        });
    }
}