using System.Reflection;
using System.Reflection.Emit;

namespace Nameless.Helpers;

public sealed class DiscoveryClassHolder<T> where T : class;

public sealed class DiscoveryStructHolder<T> where T : struct;

public sealed class DiscoveryNewHolder<T> where T : new();

public sealed class DiscoveryNakedHolder<TKey, TValue>
    where TKey : ConstraintBaseClass
    where TValue : TKey;

[UnitTest]
public class GenericTypeHelperDiscoveryTests {
    private static Assembly CreateDynamicAssembly() {
        return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"dyn-{Guid.NewGuid():N}"), AssemblyBuilderAccess.Run);
    }

    [Theory]
    [InlineData(typeof(DiscoveryClassHolder<>), "class")]
    [InlineData(typeof(DiscoveryStructHolder<>), "struct")]
    [InlineData(typeof(DiscoveryNewHolder<>), "new()")]
    public void GetArgumentsThatCloses_WhenAssembliesExposeNoTypes_DescribesConstraintsInError(Type definition, string expected) {
        // act
        var exception = Assert.Throws<InvalidOperationException>(
            () => GenericTypeHelper.GetArgumentsThatCloses(definition, CreateDynamicAssembly()).ToList());

        // assert
        Assert.Contains(expected, exception.Message);
    }

    [Fact]
    public void GetArgumentsThatCloses_WithoutAssemblies_IncludesConstraintAssemblies() {
        // act
        var combinations = GenericTypeHelper.GetArgumentsThatCloses(typeof(ConstraintBaseHolder<>)).ToList();

        // assert
        Assert.Contains(combinations, c => c[0] == typeof(ConstraintDerived));
    }

    [Fact]
    public void GetArgumentsThatCloses_WithParameterConstrainedToAnotherParameter_ReturnsAssignableCombinations() {
        // act
        var combinations = GenericTypeHelper
            .GetArgumentsThatCloses(typeof(DiscoveryNakedHolder<,>), typeof(GenericTypeHelperDiscoveryTests).Assembly)
            .ToList();

        // assert
        Assert.Multiple(
            () => Assert.Contains(combinations, c => c[0] == typeof(ConstraintBaseClass) && c[1] == typeof(ConstraintDerived)),
            () => Assert.All(combinations, c => Assert.True(c[0].IsAssignableFrom(c[1])))
        );
    }
}
