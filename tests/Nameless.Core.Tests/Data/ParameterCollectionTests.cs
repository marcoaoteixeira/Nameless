using System.Data;
using Nameless.Data;

namespace Nameless;

public class ParameterCollectionTests {
    // --- Default constructor ---

    [Fact]
    public void DefaultConstructor_CreatesEmptyCollection() {
        // act
        var collection = new ParameterCollection();

        // assert
        Assert.Empty(collection);
    }

    // --- Constructor with parameters ---

    [Fact]
    public void Constructor_WithParameters_PopulatesCollection() {
        // arrange
        var parameters = new[] {
            new Parameter("p1", 1, DbType.Int32),
            new Parameter("p2", "hello", DbType.String)
        };

        // act
        var collection = new ParameterCollection(parameters);

        // assert
        Assert.Equal(2, collection.Count());
    }

    // --- Add ---

    [Fact]
    public void Add_WithNewParameter_AddsToCollection() {
        // arrange
        var collection = new ParameterCollection();
        var param = new Parameter("name", "value", DbType.String);

        // act
        collection.Add(param);

        // assert
        Assert.Single(collection);
    }

    [Fact]
    public void Add_WithDuplicateName_ReplacesExisting() {
        // arrange
        var collection = new ParameterCollection();
        collection.Add(new Parameter("p1", "first", DbType.String));

        // act
        collection.Add(new Parameter("p1", "second", DbType.String));

        // assert
        Assert.Multiple(() => {
            Assert.Single(collection);
            Assert.Equal("second", collection.First().Value);
        });
    }

    [Fact]
    public void Add_IsCaseInsensitive_ForParameterNames() {
        // arrange
        var collection = new ParameterCollection();
        collection.Add(new Parameter("MyParam", "original", DbType.String));

        // act
        collection.Add(new Parameter("myparam", "replaced", DbType.String));

        // assert
        Assert.Single(collection);
    }

    // --- GetEnumerator ---

    [Fact]
    public void GetEnumerator_IteratesAllParameters() {
        // arrange
        var collection = new ParameterCollection(new[] {
            new Parameter("a", 1, DbType.Int32),
            new Parameter("b", 2, DbType.Int32)
        });

        // act
        var names = collection.Select(p => p.Name).ToList();

        // assert
        Assert.Contains("a", names);
        Assert.Contains("b", names);
    }

    [Fact]
    public void NonGenericGetEnumerator_IteratesParameters() {
        var collection = new ParameterCollection(new[] {
            new Parameter("p1", 1, DbType.Int32)
        });

        // cast to IEnumerable to exercise the explicit non-generic interface method
        var enumerable = (System.Collections.IEnumerable)collection;
        var count = 0;
        foreach (var _ in enumerable) { count++; }

        Assert.Equal(1, count);
    }
}
