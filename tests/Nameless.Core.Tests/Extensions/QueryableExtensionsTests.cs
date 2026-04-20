namespace Nameless;

public class QueryableExtensionsTests {
    // ─── OrderBy ────────────────────────────────────────────────────────────

    [Fact]
    public void OrderBy_WithValidProperty_SortsAscending() {
        // arrange
        var items = new[] {
            new Item { Name = "Zebra", Age = 3 },
            new Item { Name = "Apple", Age = 1 },
            new Item { Name = "Mango", Age = 2 }
        }.AsQueryable();

        // act
        var sorted = items.OrderBy(nameof(Item.Name)).ToList();

        // assert
        Assert.Equal("Apple", sorted[0].Name);
        Assert.Equal("Mango", sorted[1].Name);
        Assert.Equal("Zebra", sorted[2].Name);
    }

    [Fact]
    public void OrderBy_WithNonExistentProperty_ThrowsMissingMemberException() {
        // arrange
        var items = new[] { new Item { Name = "Test", Age = 1 } }.AsQueryable();

        // act & assert
        Assert.Throws<MissingMemberException>(() => items.OrderBy("NonExistentProperty").ToList());
    }

    // ─── OrderByDescending ──────────────────────────────────────────────────

    [Fact]
    public void OrderByDescending_WithValidProperty_SortsDescending() {
        // arrange
        var items = new[] {
            new Item { Name = "Apple", Age = 1 },
            new Item { Name = "Zebra", Age = 3 },
            new Item { Name = "Mango", Age = 2 }
        }.AsQueryable();

        // act
        var sorted = items.OrderByDescending(nameof(Item.Age)).ToList();

        // assert
        Assert.Equal(3, sorted[0].Age);
        Assert.Equal(2, sorted[1].Age);
        Assert.Equal(1, sorted[2].Age);
    }

    [Fact]
    public void OrderByDescending_WithNonExistentProperty_ThrowsMissingMemberException() {
        // arrange
        var items = new[] { new Item { Name = "Test", Age = 1 } }.AsQueryable();

        // act & assert
        Assert.Throws<MissingMemberException>(() => items.OrderByDescending("NonExistent").ToList());
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    private sealed class Item {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
