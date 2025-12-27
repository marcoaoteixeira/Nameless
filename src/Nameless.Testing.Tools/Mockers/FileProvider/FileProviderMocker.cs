using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Testing.Tools.Mockers.FileProvider;

public sealed class FileProviderMocker : Mocker<IFileProvider> {
    public FileProviderMocker WithGetFileInfo(IFileInfo returnValue) {
        MockInstance.Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public FileProviderMocker WithGetFileInfo(string subPath, IFileInfo returnValue) {
        MockInstance.Setup(mock => mock.GetFileInfo(subPath))
                    .Returns(returnValue);

        return this;
    }

    public FileProviderMocker WithWatch(IChangeToken returnValue) {
        MockInstance.Setup(mock => mock.Watch(It.IsAny<string>()))
                    .Returns(returnValue);

        return this;
    }

    public FileProviderMocker WithWatch(string filter, IChangeToken returnValue) {
        MockInstance.Setup(mock => mock.Watch(filter))
                    .Returns(returnValue);

        return this;
    }
}