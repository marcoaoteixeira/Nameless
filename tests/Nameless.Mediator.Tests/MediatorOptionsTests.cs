namespace Nameless.Mediator;

public class MediatorOptionsTests {
    [Fact]
    public void WhenRegisteringRequestPipelineBehavior_WhenTypeIsNotAssignable_ThenThrowsException() {
        // arrange

        // act
        var exception = Record.Exception(() => new MediatorOptions().RegisterRequestPipelineBehavior(typeof(string)));

        // assert
        Assert.IsType<InvalidOperationException>(exception);
    }

    [Fact]
    public void WhenRegisteringStreamPipelineBehavior_WhenTypeIsNotAssignable_ThenThrowsException() {
        // arrange

        // act
        var exception = Record.Exception(() => new MediatorOptions().RegisterStreamPipelineBehavior(typeof(string)));

        // assert
        Assert.IsType<InvalidOperationException>(exception);
    }
}
