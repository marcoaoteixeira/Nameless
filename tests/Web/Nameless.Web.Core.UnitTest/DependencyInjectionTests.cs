using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Web.Extensions;
using Nameless.Web.Services;

namespace Nameless.Web;

public class DependencyInjectionTests {
    [Test]
    public void Register_Resolve_Web_Module() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton<ILogger<JwtService>>(NullLogger<JwtService>.Instance);
        services.AddSystemClock();
        services.AddJwtAuth(_ => { }, _ => { });
        using var provider = services.BuildServiceProvider();

        // act
        var service = provider.GetService<IJwtService>();

        // assert
        Assert.That(service, Is.InstanceOf<JwtService>());
    }
}