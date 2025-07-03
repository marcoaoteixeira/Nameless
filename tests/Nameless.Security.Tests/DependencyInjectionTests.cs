using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Security;

public class DependencyInjectionTests {
    [Fact]
    public void Register_Resolve_Security_Module_Services() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<RijndaelCrypto>().Build());
        services.RegisterCrypto();
        using var provider = services.BuildServiceProvider();

        // act
        var cryptographicService = provider.GetService<ICrypto>();
        var passwordGenerator = provider.GetService<IPasswordGenerator>();

        // assert
        Assert.Multiple(() => {
            Assert.IsType<RijndaelCrypto>(cryptographicService);
            Assert.IsType<RandomPasswordGenerator>(passwordGenerator);
        });
    }
}