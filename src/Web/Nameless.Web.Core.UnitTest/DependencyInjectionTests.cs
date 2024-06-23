using Microsoft.Extensions.DependencyInjection;
using Nameless.Web.Services;
using Nameless.Web.Services.Impl;

namespace Nameless.Web {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Web_Module() {
            // arrange
            var services = new ServiceCollection();
            services.RegisterJwtService();
            using var provider = services.BuildServiceProvider();

            // act
            var service = provider.GetService<IJwtService>();

            // assert
            Assert.That(service, Is.InstanceOf<JwtService>());
        }
    }
}
