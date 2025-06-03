using Microsoft.Extensions.Hosting;
using Moq;

namespace Nameless;

public class HostEnvironmentExtensionsTests {
    [Fact]
    public void IsDeveloperMachine_Should_Returns_True_If_Developer_Env() {
        // arrange
        var hostEnvironmentMock = new Mock<IHostEnvironment>();
        hostEnvironmentMock
           .Setup(_ => _.EnvironmentName)
           .Returns(HostEnvironmentExtensions.DeveloperMachine);

        // act
        var actual = hostEnvironmentMock.Object.IsDeveloperMachine();

        // assert
        Assert.That(actual, Is.True);
    }

    [Fact]
    public void IsDeveloperMachine_Should_Returns_False_If_Not_Developer_Env() {
        // arrange
        var hostEnvironmentMock = new Mock<IHostEnvironment>();
        hostEnvironmentMock
           .Setup(_ => _.EnvironmentName)
           .Returns("Production");

        // act
        var actual = hostEnvironmentMock.Object.IsDeveloperMachine();

        // assert
        Assert.That(actual, Is.False);
    }

    [Fact]
    public void IsRunningOnContainer_Should_Returns_True_If_Containerized() {
        // arrange
        Environment.SetEnvironmentVariable(EnvironmentTokens.DOTNET_RUNNING_IN_CONTAINER, "true");

        // act
        var actual = Mock.Of<IHostEnvironment>().IsRunningOnContainer();

        // assert
        Assert.That(actual, Is.True);
    }

    [Fact]
    public void IsRunningOnContainer_Should_Returns_False_If_Containerized() {
        // arrange
        Environment.SetEnvironmentVariable(EnvironmentTokens.DOTNET_RUNNING_IN_CONTAINER, null);

        // act
        var actual = Mock.Of<IHostEnvironment>().IsRunningOnContainer();

        // assert
        Assert.That(actual, Is.False);
    }

    [Fact]
    public void GetEnvironmentVariable_Should_Returns_Environment_Values() {
        // arrange
        var id = Guid.NewGuid().ToString();
        var expected = "123";
        Environment.SetEnvironmentVariable(id, expected);

        // act
        var actual = Mock.Of<IHostEnvironment>().GetEnvironmentVariable(id);

        // assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Fact]
    public void GetEnvironmentVariable_Should_Returns_Null_If_Environment_Variable_Does_Not_Exists() {
        // arrange
        var id = Guid.NewGuid().ToString();

        // act
        var actual = Mock.Of<IHostEnvironment>().GetEnvironmentVariable(id);

        // assert
        Assert.That(actual, Is.Null);
    }

    [Fact]
    public void SetEnvironmentVariable_Should_Define_Environment_Variable() {
        // arrange
        var id = Guid.NewGuid().ToString();
        var value = "123";

        // act
        Mock.Of<IHostEnvironment>().SetEnvironmentVariable(id, value);
        var actual = Environment.GetEnvironmentVariable(id);

        // assert
        Assert.That(actual, Is.EqualTo(value));
    }
}