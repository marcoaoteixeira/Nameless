using Xunit;

namespace Nameless.Caching.InMemory.Test {
    public class InMemoryCacheTest {
        [Fact]
        public void Can_Create () {
            // arrange
            ICache cache;

            // act
            cache = new InMemoryCache ();

            // assert
            Assert.NotNull (cache);
        }

        [Fact]
        public async void Can_Set_A_Value_Into_The_Cache () {
            // arrange
            ICache cache = new InMemoryCache ();
            var value = 123;

            // act
            await cache.SetAsync ("key", value);

            var result = await cache.GetAsync<int> ("key");

            // assert
            Assert.Equal (value, result);
        }

        [Fact]
        public async void Removes_Entry () {
            // arrange
            ICache cache = new InMemoryCache ();
            var value = 123;

            // act
            await cache.SetAsync ("key", value);

            await cache.RemoveAsync ("key");

            var valueAfter = await cache.GetAsync<int> ("key");

            // assert
            Assert.NotEqual (value, valueAfter);
        }

        [Fact]
        public async void Notify_After_Object_Eviction () {
            // arrange
            ICache cache = new InMemoryCache ();
            var value = 123;
            var key = string.Empty;
            void callback (string k, object v) {
                key = k;
            }

            // act
            await cache.SetAsync ("key", value, new NotifyCacheEntryOptions {
                EvictionCallback = callback
            });
            await cache.RemoveAsync ("key");

            // assert
            Assert.Equal ("key", key);
        }
    }
}
