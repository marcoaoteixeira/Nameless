using System.Reflection;

namespace Nameless.Extensions;

public interface IAssemblyExtTestMarker { }
public sealed class ConcreteAssemblyExtTestImpl : IAssemblyExtTestMarker { }

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
        Assert.NotEmpty(name);
        Assert.Equal(assembly.GetName().Name, name);
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

    // ─── GetImplementations(Type) ────────────────────────────────────────────

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

    // ─── IEnumerable<Assembly>.GetImplementations ────────────────────────────

    [Fact]
    public void GetImplementations_OnAssemblyCollection_AggregatesResults() {
        // arrange
        var assemblies = new[] { typeof(AssemblyExtensionsTests).Assembly };

        // act
        var results = assemblies.GetImplementations(typeof(IAssemblyExtTestMarker)).ToList();

        // assert
        Assert.NotEmpty(results);
    }

    // ─── test doubles ────────────────────────────────────────────────────────

    private abstract class AbstractTestImpl : IAssemblyExtTestMarker { }
}
