using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Testing.Tools.Mockers;

public sealed class FileProviderMocker : MockerBase<IFileProvider> {
    public FileProviderMocker WithFileInfo(IFileInfo fileInfo) {
        MockInstance.Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
                    .Returns(fileInfo);

        return this;
    }

    public FileProviderMocker WithFileInfo(IFileInfo fileInfo, string subPath) {
        MockInstance.Setup(mock => mock.GetFileInfo(subPath))
                    .Returns(fileInfo);

        return this;
    }

    public FileProviderMocker WithWatch(IChangeToken changeToken) {
        MockInstance.Setup(mock => mock.Watch(It.IsAny<string>()))
                    .Returns(changeToken);

        return this;
    }

    public FileProviderMocker WithWatch(IChangeToken changeToken, string filter) {
        MockInstance.Setup(mock => mock.Watch(filter))
                    .Returns(changeToken);

        return this;
    }
}