using Nameless.Infrastructure;

namespace Nameless.Testing.Tools.Mockers;

public sealed class ApplicationContextMocker : Mocker<IApplicationContext> {
    public ApplicationContextMocker WithEnvironment(string returnValue) {
        MockInstance.Setup(mock => mock.EnvironmentName)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithAppName(string returnValue) {
        MockInstance.Setup(mock => mock.ApplicationName)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithAppBasePath(string returnValue) {
        MockInstance.Setup(mock => mock.ApplicationFolderPath)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithAppDataFolderPath(string returnValue) {
        MockInstance.Setup(mock => mock.ApplicationDataFolderPath)
                    .Returns(returnValue);

        return this;
    }

    public ApplicationContextMocker WithSemVer(string resul) {
        MockInstance.Setup(mock => mock.Version)
                    .Returns(resul);

        return this;
    }
}