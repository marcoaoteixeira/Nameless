using System.Diagnostics.Metrics;

namespace Nameless.Diagnostics.Metrics;

[UnitTest]
public class MeterWrapperTests {
    private static MeterWrapper CreateSut(string meterName) {
        return new MeterWrapper(new Meter(meterName));
    }

    [Fact]
    public void Name_ReturnsInnerMeterName() {
        // arrange
        using var sut = CreateSut("meter.name.test");

        // act & assert
        Assert.Equal("meter.name.test", sut.Name);
    }

    [Fact]
    public void CreateHistogram_ReturnsHistogram() {
        // arrange
        using var sut = CreateSut("meter.histogram.test");

        // act
        var histogram = sut.CreateHistogram<int>("my.histogram");

        // assert
        Assert.NotNull(histogram);
    }

    [Fact]
    public void CreateHistogram_CalledTwiceWithSameName_ReturnsSameInstance() {
        // arrange
        using var sut = CreateSut("meter.histogram.cache.test");

        // act
        var first = sut.CreateHistogram<int>("cached.histogram");
        var second = sut.CreateHistogram<int>("cached.histogram");

        // assert
        Assert.Same(first, second);
    }

    [Fact]
    public void CreateHistogram_WithNullName_Throws() {
        // arrange
        using var sut = CreateSut("meter.histogram.null.test");

        // act & assert
        Assert.Throws<ArgumentNullException>(() => sut.CreateHistogram<int>(name: null!));
    }

    [Fact]
    public void CreateGauge_ReturnsGauge() {
        // arrange
        using var sut = CreateSut("meter.gauge.test");

        // act
        var gauge = sut.CreateGauge<int>("my.gauge");

        // assert
        Assert.NotNull(gauge);
    }

    [Fact]
    public void CreateCounter_ReturnsCounter() {
        // arrange
        using var sut = CreateSut("meter.counter.test");

        // act
        var counter = sut.CreateCounter<int>("my.counter");

        // assert
        Assert.NotNull(counter);
    }

    [Fact]
    public void CreateUpDownCounter_ReturnsUpDownCounter() {
        // arrange
        using var sut = CreateSut("meter.updowncounter.test");

        // act
        var counter = sut.CreateUpDownCounter<int>("my.updowncounter");

        // assert
        Assert.NotNull(counter);
    }

    [Fact]
    public void Dispose_FiresOnDisposeEvent() {
        // arrange
        var sut = CreateSut("meter.dispose.test");

        IMeter? captured = null;
        sut.OnDispose += meter => captured = meter;

        // act
        sut.Dispose();

        // assert
        Assert.Multiple(
            () => Assert.NotNull(captured),
            () => Assert.Same(sut, captured)
        );
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow() {
        // arrange
        var sut = CreateSut("meter.double-dispose.test");
        sut.Dispose();

        // act
        var exception = Record.Exception(sut.Dispose);

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void AfterDispose_CreateHistogram_Throws() {
        // arrange
        var sut = CreateSut("meter.disposed-access.test");
        sut.Dispose();

        // act & assert
        Assert.Throws<ObjectDisposedException>(() => sut.CreateHistogram<int>("x"));
    }
}
