using Autofac;
using Moq;
using Nameless.Infrastructure;
using Nameless.Messenger.Email.DependencyInjection;
using Nameless.Messenger.Email.Impl;

namespace Nameless.Messenger.Email {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterMessengerModule();

            // We need an IApplicationContext
            var applicationContextMock = new Mock<IApplicationContext>();
            builder
                .RegisterInstance(applicationContextMock.Object)
                .As<IApplicationContext>();

            using var container = builder.Build();

            // act
            var service = container.Resolve<IMessenger>();

            // assert
            Assert.Multiple(() => {
                Assert.That(service, Is.Not.Null);
                Assert.That(service, Is.InstanceOf<EmailMessenger>());
            });
        }
    }
}