using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Web.Auth;
using Nameless.Web.Auth.Impl;
using Nameless.Web.Options;

namespace Nameless.Web;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Web_Module() {
        // arrange
        var configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(mock => mock.GetSection(It.IsAny<string>()))
                         .Returns(Mock.Of<IConfigurationSection>());
        var services = new ServiceCollection();
        services.AddSingleton<ILogger<JwtService>>(NullLogger<JwtService>.Instance);
        services.AddSystemClock();
        services.AddJwtAuth(configurationMock.Object);
        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IJwtService>();

        // assert
        Assert.That(service, Is.InstanceOf<JwtService>());
    }
}