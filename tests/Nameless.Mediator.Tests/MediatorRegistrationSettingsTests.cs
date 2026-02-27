namespace Nameless.Mediator;

public class MediatorRegistrationSettingsTests {
    [Fact]
    public void WhenRegisteringRequestPipelineBehavior_WhenTypeIsNotAssignable_ThenThrowsException() {
        // arrange

        // act
        var exception = Record.Exception(() => new MediatorRegistrationSettings().RegisterRequestPipelineBehavior(typeof(string)));

        // assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void WhenRegisteringStreamPipelineBehavior_WhenTypeIsNotAssignable_ThenThrowsException() {
        // arrange

        // act
        var exception = Record.Exception(() => new MediatorRegistrationSettings().RegisterStreamPipelineBehavior(typeof(string)));

        // assert
        Assert.IsType<ArgumentException>(exception);
    }
}