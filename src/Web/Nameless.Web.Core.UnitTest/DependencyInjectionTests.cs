using Autofac;
using Nameless.Web.DependencyInjection;
using Nameless.Web.Services;
using Nameless.Web.Services.Impl;

namespace Nameless.Web {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Web_Module() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterWebModule();
            using var container = builder.Build();

            // act
            var service = container.Resolve<IJwtService>();

            // assert
            Assert.Multiple(() => {
                Assert.That(service, Is.Not.Null);
                Assert.That(service, Is.InstanceOf<JwtService>());
            });
        }
    }
}
