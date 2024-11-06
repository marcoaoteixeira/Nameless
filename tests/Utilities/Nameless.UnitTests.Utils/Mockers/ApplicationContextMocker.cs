using Nameless.Infrastructure;

namespace Nameless.Mockers;

public class ApplicationContextMocker : MockerBase<IApplicationContext> {
    public ApplicationContextMocker WithEnvironment(string environment) {
        InnerMock.Setup(mock => mock.Environment).Returns(environment);

        return this;
    }

    public ApplicationContextMocker WithAppName(string appName) {
        InnerMock.Setup(mock => mock.AppName).Returns(appName);

        return this;
    }

    public ApplicationContextMocker WithAppBasePath(string appBasePath) {
        InnerMock.Setup(mock => mock.AppBasePath).Returns(appBasePath);

        return this;
    }

    public ApplicationContextMocker WithAppDataFolderPath(string appDataFolderPath) {
        InnerMock.Setup(mock => mock.AppDataFolderPath).Returns(appDataFolderPath);

        return this;
    }

    public ApplicationContextMocker WithSemVer(string semVer) {
        InnerMock.Setup(mock => mock.SemVer).Returns(semVer);

        return this;
    }
}