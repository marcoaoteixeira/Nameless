using Autofac;
using Moq;
using Nameless.Infrastructure;
using Nameless.NHibernate.DependencyInjection;
using NHibernate;
using NHibernate.Impl;

namespace Nameless.NHibernate {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_NHibernate_Module() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterNHibernateModule();

            var applicationContextMock = new Mock<IApplicationContext>();
            applicationContextMock
                .Setup(mock => mock.ApplicationDataFolderPath)
                .Returns(Path.GetTempPath());

            builder
                .RegisterInstance(applicationContextMock.Object)
                .As<IApplicationContext>()
                .SingleInstance();

            using var container = builder.Build();

            // act
            var session = container.Resolve<ISession>();

            // assert
            Assert.Multiple(() => {
                Assert.That(session, Is.Not.Null);
                Assert.That(session, Is.InstanceOf<SessionImpl>());
            });
        }
    }
}