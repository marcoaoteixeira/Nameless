using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace Nameless.Caching.InMemory.UnitTests {

    public class InMemoryCacheTests {

        private MemoryCacheOptions _options = new();

        [Test]
        public async Task SetAsync_Creates_Entry_To_The_Underline_Cache() {
            // arrange
            ICache cache = new InMemoryCache(_options);

            // act
            var obj = await cache.SetAsync("Test", 123456);

            // assert
            obj.Should().Be(123456);
        }

        [Test]
        public async Task SetAsync_Sets_Value_To_The_Cache() {
            // arrange
            ICache cache = new InMemoryCache(_options);

            // act
            await cache.SetAsync("Test", 123456);

            var obj = await cache.GetAsync<int>("Test");

            // assert
            obj.Should().Be(123456);
        }

        [Test]
        public async Task SetAsync_Sets_Value_To_The_Cache_With_Expiration() {
            // arrange
            ICache cache = new InMemoryCache(_options);

            // act
            var actual = await cache.SetAsync("Test", 123456, new CacheEntryOptions {
                ExpiresIn = TimeSpan.FromMilliseconds(150)
            });
            await Task.Delay(200);

            var expired = await cache.GetAsync<int>("Test");

            // assert
            actual.Should().Be(123456);
            expired.Should().Be(0);
        }

        [Test]
        public async Task SetAsync_Sets_Value_To_The_Cache_With_Expiration_And_EvictionCallback() {
            // arrange
            ICache cache = new InMemoryCache(_options);
            var innerKey = string.Empty;
            var innerValue = 0;

            void callback(string key, object? value, string? reason) {
                innerKey = key;
                innerValue = (int)value!;
            };

            // act
            var actual = await cache.SetAsync("Test", 123456, new CacheEntryOptions {
                ExpiresIn = TimeSpan.FromMilliseconds(150),
                EvictionCallback = callback
            });

            await Task.Delay(200);

            var expired = await cache.GetAsync<int>("Test");

            // assert
            actual.Should().Be(123456);
            expired.Should().Be(0);
            innerKey.Should().Be("Test");
            innerValue.Should().Be(123456);
        }
    }
}
