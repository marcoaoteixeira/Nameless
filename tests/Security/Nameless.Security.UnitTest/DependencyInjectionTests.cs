using Microsoft.Extensions.DependencyInjection;
using Nameless.Mockers;
using Nameless.Security.Crypto;

namespace Nameless.Security;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Security_Module_Services() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<RijndaelCryptographicService>().Build());
        services.AddRandomPasswordGenerator();
        services.AddRijndaelCryptographicService(_ => { });
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