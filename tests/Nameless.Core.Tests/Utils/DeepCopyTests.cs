namespace Nameless.Utils;

public class DeepCopyTests {
    // ─── Clone(object) ───────────────────────────────────────────────────────

    [Fact]
    public void Clone_ConcreteObject_ReturnsDifferentReference() {
        // arrange
        var original = new SamplePoco { Name = "Alice", Age = 30 };

        // act
        var clone = DeepCopy.Clone(original);

        // assert
        Assert.NotSame(original, clone);
    }

    [Fact]
    public void Clone_ConcreteObject_HasSamePropertyValues() {
        // arrange
        var original = new SamplePoco { Name = "Alice", Age = 30 };

        // act
        var clone = (SamplePoco)DeepCopy.Clone(original);

        // assert
        Assert.Multiple(() => {
            Assert.Equal("Alice", clone.Name);
            Assert.Equal(30, clone.Age);
        });
    }

    [Fact]
    public void Clone_Type_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(() => DeepCopy.Clone(typeof(string)));
    }

    // ─── Clone<T>(T) ─────────────────────────────────────────────────────────

    [Fact]
    public void CloneGeneric_ConcreteObject_ReturnsDifferentReference() {
        // arrange
        var original = new SamplePoco { Name = "Bob", Age = 25 };

        // act
        var clone = DeepCopy.Clone(original);

        // assert
        Assert.NotSame(original, clone);
    }

    [Fact]
    public void CloneGeneric_ConcreteObject_HasSamePropertyValues() {
        // arrange
        var original = new SamplePoco { Name = "Bob", Age = 25 };

        // act
        var clone = DeepCopy.Clone(original);

        // assert
        Assert.Multiple(() => {
            Assert.Equal("Bob", clone.Name);
            Assert.Equal(25, clone.Age);
        });
    }

    // ─── Deep copy verification ───────────────────────────────────────────────

    [Fact]
    public void Clone_MutatingClone_DoesNotAffectOriginal() {
        // arrange
        var original = new SamplePoco { Name = "Carol", Age = 40, Inner = new InnerPoco { Value = "original" } };

        // act
        var clone = DeepCopy.Clone(original);
        clone.Inner!.Value = "mutated";

        // assert
        Assert.Equal("original", original.Inner!.Value);
    }

    // ─── test doubles ─────────────────────────────────────────────────────────

    public sealed class SamplePoco {
        public string? Name { get; set; }
        public int Age { get; set; }
        public InnerPoco? Inner { get; set; }
    }

    public sealed class InnerPoco {
        public string? Value { get; set; }
    }
}
