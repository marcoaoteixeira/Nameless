using System.Text.Json;
using Moq;
using StackExchange.Redis;

namespace Nameless.Caching.Redis.UnitTesting {
    public class RedisCacheTests {

        [Test]
        public async Task Can_Set_New_Value_To_Cache() {
            // arrange
            var key = "Test";
            var value = new {
                Id = 1,
                Name = "Test"
            };
            var json = JsonSerializer.Serialize(value);
            var database = new Mock<IDatabase>();
            database
                .Setup(_ => _.StringSetAsync(key, json, It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true)
                .Verifiable();
            var cache = new RedisCache(database.Object);

            // act
            var set = await cache.SetAsync(key, value);

            // assert
            Assert.Multiple(() => {
                Assert.That(set, Is.True);
                Assert.That(database.VerifyAll, Throws.Nothing);
            });
        }

        [Test]
        public async Task Can_Get_Value_From_Cache() {
            // arrange
            var key = "Test";
            var value = new {
                Id = 1,
                Name = "Test"
            };
            var json = JsonSerializer.Serialize(value);
            var database = new Mock<IDatabase>();
            database
                .Setup(_ => _.StringSetAsync(key, json, It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true)
                .Verifiable();
            database
                .Setup(_ => _.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(json)
                .Verifiable();
            var cache = new RedisCache(database.Object);


            // act
            var set = await cache.SetAsync(key, value);
            var get = await cache.GetAsync<object>(key);

            // assert
            Assert.Multiple(() => {
                Assert.That(set, Is.True);
                Assert.That(get, Is.Not.Null);
                Assert.That(database.VerifyAll, Throws.Nothing);
            });
        }

        [Test]
        public async Task Can_Set_Value_To_Cache_With_ExpiresIn() {
            // arrange
            // arrange
            var key = "Test";
            var value = new {
                Id = 1,
                Name = "Test"
            };
            var json = JsonSerializer.Serialize(value);
            var opts = new CacheEntryOptions {
                ExpiresIn = TimeSpan.FromMilliseconds(500)
            };
            var database = new Mock<IDatabase>();
            database
                .Setup(_ => _.StringSetAsync(key, json, opts.ExpiresIn, It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true)
                .Verifiable();
            database
                .Setup(_ => _.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(string.Empty)
                .Verifiable();
            var cache = new RedisCache(database.Object);
            

            // act
            var set = await cache.SetAsync(key, value, opts);

            await Task.Delay(TimeSpan.FromMilliseconds(750));

            var get = await cache.GetAsync<object>(key);

            // assert
            Assert.Multiple(() => {
                Assert.That(set, Is.True);
                Assert.That(get, Is.Null);
                Assert.That(database.VerifyAll, Throws.Nothing);
            });
        }
    }
}