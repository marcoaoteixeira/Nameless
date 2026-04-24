using Moq;

namespace Nameless.Resilience;

public class RetryPipelineFactoryExtensionsTests {
    // --- Create(Action) ---

    [Fact]
    public void Create_WithOnRetryAction_CallsCreateWithConfiguration() {
        // arrange
        Action<Exception?, TimeSpan, int, int> onRetry = (_, _, _, _) => { };

        var pipelineMock = new Mock<IRetryPipeline>();
        var factoryMock = new Mock<IRetryPipelineFactory>();
        factoryMock.Setup(f => f.Create(It.IsAny<RetryPolicyConfiguration>()))
                   .Returns(pipelineMock.Object);

        // act
        var result = factoryMock.Object.CreateDefault(onRetry);

        // assert
        Assert.Same(pipelineMock.Object, result);
        factoryMock.Verify(
            f => f.Create(It.Is<RetryPolicyConfiguration>(c => c.OnRetry == onRetry)),
            Times.Once);
    }
}
