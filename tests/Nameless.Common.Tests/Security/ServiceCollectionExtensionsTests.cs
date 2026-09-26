using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nameless.Security.Cryptography;
using Nameless.Security.Password;

namespace Nameless.Security;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    private static ServiceCollection CreateServices() {
        var services = new ServiceCollection();
        services.AddLogging();

        return services;
    }

    [Fact]
    public void RegisterSecurity_RegistersPasswordGeneratorAndCrypto() {
        // arrange
        var services = CreateServices();

        // act
        var returned = services.RegisterSecurity();
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.IsType<RandomPassGenerator>(provider.GetRequiredService<IPassGenerator>()),
            () => Assert.IsType<RijndaelCrypto>(provider.GetRequiredService<ICrypto>())
        );
    }

    [Fact]
    public void RegisterSecurity_BindsOptionsFromConfiguration() {
        // arrange
        var services = CreateServices();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["RijndaelCrypto:Passphrase"] = "custom-pass-phrase" })
            .Build();

        // act
        services.RegisterSecurity(configuration);
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.Equal("custom-pass-phrase", provider.GetRequiredService<IOptions<RijndaelCryptoOptions>>().Value.Passphrase);
    }

    [Fact]
    public void RegisterSecurity_RegistersSingletons() {
        // arrange
        var services = CreateServices();
        services.RegisterSecurity();
        using var provider = services.BuildServiceProvider();

        // act & assert
        Assert.Same(provider.GetRequiredService<ICrypto>(), provider.GetRequiredService<ICrypto>());
    }
}
