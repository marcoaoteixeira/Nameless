using Autofac;
using Nameless.Mailing.MailKit.DependencyInjection;
using Nameless.Mailing.MailKit.Impl;

namespace Nameless.Mailing.MailKit {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterMailingModule();

            using var container = builder.Build();

            // act
            var service = container.Resolve<IMailingService>();

            // assert
            Assert.Multiple(() => {
                Assert.That(service, Is.Not.Null);
                Assert.That(service, Is.InstanceOf<MailingService>());
            });
        }
    }
}