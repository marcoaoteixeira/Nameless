namespace Nameless.Collections.Generic;

public class PageTests {

    [Test]
    public void When_Creating_New_Instance_Then_Return_New_Object_With_Given_Queryable() {
        // arrange
        var items = Enumerable.Range(0, 10).ToArray();

        // act
        var page = new Page<int>(items, 1, items.Length);

        // assert
        Assert.Multiple(() => {
            Assert.That(page.Number, Is.EqualTo(1));
            Assert.That(page.Size, Is.EqualTo(10));
            Assert.That(page, Is.EquivalentTo(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));
        });
    }
}