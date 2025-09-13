using Microsoft.Extensions.DependencyInjection;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Security;

public class ServiceCollectionExtensionsTests {
    [Fact]
    public void WhenRegisteringCryptoService_ThenResolveCryptoService() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton(new LoggerMocker<RijndaelCrypto>().Build());
        services.RegisterCrypto();
        using var provider = services.BuildServiceProvider();

        // act
        var crypto = provider.GetService<ICrypto>();

        // assert
        Assert.IsType<RijndaelCrypto>(crypto);
    }

    [Fact]
    public void WhenRegisteringPasswordGenerator_ThenResolvePasswordGenerator() {
        // arrange
        var services = new ServiceCollection();
        services.RegisterPasswordGenerator();
        using var provider = services.BuildServiceProvider();

        // act
        var passwordGenerator = provider.GetService<IPasswordGenerator>();

        // assert
        Assert.IsType<RandomPasswordGenerator>(passwordGenerator);
    }
}