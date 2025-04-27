using Nameless.Infrastructure;

namespace Nameless.Mockers;

public class ApplicationContextMocker : MockerBase<IApplicationContext> {
    public ApplicationContextMocker WithEnvironment(string environment) {
        Mock.Setup(mock => mock.Environment).Returns(environment);

        return this;
    }

    public ApplicationContextMocker WithAppName(string appName) {
        Mock.Setup(mock => mock.AppName).Returns(appName);

        return this;
    }

    public ApplicationContextMocker WithAppBasePath(string appBasePath) {
        Mock.Setup(mock => mock.AppFolderPath).Returns(appBasePath);

        return this;
    }

    public ApplicationContextMocker WithAppDataFolderPath(string appDataFolderPath) {
        Mock.Setup(mock => mock.AppDataFolderPath).Returns(appDataFolderPath);

        return this;
    }

    public ApplicationContextMocker WithSemVer(string semVer) {
        Mock.Setup(mock => mock.Version).Returns(semVer);

        return this;
    }
}