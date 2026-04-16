using Lucene.Net.Documents;
using Nameless.Lucene.Repository.Fixtures;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Lucene.Repository;

namespace Nameless.Lucene.Repository.Infrastructure;

[UnitTest]
public class AsyncEnumeratorWrapperTests {
    [Fact]
    public async Task WhenEnumerating_WithValidEnumerable_ThenShouldBePossibleIterate() {
        // arrange
        var car = CarFaker.Instance.Generate();
        var mapper = new MapperMocker().WithMap(car).Build();
        await using var sut = new AsyncEnumeratorWrapper<Car>(
            [
                [new StringField("field", "value", Field.Store.YES)]
            ],
            mapper,
            TestContext.Current.CancellationToken
        );

        // act
        var moveNext = await sut.MoveNextAsync();
        var current = sut.Current;
        var moveNextAgain = await sut.MoveNextAsync();

        // assert
        Assert.Multiple(() => {
            Assert.True(moveNext);
            Assert.Equal(car, current);
            Assert.False(moveNextAgain);
        });
    }
}
