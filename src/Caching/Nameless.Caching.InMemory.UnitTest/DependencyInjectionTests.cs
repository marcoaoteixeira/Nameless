using Autofac;
using Nameless.Caching.InMemory.DependencyInjection;

namespace Nameless.Caching.InMemory {
    public class DependencyInjectionTests {
        [Test]
        public void Register_And_Resolve_Service() {
            // arrange
            var builder = new ContainerBuilder();
            builder.AddCaching();

            // act
            using var container = builder.Build();
            var cache = container.Resolve<ICache>();

            // assert
            Assert.That(cache, Is.InstanceOf<InMemoryCache>());
        }
    }
}