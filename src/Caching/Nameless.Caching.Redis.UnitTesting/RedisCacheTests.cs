using StackExchange.Redis;

namespace Nameless.Caching.Redis.UnitTesting {
    public class RedisCacheTests {
        private IRedisDatabaseManager _databaseManager;
        private IDatabase _database;

        [OneTimeSetUp]
        public void OneTimeSetup() {

            _databaseManager = new RedisDatabaseManager(new RedisOptions {
                Port = 55400
            });
            _database = _databaseManager.GetDatabase();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() {
            ((IDisposable)_databaseManager).Dispose();
        }

        [Test]
        public async Task Can_Set_New_Value_To_Cache() {
            // arrange
            var cache = new RedisCache(_database);
            var obj = new {
                Id = 1,
                Name = "My Test"
            };

            // act
            var result = await cache.SetAsync("cd5cfd4c-db35-49e9-b5d1-d1dcd0fed30a", obj);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Can_Get_Value_From_Cache() {
            // arrange
            var cache = new RedisCache(_database);
            var obj = new {
                Id = 1,
                Name = "My Test"
            };
            var key = "ee15493f-c1ec-444b-b61b-7461a0b4622e";

            // act
            var set = await cache.SetAsync(key, obj);
            var get = await cache.GetAsync<object>(key);

            // assert
            Assert.That(set, Is.True);
            Assert.That(get, Is.Not.Null);
        }

        [Test]
        public async Task Can_Set_Value_To_Cache_With_ExpiresIn() {
            // arrange
            var cache = new RedisCache(_database);
            var obj = new {
                Id = 1,
                Name = "My Test"
            };
            var key = "95931450-923e-46ea-8908-cf09f627af96";
            var opts = new CacheEntryOptions {
                ExpiresIn = TimeSpan.FromMilliseconds(500)
            };

            // act
            var set = await cache.SetAsync(key, obj, opts);

            await Task.Delay(TimeSpan.FromMilliseconds(750));

            var get = await cache.GetAsync<object>(key);

            // assert
            Assert.That(set, Is.True);
            Assert.That(get, Is.Null);
        }
    }
}