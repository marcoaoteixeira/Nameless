using Autofac;
using Nameless.Caching.Redis.DependencyInjection;

namespace Nameless.Caching.Redis {
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
            Assert.That(sut, Is.InstanceOf<RedisCache>());
        }
    }
}