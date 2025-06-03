namespace Nameless.Collections.Generic;

public class PageTests {
    [Fact]
    public void When_Creating_New_Instance_Then_Return_New_Object_With_Given_Queryable() {
        // arrange
        // 100 items
        var items = Enumerable.Range(0, Page<int>.DEFAULT_SIZE * 10).AsQueryable();

        // act
        var page = new Page<int>(items); // page 1, size 10

        // assert
        Assert.Multiple(() => {
            Assert.Equal(expected: 1, page.Number);
            Assert.Equal(expected: 10, page.Size);
            Assert.Equal(expected: 100, page.TotalItems);
            Assert.Equal(expected: 10, page.TotalPages);
            Assert.Equivalent(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, page.Items);
            Assert.False(page.HasPrevious);
            Assert.True(page.HasNext);
        });
    }

    [Fact]
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
            Assert.Equal(expected: 1, page.Number);
            Assert.Equal(expected: pageSize, page.Size);
            Assert.Equal(expected: 100, page.TotalItems);
            Assert.Equal(expected: expectedTotalPages, page.TotalPages);
            Assert.Equivalent(expectedItems, page.Items);
            Assert.False(page.HasPrevious);
            Assert.True(page.HasNext);
        });
    }

    [Fact]
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
            Assert.Equal(expected: 1, page.Number);
            Assert.Equal(expected: pageSize, page.Size);
            Assert.Equal(expected: 100, page.TotalItems);
            Assert.Equal(expected: expectedTotalPages, page.TotalPages);
            Assert.Equivalent(expectedItems, page.Items);
            Assert.False(page.HasPrevious);
            Assert.False(page.HasNext);
        });
    }
}