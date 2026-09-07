using Nameless.Diagnostics.ActivitySource;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Diagnostics;

[UnitTest]
public class ActivitySourceProviderTests {
    // The provider uses a static cache keyed on assembly name+version.
    // Each test receives a freshly disposed source (or a distinct assembly)
    // so that cache state from one test does not bleed into the next.

    [Fact]
    public void Create_WithNullAssembly_ReturnsNonNullActivitySource() {
        // arrange & act
        // Passing null falls back to Assembly.GetCallingAssembly() inside the provider.
        var activitySource = ActivitySourceProvider.Create(assembly: null);

        // assert
        Assert.NotNull(activitySource);

        // cleanup — dispose to remove from cache so other tests see a fresh state
        activitySource.Dispose();
    }

    [Fact]
    public void Create_WithAssembly_ReturnsNonNullActivitySource() {
        // arrange
        var assembly = typeof(ActivitySourceProviderTests).Assembly;

        // act
        var activitySource = ActivitySourceProvider.Create(assembly);

        // assert
        Assert.NotNull(activitySource);

        // cleanup
        activitySource.Dispose();
    }

    [Fact]
    public void Create_CalledTwiceWithSameAssembly_ReturnsSameInstance() {
        // arrange
        var assembly = typeof(ActivitySourceProvider).Assembly;

        // act
        var first = ActivitySourceProvider.Create(assembly);
        var second = ActivitySourceProvider.Create(assembly);

        // assert
        Assert.Same(first, second);

        // cleanup
        first.Dispose();
    }

    [Fact]
    public void Create_CalledWithDifferentAssemblies_ReturnsDifferentInstances() {
        // arrange — use two assemblies with different names/versions
        var assemblyA = typeof(ActivitySourceProvider).Assembly;   // Nameless.Core.Impl
        var assemblyB = typeof(ActivitySourceProviderTests).Assembly; // Nameless.Core.Impl.Tests

        // act
        var sourceA = ActivitySourceProvider.Create(assemblyA);
        var sourceB = ActivitySourceProvider.Create(assemblyB);

        // assert
        Assert.NotSame(sourceA, sourceB);

        // cleanup
        sourceA.Dispose();
        sourceB.Dispose();
    }
}
