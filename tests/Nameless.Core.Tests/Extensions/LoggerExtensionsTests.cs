using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless;

public class LoggerExtensionsTests {
    // --- OnCondition ---

    [Fact]
    public void OnCondition_WhenConditionTrue_ReturnsSameLogger() {
        // arrange
        ILogger<LoggerExtensionsTests> logger = NullLogger<LoggerExtensionsTests>.Instance;

        // act
        var result = logger.OnCondition(condition: true);

        // assert
        Assert.Same(logger, result);
    }

    [Fact]
    public void OnCondition_WhenConditionFalse_ReturnsNullLogger() {
        // arrange
        ILogger<LoggerExtensionsTests> logger = NullLogger<LoggerExtensionsTests>.Instance;

        // act
        var result = logger.OnCondition(condition: false);

        // assert
        Assert.Same(NullLogger<LoggerExtensionsTests>.Instance, result);
    }
}
