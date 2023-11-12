using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Nameless.Caching.InMemory {
    public class CacheTests {
        [Test]
        public async Task Set_New_Object_To_Cache() {
            // arrange
            ICacheEntry entry = new FakeCacheEntry();
            var value = new {
                Id = 1,
                Name = "Test"
            };

            var memoryCache = new Mock<IMemoryCache>();
            memoryCache
                .Setup(_ => _.CreateEntry(It.IsAny<object>()))
                .Returns(entry)
                .Verifiable();

            var cache = new InMemoryCache(memoryCache.Object);

            // act
            var result = await cache.SetAsync("Key", value);
            
            // assert
            Assert.Multiple(() => {
                Assert.That(result, Is.True);
                Assert.DoesNotThrow(memoryCache.VerifyAll);
            });
        }

        [Test]
        public async Task Get_Object_From_Cache() {
            // arrange
            object? value = new {
                Id = 1,
                Name = "Test"
            };

            var memoryCache = new Mock<IMemoryCache>();
            memoryCache
                .Setup(_ => _.TryGetValue(It.IsAny<object>(), out value))
                .Returns(true)
                .Verifiable();

            var cache = new InMemoryCache(memoryCache.Object);

            // act
            var result = await cache.GetAsync<object?>("Key");

            // assert
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Property("Id").EqualTo(1));
                Assert.That(result, Has.Property("Name").EqualTo("Test"));
                Assert.DoesNotThrow(memoryCache.VerifyAll);
            });
        }

        [Test]
        public async Task Remove_Object_From_Cache() {
            var memoryCache = new Mock<IMemoryCache>();
            memoryCache
                .Setup(_ => _.Remove(It.IsAny<string>()))
                .Verifiable();

            var cache = new InMemoryCache(memoryCache.Object);

            // act
            var result = await cache.RemoveAsync("Key");

            // assert
            Assert.Multiple(() => {
                Assert.That(result, Is.True);
                Assert.DoesNotThrow(memoryCache.VerifyAll);
            });
        }
    }
}
