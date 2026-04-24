using Microsoft.Extensions.Hosting;

namespace Nameless.Extensions;

public class HostEnvironmentExtensionsTests {
    // --- IsDeveloperMachine ---

    [Fact]
    public void IsDeveloperMachine_WhenEnvironmentMatchesDeveloperMachine_ReturnsTrue() {
        // arrange
        var env = new FakeHostEnvironment { EnvironmentName = HostEnvironmentExtensions.DeveloperMachine };

        // act & assert
        Assert.True(env.IsDeveloperMachine);
    }

    [Fact]
    public void IsDeveloperMachine_WhenEnvironmentIsDevelopment_ReturnsFalse() {
        // arrange
        var env = new FakeHostEnvironment { EnvironmentName = Environments.Development };

        // act & assert
        Assert.False(env.IsDeveloperMachine);
    }

    // --- IsRunningOnContainer ---

    [Fact]
    public void IsRunningOnContainer_WhenEnvVarIsTrue_ReturnsTrue() {
        // arrange
        var env = new FakeHostEnvironment { EnvironmentName = Environments.Production };
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", "true");

        try {
            // act & assert
            Assert.True(env.IsRunningOnContainer);
        }
        finally {
            Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", null);
        }
    }

    [Fact]
    public void IsRunningOnContainer_WhenEnvVarIsAbsent_ReturnsFalse() {
        // arrange
        var env = new FakeHostEnvironment { EnvironmentName = Environments.Production };
        Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER", null);

        // act & assert
        Assert.False(env.IsRunningOnContainer);
    }

    // --- GetEnvironmentVariable / SetEnvironmentVariable ---

    [Fact]
    public void SetAndGetEnvironmentVariable_RoundTrips() {
        // arrange
        var env = new FakeHostEnvironment { EnvironmentName = Environments.Development };
        var varName = $"TEST_VAR_{Guid.NewGuid():N}";

        // act
        env.SetEnvironmentVariable(varName, "hello");
        var value = env.GetEnvironmentVariable(varName);

        // assert
        Assert.Equal("hello", value);

        // cleanup
        env.SetEnvironmentVariable(varName, null);
    }

    [Fact]
    public void GetEnvironmentVariable_WhenVariableDoesNotExist_ReturnsNull() {
        // arrange
        var env = new FakeHostEnvironment { EnvironmentName = Environments.Development };
        var varName = $"NONEXISTENT_{Guid.NewGuid():N}";

        // act
        var value = env.GetEnvironmentVariable(varName);

        // assert
        Assert.Null(value);
    }

    // --- test doubles ---

    private sealed class FakeHostEnvironment : IHostEnvironment {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = "/";
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
