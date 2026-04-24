using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nameless.IO.FileSystem;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Application;

[UnitTest]
public class ApplicationContextTests {
    [Fact]
    public void WhenCreatingApplicationContext_ThenPropertiesAreMappedAndFileSystemProviderIsCreated() {
        // arrange
        var options = Options.Create(new ApplicationContextOptions {
            EnvironmentName = "unittest",
            ApplicationName = "nameless.test.app",
            ApplicationDataLocation = ApplicationDataLocation.Base,
            Version = new SemanticVersion(1, 2, 3)
        });

        var logger = new Mock<ILogger<ApplicationContext>>().Object;

        // act
        var sut = new ApplicationContext(options, logger);

        // assert - basic properties
        Assert.Equal("unittest", sut.EnvironmentName);
        Assert.Equal("nameless.test.app", sut.ApplicationName);
        Assert.Equal(AppDomain.CurrentDomain.BaseDirectory, sut.BaseDirectoryPath);
        Assert.Equal("v1.2.3", sut.Version);

        // assert - FileSystemProvider created and cached
        var fsp1 = sut.FileSystemProvider;
        Assert.NotNull(fsp1);
        Assert.IsType<FileSystemProvider>(fsp1);
        Assert.True(Directory.Exists(fsp1.Root));
        Assert.StartsWith(sut.BaseDirectoryPath, fsp1.Root, StringComparison.OrdinalIgnoreCase);

        var fsp2 = sut.FileSystemProvider;
        Assert.Same(fsp1, fsp2);
    }
}
