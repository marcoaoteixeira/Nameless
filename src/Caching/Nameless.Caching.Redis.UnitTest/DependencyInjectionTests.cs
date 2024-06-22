using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Caching.Redis {
    public class DependencyInjectionTests {
        [Test]
        public void Register_And_Resolve_Service() {
            // arrange
            var services = new ServiceCollection();
            services.RegisterCaching();
            using var provider = services.BuildServiceProvider();

            // act
            var sut = provider.GetRequiredService<ICacheService>();

            // assert
            Assert.That(sut, Is.InstanceOf<RedisCacheService>());
        }
    }
}