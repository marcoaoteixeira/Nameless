using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless.Infrastructure;

public class ApplicationContextTests {
    [Theory]
    [InlineData(Environment.SpecialFolder.CommonApplicationData)]
    [InlineData(Environment.SpecialFolder.LocalApplicationData)]
    public void WhenBuildingApplicationContext_ThenReturnCorrectValuesForProperties(
        Environment.SpecialFolder specialFolder) {
        // Due to restrictions on Linux systems, we'll not test the CommonApplicationData folder.
        if (OperatingSystem.IsLinux() && specialFolder == Environment.SpecialFolder.CommonApplicationData) {
            return;
        }

        // arrange
        const string EnvironmentName = "Development";
        const string ApplicationName = "Test_App";
        var appVersion = new Version(major: 1, minor: 2, build: 3);
        var baseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
        var dataDirectoryPath = Path.Combine(Environment.GetFolderPath(specialFolder), ApplicationName);
        var options = Options.Create(new ApplicationContextOptions {
            EnvironmentName = EnvironmentName,
            ApplicationName = ApplicationName,
            ApplicationDataLocation = specialFolder == Environment.SpecialFolder.CommonApplicationData
                ? ApplicationDataLocation.Machine
                : ApplicationDataLocation.User,
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