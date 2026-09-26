using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Application;

[IntegrationTest]
public class ApplicationContextLocationTests : IDisposable {
    private readonly string _appName = $"nameless-test-{Guid.NewGuid():N}";
    private readonly List<string> _created = [];

    public void Dispose() {
        foreach (var directory in _created.Where(SysDirectory.Exists)) {
            SysDirectory.Delete(directory, recursive: true);
        }
    }

    private ApplicationContext CreateSut(ApplicationDataLocation location, string? version = null) {
        var options = Options.Create(new ApplicationContextOptions {
            EnvironmentName = "test",
            ApplicationName = _appName,
            ApplicationDataLocation = location,
            Version = version
        });

        var logger = new LoggerMocker<ApplicationContext>().WithAnyLogLevel().Build();
        var sut = new ApplicationContext(options, logger);

        if (location is ApplicationDataLocation.Machine or ApplicationDataLocation.User) {
            _created.Add(sut.ApplicationDataDirectory);
        }

        return sut;
    }

    [Fact]
    public void ApplicationDataDirectory_ForUserLocation_IsUnderLocalApplicationData() {
        // act
        var sut = CreateSut(ApplicationDataLocation.User);

        // assert
        Assert.Multiple(
            () => Assert.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), sut.ApplicationDataDirectory),
            () => Assert.EndsWith(_appName, sut.ApplicationDataDirectory),
            () => Assert.True(SysDirectory.Exists(sut.ApplicationDataDirectory))
        );
    }

    [Fact]
    public void ApplicationDataDirectory_ForMachineLocation_IsUnderCommonApplicationData() {
        // act
        var sut = CreateSut(ApplicationDataLocation.Machine);

        // assert
        Assert.Multiple(
            () => Assert.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), sut.ApplicationDataDirectory),
            () => Assert.EndsWith(_appName, sut.ApplicationDataDirectory)
        );
    }

    [Fact]
    public void ApplicationDataDirectory_ForLocalLocation_IsAppDataUnderBaseDirectory() {
        // act
        var sut = CreateSut(ApplicationDataLocation.Local);

        // assert
        Assert.Multiple(
            () => Assert.Equal(SysPath.GetFullPath(SysPath.Combine(AppContext.BaseDirectory, "App_Data")), sut.ApplicationDataDirectory),
            () => Assert.True(SysDirectory.Exists(sut.ApplicationDataDirectory))
        );
    }

    [Fact]
    public void ApplicationDataDirectory_ForUnknownLocation_FallsBackToLocal() {
        // act
        var sut = CreateSut((ApplicationDataLocation)999);

        // assert
        Assert.Equal(SysPath.GetFullPath(SysPath.Combine(AppContext.BaseDirectory, "App_Data")), sut.ApplicationDataDirectory);
    }

    [Fact]
    public void Version_WithInvalidValue_FallsBackToDefault() {
        // act
        var sut = CreateSut(ApplicationDataLocation.User, version: "not-a-version");

        // assert
        Assert.Equal("1.0.0", sut.Version);
    }

    [Fact]
    public void Version_WithoutValue_FallsBackToDefault() {
        // act
        var sut = CreateSut(ApplicationDataLocation.User);

        // assert
        Assert.Equal("1.0.0", sut.Version);
    }

    [Fact]
    public void GetEnvironmentVariable_ReturnsProcessVariable() {
        // arrange
        var key = $"NAMELESS_TEST_{Guid.NewGuid():N}";
        Environment.SetEnvironmentVariable(key, "value");
        var sut = CreateSut(ApplicationDataLocation.User);

        try {
            // act & assert
            Assert.Multiple(
                () => Assert.Equal("value", sut.GetEnvironmentVariable(key)),
                () => Assert.Null(sut.GetEnvironmentVariable($"{key}_MISSING"))
            );
        }
        finally {
            Environment.SetEnvironmentVariable(key, null);
        }
    }
}
