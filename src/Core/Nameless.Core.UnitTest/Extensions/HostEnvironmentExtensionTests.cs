using Microsoft.Extensions.Hosting;
using Moq;

namespace Nameless {
    public class HostEnvironmentExtensionTests {
        [Test]
        public void IsDeveloperMachine_Should_Returns_True_If_Developer_Env() {
            // arrange
            var hostEnvironmentMock = new Mock<IHostEnvironment>();
            hostEnvironmentMock
                .Setup(_ => _.EnvironmentName)
                .Returns(HostEnvironmentExtension.DeveloperMachine);

            // act
            var actual = HostEnvironmentExtension.IsDeveloperMachine(hostEnvironmentMock.Object);

            // assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsDeveloperMachine_Should_Returns_False_If_Not_Developer_Env() {
            // arrange
            var hostEnvironmentMock = new Mock<IHostEnvironment>();
            hostEnvironmentMock
                .Setup(_ => _.EnvironmentName)
                .Returns("Production");

            // act
            var actual = HostEnvironmentExtension.IsDeveloperMachine(hostEnvironmentMock.Object);

            // assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsRunningOnContainer_Should_Returns_True_If_Containerized() {
            // arrange
            Environment.SetEnvironmentVariable(Root.EnvTokens.DOTNET_RUNNING_IN_CONTAINER, "true");

            // act
            var actual = HostEnvironmentExtension.IsRunningOnContainer(Mock.Of<IHostEnvironment>());

            // assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void IsRunningOnContainer_Should_Returns_False_If_Containerized() {
            // arrange
            Environment.SetEnvironmentVariable(Root.EnvTokens.DOTNET_RUNNING_IN_CONTAINER, null);

            // act
            var actual = HostEnvironmentExtension.IsRunningOnContainer(Mock.Of<IHostEnvironment>());

            // assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void GetEnvironmentVariable_Should_Returns_Environment_Values() {
            // arrange
            var id = Guid.NewGuid().ToString();
            var expected = "123";
            Environment.SetEnvironmentVariable(id, expected);

            // act
            var actual = HostEnvironmentExtension.GetEnvironmentVariable(Mock.Of<IHostEnvironment>(), id);

            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetEnvironmentVariable_Should_Returns_Null_If_Environment_Variable_Does_Not_Exists() {
            // arrange
            var id = Guid.NewGuid().ToString();

            // act
            var actual = HostEnvironmentExtension.GetEnvironmentVariable(Mock.Of<IHostEnvironment>(), id);

            // assert
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void SetEnvironmentVariable_Should_Define_Environment_Variable() {
            // arrange
            var id = Guid.NewGuid().ToString();
            var value = "123";

            // act
            HostEnvironmentExtension.SetEnvironmentVariable(Mock.Of<IHostEnvironment>(), id, value);
            var actual = Environment.GetEnvironmentVariable(id);

            // assert
            Assert.That(actual, Is.EqualTo(value));
        }
    }
}
