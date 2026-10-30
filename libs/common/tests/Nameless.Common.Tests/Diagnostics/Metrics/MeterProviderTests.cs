namespace Nameless.Diagnostics.Metrics;

[UnitTest]
public class MeterProviderTests {
    // The provider uses a static cache keyed on assembly name+version.
    // Each test disposes what it creates so cache state does not bleed
    // into other tests.

    [Fact]
    public void Create_WithNullAssembly_ReturnsNonNullMeter() {
        // arrange & act
        var meter = MeterProvider.Create(assembly: null);

        // assert
        Assert.NotNull(meter);

        // cleanup
        meter.Dispose();
    }

    [Fact]
    public void Create_WithAssembly_ReturnsNonNullMeter() {
        // arrange
        var assembly = typeof(MeterProviderTests).Assembly;

        // act
        var meter = MeterProvider.Create(assembly);

        // assert
        Assert.NotNull(meter);

        // cleanup
        meter.Dispose();
    }

    [Fact]
    public void Create_CalledTwiceWithSameAssembly_ReturnsSameInstance() {
        // arrange
        var assembly = typeof(MeterProvider).Assembly;

        // act
        var first = MeterProvider.Create(assembly);
        var second = MeterProvider.Create(assembly);

        // assert
        Assert.Same(first, second);

        // cleanup
        first.Dispose();
    }

    [Fact]
    public void Create_CalledWithDifferentAssemblies_ReturnsDifferentInstances() {
        // arrange
        var assemblyA = typeof(MeterProvider).Assembly;
        var assemblyB = typeof(MeterProviderTests).Assembly;

        // act
        var meterA = MeterProvider.Create(assemblyA);
        var meterB = MeterProvider.Create(assemblyB);

        // assert
        Assert.NotSame(meterA, meterB);

        // cleanup
        meterA.Dispose();
        meterB.Dispose();
    }

    [Fact]
    public void Create_AfterDispose_ReturnsNewInstance() {
        // arrange
        var assembly = typeof(MeterProviderTests).Assembly;
        var first = MeterProvider.Create(assembly);
        first.Dispose();

        // act
        var second = MeterProvider.Create(assembly);

        // assert
        Assert.NotSame(first, second);

        // cleanup
        second.Dispose();
    }
}
