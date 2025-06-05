using Nameless.Infrastructure;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ApplicationContextMocker : MockerBase<IApplicationContext> {
    public ApplicationContextMocker WithEnvironment(string environment) {
        MockInstance.Setup(mock => mock.Environment)
                    .Returns(environment);

        return this;
    }

    public ApplicationContextMocker WithAppName(string appName) {
        MockInstance.Setup(mock => mock.AppName)
                    .Returns(appName);

        return this;
    }

    public ApplicationContextMocker WithAppBasePath(string appBasePath) {
        MockInstance.Setup(mock => mock.AppFolderPath)
                    .Returns(appBasePath);

        return this;
    }

    public ApplicationContextMocker WithAppDataFolderPath(string appDataFolderPath) {
        MockInstance.Setup(mock => mock.AppDataFolderPath)
                    .Returns(appDataFolderPath);

        return this;
    }

    public ApplicationContextMocker WithSemVer(string semVer) {
        MockInstance.Setup(mock => mock.Version)
                    .Returns(semVer);

        return this;
    }
}