namespace Nameless.Collections.Generic;
public class PaginableTests {
    [Test]
    public void When_Enumerating_Paginable_Then_Return_List_Of_Pages() {
        // arrange
        const int total = 100;
        const int pageSize = 10;
        var items = Enumerable.Range(1, total)
                              .AsQueryable();
        var count = items.Count();

        // act
        var paginable = new Paginable<int>(items, pageSize: pageSize);

        var totalPages = count / pageSize;
        if (count % pageSize != 0) {
            ++totalPages;
        }

        // assert
        Assert.Multiple(() => {
            Assert.That(paginable.Total, Is.EqualTo(total));
            Assert.That(paginable.Count(), Is.EqualTo(totalPages));
        });
    }

    [Test]
    public void When_Retrieving_Specific_Page_Then_Return_Correct_Items() {
        // arrange
        const int total = 100;
        const int pageSize = 10;
        var items = Enumerable.Range(1, total)
                              .AsQueryable();

        // act
        var paginable = new Paginable<int>(items, pageSize: pageSize);
        var page = paginable.ElementAt(3);

        // assert
        Assert.Multiple(() => {
            Assert.That(page.Count, Is.EqualTo(pageSize));
            Assert.That(page, Is.EqualTo(Enumerable.Range(31, 10)));
        });
    }

    [Test]
    public void When_Page_Has_Fewer_Items_Than_PageSize_Then_Return_Correct_Items() {
        // arrange
        const int total = 15;
        const int pageSize = 10;
        var items = Enumerable.Range(1, total)
                              .AsQueryable();

        // act
        var paginable = new Paginable<int>(items, pageSize: pageSize);
        var page = paginable.Last();

        // assert
        Assert.Multiple(() => {
            Assert.That(page.Count, Is.EqualTo(pageSize / 2));
            Assert.That(page, Is.EqualTo(Enumerable.Range(11, 5)));
        });
    }
}
