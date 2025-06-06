﻿using Microsoft.Extensions.Logging.Abstractions;

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

        // act
        var sut = new ApplicationContext(Environment,
            AppName,
            specialFolder == System.Environment.SpecialFolder.CommonApplicationData,
            appVersion,
            NullLogger<ApplicationContext>.Instance);

        // assert
        Assert.Multiple(() => {
            Assert.Equal($"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}", sut.Version);
            Assert.Equal(appDataFolder, sut.AppDataFolderPath);
            Assert.Equal(baseAppFolder, sut.AppFolderPath);
            Assert.Equal(AppName, sut.AppName);
            Assert.Equal(Environment, sut.Environment);
        });
    }
}