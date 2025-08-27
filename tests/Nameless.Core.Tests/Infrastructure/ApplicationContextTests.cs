using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless.Infrastructure;

public class ApplicationContextTests {
    [Theory]
    [InlineData(Environment.SpecialFolder.CommonApplicationData)]
    [InlineData(Environment.SpecialFolder.LocalApplicationData)]
    public void WhenBuildingApplicationContext_ThenReturnCorrectValuesForProperties(Environment.SpecialFolder specialFolder) {
        // arrange
        const string EnvironmentName = "Development";
        const string ApplicationName = "Test App";
        var appVersion = new Version(1, 2, 3);
        var baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
        var dataDirectoryPath = Path.Combine(Environment.GetFolderPath(specialFolder), ApplicationName);
        var options = Options.Create(new ApplicationContextOptions {
            EnvironmentName = EnvironmentName,
            ApplicationName = ApplicationName,
            UseLocalApplicationData = specialFolder == Environment.SpecialFolder.LocalApplicationData,
            Version = appVersion
        });
        // act
        var sut = new ApplicationContext(options, NullLogger<ApplicationContext>.Instance);

        // assert
        Assert.Multiple(() => {
            Assert.Equal($"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}", sut.Version);
            Assert.Equal(dataDirectoryPath, sut.DataDirectoryPath);
            Assert.Equal(baseDirectoryPath, sut.BaseDirectoryPath);
            Assert.Equal(ApplicationName, sut.ApplicationName);
            Assert.Equal(EnvironmentName, sut.EnvironmentName);
        });
    }
}