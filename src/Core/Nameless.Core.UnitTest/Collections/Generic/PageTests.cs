using FluentAssertions;

namespace Nameless.Collections.Generic {

    public class PageTests {

        [Test]
        public void Page_Returns_Valid_Object_From_Valid_Data() {
            // arrage
            var collection = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }; // 10 items

            // act
            var page = new Page<int>(collection);

            // assert
            page.Index.Should().Be(0);
            page.Number.Should().Be(1);
            page.Size.Should().Be(10);
            page.Length.Should().Be(10);
            page.PageCount.Should().Be(1);
            page.Total.Should().Be(10);
            page.HasNext.Should().BeFalse();
            page.HasPrevious.Should().BeFalse();
        }

        [Test]
        public void Page_Returns_PageCount_Equals_2_For_Collection_With_11_Items() {
            // arrage
            var collection = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // 11 items

            // act
            var page = new Page<int>(collection);

            // assert
            page.Index.Should().Be(0);
            page.Number.Should().Be(1);
            page.Size.Should().Be(10);
            page.Length.Should().Be(10);
            page.PageCount.Should().Be(2);
            page.Total.Should().Be(collection.Length);
            page.HasNext.Should().BeTrue();
            page.HasPrevious.Should().BeFalse();
        }

        [Test]
        public void Page_Collection_With_11_Items_Index_Equals_1_Should_HasPrevious() {
            // arrage
            var collection = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // 11 items

            // act
            var page = new Page<int>(collection, index: 1);

            // assert
            page.Index.Should().Be(1);
            page.Number.Should().Be(2);
            page.Size.Should().Be(10);
            page.Length.Should().Be(1);
            page.PageCount.Should().Be(2);
            page.Total.Should().Be(collection.Length);
            page.HasNext.Should().BeFalse();
            page.HasPrevious.Should().BeTrue();
        }

        [Test]
        public void Page_Collection_With_5_Items() {
            // arrage
            var collection = new[] { 0, 1, 2, 3, 4 }; // 5 items

            // act
            var page = new Page<int>(collection, index: 0, size: 10);

            // assert
            page.Index.Should().Be(0);
            page.Number.Should().Be(1);
            page.Size.Should().Be(10);
            page.Length.Should().Be(5);
            page.PageCount.Should().Be(1);
            page.Total.Should().Be(collection.Length);
            page.HasNext.Should().BeFalse();
            page.HasPrevious.Should().BeFalse();
        }
    }
}
