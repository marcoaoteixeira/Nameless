using Autofac;
using Moq;
using Nameless.Infrastructure;
using Nameless.Lucene.DependencyInjection;

namespace Nameless.Lucene {
    public class DependencyInjectionTests {
        [Test]
        public void Register_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterLuceneModule();

            // We need an IApplicationContext
            var applicationContextMock = new Mock<IApplicationContext>();
            builder
                .RegisterInstance(applicationContextMock.Object)
                .As<IApplicationContext>();

            using var container = builder.Build();

            // act
            var service = container.Resolve<IIndexManager>();

            // assert
            Assert.Multiple(() => {
                Assert.That(service, Is.Not.Null);
                Assert.That(service, Is.InstanceOf<IndexManager>());
            });
        }
    }
}
