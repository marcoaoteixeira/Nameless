using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        services.AddJwtAuth(configurationMock.Object);
        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IJwtService>();

        // assert
        Assert.That(service, Is.InstanceOf<JwtService>());
    }
}