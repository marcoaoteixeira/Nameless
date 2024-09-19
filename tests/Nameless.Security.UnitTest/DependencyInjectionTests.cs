using Microsoft.Extensions.DependencyInjection;
using Nameless.Security.Crypto;

namespace Nameless.Security.UnitTest;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Security_Module_Services() {
        // arrange
        var services = new ServiceCollection();
        services.AddPasswordGenerator();
        services.AddCryptographicService();
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