using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Infrastructure;

public class ApplicationContextTests {
    [TestCase(Environment.SpecialFolder.CommonApplicationData)]
    [TestCase(Environment.SpecialFolder.LocalApplicationData)]
    public void WhenBuildingApplicationContext_ThenReturnCorrectValuesForProperties(Environment.SpecialFolder specialFolder) {
        // arrange
        const string environment = "Development";
        const string appName = "Test App";
        var appVersion = new Version(1, 2, 3);
        var baseAppFolder = AppDomain.CurrentDomain.BaseDirectory;
        var appDataFolder = Path.Combine(Environment.GetFolderPath(specialFolder), appName);

        // act
        var sut = new ApplicationContext(environment,
                                         appName,
                                         useCommonAppDataFolder: specialFolder == Environment.SpecialFolder.CommonApplicationData,
                                         appVersion,
                                         NullLogger<ApplicationContext>.Instance);
        
        // assert
        Assert.Multiple(() => {
            Assert.That(sut.Version, Is.EqualTo($"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}"));
            Assert.That(sut.AppDataFolderPath, Is.EqualTo(appDataFolder));
            Assert.That(sut.AppFolderPath, Is.EqualTo(baseAppFolder));
            Assert.That(sut.AppName, Is.EqualTo(appName));
            Assert.That(sut.Environment, Is.EqualTo(environment));
        });
    }
}
