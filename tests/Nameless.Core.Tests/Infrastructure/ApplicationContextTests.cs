using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless.Infrastructure;

public class ApplicationContextTests {
    [Theory]
    [InlineData(Environment.SpecialFolder.CommonApplicationData)]
    [InlineData(Environment.SpecialFolder.LocalApplicationData)]
    public void WhenBuildingApplicationContext_ThenReturnCorrectValuesForProperties(
        Environment.SpecialFolder specialFolder) {
        // arrange
        const string Environment = "Development";
        const string AppName = "Test App";
        var appVersion = new Version(1, 2, 3);
        var baseAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        var appDataFolder = Path.Combine(System.Environment.GetFolderPath(specialFolder), AppName);
        var options = Options.Create(new ApplicationContextOptions {
            EnvironmentName = Environment,
            ApplicationName = AppName,
            UseCommonAppDataFolder = specialFolder == System.Environment.SpecialFolder.CommonApplicationData,
            Version = appVersion
        });
        // act
        var sut = new ApplicationContext(options, NullLogger<ApplicationContext>.Instance);

        // assert
        Assert.Multiple(() => {
            Assert.Equal($"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}", sut.Version);
            Assert.Equal(appDataFolder, sut.ApplicationDataFolderPath);
            Assert.Equal(baseAppFolder, sut.ApplicationFolderPath);
            Assert.Equal(AppName, sut.ApplicationName);
            Assert.Equal(Environment, sut.EnvironmentName);
        });
    }
}