namespace Nameless.Collections.Generic;

public class PageTests {

    [Test]
    public void When_Creating_New_Instance_Then_Return_New_Object_With_Given_Queryable() {
        // arrange
        // 100 items
        var items = Enumerable.Range(0, Page<int>.DEFAULT_SIZE * 10)
                              .AsQueryable();

        // act
        var page = new Page<int>(items); // page 1, size 10

        // assert
        Assert.Multiple(() => {
            Assert.That(page.Number, Is.EqualTo(1));
            Assert.That(page.Size, Is.EqualTo(10));
            Assert.That(page.TotalItems, Is.EqualTo(100));
            Assert.That(page.TotalPages, Is.EqualTo(10));
            Assert.That(page.Items, Is.EquivalentTo([0, 1, 2, 3, 4, 5, 6, 7, 8, 9]));
            Assert.That(page.HasPrevious, Is.False);
            Assert.That(page.HasNext, Is.True);
        });
    }

    [Test]
    public void WhenQueryHas100Items_WhenPageSizeIs15_ThenTotalPagesIs7() {
        // arrange
        const int pageSize = 15;
        const int expectedTotalPages = 7;
        // 100 items
        var items = Enumerable.Range(0, Page<int>.DEFAULT_SIZE * 10)
                              .AsQueryable();
        var expectedItems = items.Take(pageSize)
                                 .ToArray();

        // act
        var page = new Page<int>(items, size: pageSize);

        // assert
        Assert.Multiple(() => {
            Assert.That(page.Number, Is.EqualTo(1));
            Assert.That(page.Size, Is.EqualTo(pageSize));
            Assert.That(page.TotalItems, Is.EqualTo(100));
            Assert.That(page.TotalPages, Is.EqualTo(expectedTotalPages));
            Assert.That(page.Items, Is.EquivalentTo(expectedItems));
            Assert.That(page.HasPrevious, Is.False);
            Assert.That(page.HasNext, Is.True);
        });
    }

    [Test]
    public void WhenQueryHas100Items_WhenPageSizeIs100_ThenHasPreviousAndHasNextIsFalse() {
        // arrange
        const int pageSize = 100;
        const int expectedTotalPages = 1;
        // 100 items
        var items = Enumerable.Range(0, Page<int>.DEFAULT_SIZE * 10)
                              .AsQueryable();
        var expectedItems = items.Take(pageSize)
                                 .ToArray();

        // act
        var page = new Page<int>(items, size: pageSize);

        // assert
        Assert.Multiple(() => {
            Assert.That(page.Number, Is.EqualTo(1));
            Assert.That(page.Size, Is.EqualTo(pageSize));
            Assert.That(page.TotalItems, Is.EqualTo(100));
            Assert.That(page.TotalPages, Is.EqualTo(expectedTotalPages));
            Assert.That(page.Items, Is.EquivalentTo(expectedItems));
            Assert.That(page.HasPrevious, Is.False);
            Assert.That(page.HasNext, Is.False);
        });
    }
}