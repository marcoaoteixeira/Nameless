namespace Nameless.Helpers;

public class NetworkPortFinderTests {
    // --- ListAvailablePorts ---

    [Fact]
    public void ListAvailablePorts_WithValidRange_ReturnsPorts() {
        // act
        var ports = NetworkPortFinder.ListAvailablePorts(start: 20000, limit: 10).ToList();

        // assert: at least some ports should be available
        Assert.NotEmpty(ports);
    }

    [Fact]
    public void ListAvailablePorts_AllPortsAreInRange() {
        // arrange
        const int start = 20100;
        const int limit = 20;

        // act
        var ports = NetworkPortFinder.ListAvailablePorts(start, limit).ToList();

        // assert
        Assert.All(ports, p => {
            Assert.True(p >= start);
            Assert.True(p < start + limit);
        });
    }

    [Fact]
    public void ListAvailablePorts_WithInvalidRange_ThrowsArgumentOutOfRangeException() {
        // act & assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => NetworkPortFinder.ListAvailablePorts(start: -1).ToList()
        );
    }

    // --- GetFirstAvailablePort ---

    [Fact]
    public void GetFirstAvailablePort_WithValidRange_ReturnsPort() {
        // act
        var port = NetworkPortFinder.GetFirstAvailablePort(start: 20200, limit: 50);

        // assert
        Assert.True(port >= 20200);
    }
}
