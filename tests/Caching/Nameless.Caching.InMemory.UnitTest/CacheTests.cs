namespace Nameless.Caching.InMemory;

public class CacheTests {
    private static InMemoryCache CreateSut() => new();

    [Test]
    public async Task Set_New_Object_To_Cache() {
        // arrange
        var value = new {
            Id = 1,
            Name = "Test"
        };

        var cacheEntryOptions = new CacheEntryOptions();
        var sut = CreateSut();

        // act
        var result = await sut.SetAsync("Key", value, cacheEntryOptions, cancellationToken: default);

        // assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Set_And_Get_Object_From_Cache() {
        // arrange
        const string key = "Key";
        object value = new {
            Id = 1,
            Name = "Test"
        };

        var cacheEntryOptions = new CacheEntryOptions();
        var sut = CreateSut();

        // act
        _ = await sut.SetAsync(key, value, cacheEntryOptions, cancellationToken: default);
        var result = await sut.GetAsync<object>(key, cancellationToken: default);

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
        var result = await sut.RemoveAsync("Key", CancellationToken.None);

        // assert
        Assert.That(result, Is.True);
    }
}