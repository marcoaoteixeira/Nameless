using Autofac;
using Nameless.Caching.InMemory.DependencyInjection;

namespace Nameless.Caching.InMemory {
    public class DependencyInjectionTests {
        [Test]
        public void Register_And_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.RegisterCachingModule();
            using var container = builder.Build();

            // act
            var sut = container.Resolve<ICache>();

            // assert
            Assert.That(sut, Is.InstanceOf<InMemoryCache>());
        }
    }
}