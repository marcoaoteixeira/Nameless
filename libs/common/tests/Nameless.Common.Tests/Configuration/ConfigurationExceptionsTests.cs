namespace Nameless.Configuration;

[UnitTest]
public class ConfigurationExceptionsTests {
    [Fact]
    public void WhenInitialize_ConfigurationBindingException_MentionsPathAndType() {
        // act
        var sut = new ConfigurationBindingException("Some:Path", typeof(string));

        // assert
        Assert.Multiple(
            () => Assert.Contains("Some:Path", sut.Message),
            () => Assert.Contains("System.String", sut.Message)
        );
    }
}
