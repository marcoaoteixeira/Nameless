using Moq;
using Nameless.ObjectModel;

namespace Nameless.Validation;

public class ValidationExtensionsTests {
    // --- ValidateAsync(object, CancellationToken) ---

    [Fact]
    public async Task ValidateAsync_WithoutContext_DelegatesToFullSignature() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var mock = new Mock<IValidationService>();
        mock.Setup(s => s.ValidateAsync(
                It.IsAny<object>(),
                It.IsAny<Dictionary<string, object>>(),
                ct))
            .ReturnsAsync(ValidationResult.Successful);

        // act
        var result = await mock.Object.ValidateAsync("value", ct);

        // assert
        Assert.True(result.Success);
        mock.Verify(
            s => s.ValidateAsync(
                "value",
                It.IsAny<Dictionary<string, object>>(),
                ct),
            Times.Once);
    }

    [Fact]
    public async Task ValidateAsync_WhenValidationFails_ReturnsFailedResult() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        ValidationResult failed = Error.Validation("invalid");
        var mock = new Mock<IValidationService>();
        mock.Setup(s => s.ValidateAsync(
                It.IsAny<object>(),
                It.IsAny<Dictionary<string, object>>(),
                ct))
            .ReturnsAsync(failed);

        // act
        var result = await mock.Object.ValidateAsync("value", ct);

        // assert
        Assert.False(result.Success);
    }
}
