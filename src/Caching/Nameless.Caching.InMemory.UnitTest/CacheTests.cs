namespace Nameless.Caching.InMemory {
    public class CacheTests {
        private static InMemoryCache CreateSut()
            => new InMemoryCache();

        [Test]
        public async Task Set_New_Object_To_Cache() {
            // arrange
            var value = new {
                Id = 1,
                Name = "Test"
            };

            var sut = CreateSut();

            // act
            var result = await sut.SetAsync("Key", value);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Set_And_Get_Object_From_Cache() {
            // arrange
            const string key = "Key";
            object? value = new {
                Id = 1,
                Name = "Test"
            };

            var sut = CreateSut();

            // act
            var objA = await sut.SetAsync(key, value);
            var result = await sut.GetAsync<object?>(key);

            // assert
            Assert.Multiple(() => {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.SameAs(value));
                Assert.That(result, Has.Property("Id").EqualTo(1));
                Assert.That(result, Has.Property("Name").EqualTo("Test"));
            });
        }

        [Test]
        public async Task Remove_Object_From_Cache() {
            // arrange
            var sut = CreateSut();

            // act
            var result = await sut.RemoveAsync("Key");

            // assert
            Assert.That(result, Is.True);
        }
    }
}
