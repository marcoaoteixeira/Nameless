using Autofac;

namespace Nameless.Caching.Redis.UnitTesting {
    public class CacheModuleTests {

        [Test]
        public void Can_Initialize_Cache_Module() {
            var builder = new ContainerBuilder();

            builder
                .RegisterInstance(new RedisOptions { Port = 55400 });
            builder
                .RegisterModule<CachingModule>();

            using var container = builder.Build();

            var cache = container.Resolve<ICache>();

            Assert.That(cache, Is.Not.Null);
            Assert.That(cache, Is.InstanceOf<RedisCache>());
        }
    }
}
