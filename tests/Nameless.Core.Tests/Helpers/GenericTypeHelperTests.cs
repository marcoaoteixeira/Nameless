namespace Nameless.Helpers;

public class GenericTypeHelperTests {
    // --- GetArgumentsThatCloses ---

    [Fact]
    public void GetArgumentsThatCloses_WithNonGenericType_ThrowsArgumentException() {
        // act & assert
        Assert.Throws<ArgumentException>(
            () => GenericTypeHelper.GetArgumentsThatCloses(typeof(string)).ToList()
        );
    }

    [Fact]
    public void GetArgumentsThatCloses_WithOpenGenericWithClassConstraint_FindsConcreteTypes() {
        // act
        var combinations = GenericTypeHelper
            .GetArgumentsThatCloses(typeof(GenericTestRepository<>), typeof(GenericTypeHelperTests).Assembly)
            .ToList();

        // assert: should find at least one combination (OrderGenericEntity, ProductGenericEntity)
        Assert.NotEmpty(combinations);
    }

    [Fact]
    public void GetArgumentsThatCloses_WithUnconstrained_ThrowsInvalidOperationException() {
        // act & assert
        Assert.Throws<InvalidOperationException>(
            () => GenericTypeHelper.GetArgumentsThatCloses(typeof(UnconstrainedGeneric<>)).ToList()
        );
    }

    [Fact]
    public void GetArgumentsThatCloses_ResultsAreAllSingleElementArrays() {
        // act
        var combinations = GenericTypeHelper
            .GetArgumentsThatCloses(typeof(GenericTestRepository<>), typeof(GenericTypeHelperTests).Assembly)
            .ToList();

        // assert
        Assert.All(combinations, combo => Assert.Single(combo));
    }

    [Fact]
    public void GetArgumentsThatCloses_ResultContainsExpectedEntityTypes() {
        // act
        var types = GenericTypeHelper
            .GetArgumentsThatCloses(typeof(GenericTestRepository<>), typeof(GenericTypeHelperTests).Assembly)
            .Select(combo => combo[0])
            .ToList();

        // assert
        Assert.Multiple(() => {
            Assert.Contains(typeof(OrderGenericEntity), types);
            Assert.Contains(typeof(ProductGenericEntity), types);
        });
    }
}

// --- test doubles (must be public top-level types for GetExportedTypes() to find them) ---

public interface IGenericTestEntity { }
public sealed class OrderGenericEntity : IGenericTestEntity { }
public sealed class ProductGenericEntity : IGenericTestEntity { }
public sealed class GenericTestRepository<T> where T : class, IGenericTestEntity { }
public sealed class UnconstrainedGeneric<T> { }
