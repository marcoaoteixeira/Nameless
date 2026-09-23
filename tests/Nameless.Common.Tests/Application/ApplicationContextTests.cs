using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.IO.System;

namespace Nameless.Application;

[UnitTest]
public class ApplicationContextTests {
    [Fact]
    public void WhenCreatingApplicationContext_ThenPropertiesAreMappedAndFileSystemProviderIsCreated() {
        // arrange
        var options = Options.Create(new ApplicationContextOptions {
            EnvironmentName = "unittest",
            ApplicationName = "nameless.test.app",
            ApplicationDataLocation = ApplicationDataLocation.Local,
            Version = "v1.2.3"
        });

        var logger = new Mock<ILogger<ApplicationContext>>().Object;

        // act
        var sut = new ApplicationContext(options, logger);

        // assert - basic properties
        Assert.Multiple(
            () => Assert.Equal("unittest", sut.EnvironmentName),
            () => Assert.Equal("nameless.test.app", sut.ApplicationName),
            () => Assert.Equal(SysPath.GetFullPath(SysPath.Combine(AppContext.BaseDirectory, "App_Data")), sut.ApplicationDataDirectory),
            () => Assert.Equal("v1.2.3", sut.Version)
        );

        // assert - SystemFileExplorer created and cached
        var fsp1 = sut.ApplicationDataFileProvider;
        Assert.Multiple(
            () => Assert.NotNull(fsp1),
            () => Assert.IsType<FileProvider>(fsp1),
            () => Assert.True(SysDirectory.Exists(fsp1.Root)),
            () => Assert.StartsWith(sut.ApplicationDataDirectory, fsp1.Root, StringComparison.OrdinalIgnoreCase)
        );

        var fsp2 = sut.ApplicationDataFileProvider;
        Assert.Same(fsp1, fsp2);
    }
}
