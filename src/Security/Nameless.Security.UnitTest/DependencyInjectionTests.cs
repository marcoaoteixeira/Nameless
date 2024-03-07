using Autofac;
using Nameless.Security.Crypto;
using Nameless.Security.DependencyInjection;

namespace Nameless.Security.UnitTest {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Security_Module_Services() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterSecurityModule();
            using var container = builder.Build();

            // act
            var cryptographicService = container.Resolve<ICryptographicService>();
            var passwordGenerator = container.Resolve<IPasswordGenerator>();

            // assert
            Assert.Multiple(() => {
                Assert.That(cryptographicService, Is.Not.Null);
                Assert.That(cryptographicService, Is.InstanceOf<RijndaelCryptographicService>());

                Assert.That(passwordGenerator, Is.Not.Null);
                Assert.That(passwordGenerator, Is.InstanceOf<RandomPasswordGenerator>());
            });
        }
    }
}