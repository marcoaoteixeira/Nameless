using System.Diagnostics;
using Moq;

namespace Nameless.Diagnostics;

public class ActivityExtensionsTests {
    // --- ActivityExtensions.SetStatus(code) ---

    [Fact]
    public void SetStatus_WithCodeOnly_DelegatesToFullSignatureWithNullDescription() {
        // arrange
        var mock = new Mock<IActivity>();
        mock.Setup(a => a.SetStatus(ActivityStatusCode.Ok, null))
            .Returns(mock.Object);

        // act
        mock.Object.SetStatus(ActivityStatusCode.Ok);

        // assert
        mock.Verify(a => a.SetStatus(ActivityStatusCode.Ok, null), Times.Once);
    }

    // --- ActivitySourceExtensions.StartActivity(name) ---

    [Fact]
    public void StartActivity_WithNameOnly_UsesInternalKindAndNullParent() {
        // arrange
        var activityMock = new Mock<IActivity>();
        var sourceMock = new Mock<IActivitySource>();
        sourceMock.Setup(s => s.StartActivity(
                It.IsAny<string>(),
                ActivityKind.Internal,
                null))
            .Returns(activityMock.Object);

        // act
        sourceMock.Object.StartActivity("my-activity");

        // assert
        sourceMock.Verify(
            s => s.StartActivity("my-activity", ActivityKind.Internal, null),
            Times.Once);
    }

    [Fact]
    public void StartActivity_WithNameAndKind_UsesNullParent() {
        // arrange
        var activityMock = new Mock<IActivity>();
        var sourceMock = new Mock<IActivitySource>();
        sourceMock.Setup(s => s.StartActivity(
                It.IsAny<string>(),
                ActivityKind.Server,
                null))
            .Returns(activityMock.Object);

        // act
        sourceMock.Object.StartActivity("my-activity", ActivityKind.Server);

        // assert
        sourceMock.Verify(
            s => s.StartActivity("my-activity", ActivityKind.Server, null),
            Times.Once);
    }
}
