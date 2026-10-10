using System.Reflection;
using System.Reflection.Emit;

namespace Nameless.Extensions;

public interface IAssemblyExtTestMarker { }
public sealed class ConcreteAssemblyExtTestImpl : IAssemblyExtTestMarker { }

public static class AssemblyExtVisibleContainer {
    public sealed class NestedImpl : IAssemblyExtTestMarker { }
}

internal static class AssemblyExtHiddenContainer {
    public sealed class NestedImpl : IAssemblyExtTestMarker { }
}

[UnitTest]
public class AssemblyExtensionsTests {
    // ─── GetDirectoryPath ────────────────────────────────────────────────────

    [Fact]
    public void GetDirectoryPath_ForKnownAssembly_ReturnsNonEmptyPath() {
        // arrange
        var assembly = typeof(string).Assembly;

        // act
        var path = assembly.GetDirectoryPath();

        // assert
        Assert.NotEmpty(path);
    }

    // ─── GetSemanticName ─────────────────────────────────────────────────────

    [Fact]
    public void GetSemanticName_ReturnsAssemblyName() {
        // arrange
        var assembly = Assembly.GetExecutingAssembly();

        // act
        var name = assembly.GetSemanticName();

        // assert
        Assert.Multiple(
            () => Assert.NotEmpty(name),
            () => Assert.Equal(assembly.GetName().Name, name)
        );
    }

    // ─── GetSemanticVersion ──────────────────────────────────────────────────

    [Fact]
    public void GetSemanticVersion_ReturnsNonEmptyVersionString() {
        // arrange
        var assembly = Assembly.GetExecutingAssembly();

        // act
        var version = assembly.GetSemanticVersion();

        // assert
        Assert.NotEmpty(version);
    }

    // ─── ExecuteAssemblyScan(Type) ────────────────────────────────────────────

    [Fact]
    public void GetImplementations_FindsConcreteImplementationsOfInterface() {
        // arrange
        var assembly = typeof(AssemblyExtensionsTests).Assembly;

        // act
        var results = assembly.GetImplementations(typeof(IAssemblyExtTestMarker)).ToList();

        // assert
        Assert.Contains(typeof(ConcreteAssemblyExtTestImpl), results);
    }

    [Fact]
    public void GetImplementations_DoesNotReturnAbstractTypes() {
        // arrange
        var assembly = typeof(AssemblyExtensionsTests).Assembly;

        // act
        var results = assembly.GetImplementations(typeof(IAssemblyExtTestMarker)).ToList();

        // assert
        Assert.DoesNotContain(typeof(AbstractTestImpl), results);
    }

    [Fact]
    public void GetImplementations_FindsNestedPublicImplementations() {
        // arrange
        var assembly = typeof(AssemblyExtensionsTests).Assembly;

        // act
        var results = assembly.GetImplementations(typeof(IAssemblyExtTestMarker)).ToList();

        // assert
        Assert.Contains(typeof(AssemblyExtVisibleContainer.NestedImpl), results);
    }

    [Fact]
    public void GetImplementations_DoesNotReturnTypesNestedInNonPublicTypes() {
        // arrange
        var assembly = typeof(AssemblyExtensionsTests).Assembly;

        // act
        var results = assembly.GetImplementations(typeof(IAssemblyExtTestMarker)).ToList();

        // assert
        Assert.DoesNotContain(typeof(AssemblyExtHiddenContainer.NestedImpl), results);
    }

    [Fact]
    public void GetImplementations_OnDynamicAssembly_ReturnsEmpty() {
        // arrange
        var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Nameless.Dynamic.Tests"), AssemblyBuilderAccess.Run);

        // act
        var results = assembly.GetImplementations(typeof(IAssemblyExtTestMarker)).ToList();

        // assert
        Assert.Empty(results);
    }

    // ─── IEnumerable<Assembly>.ExecuteAssemblyScan ────────────────────────────

    [Fact]
    public void GetImplementations_OnAssemblyCollection_AggregatesResults() {
        // arrange
        var assemblies = new[] { typeof(AssemblyExtensionsTests).Assembly };

        // act
        var results = assemblies.GetImplementations([typeof(IAssemblyExtTestMarker)]).ToList();

        // assert
        Assert.NotEmpty(results);
    }

    // ─── test doubles ────────────────────────────────────────────────────────

    private abstract class AbstractTestImpl : IAssemblyExtTestMarker { }
}
