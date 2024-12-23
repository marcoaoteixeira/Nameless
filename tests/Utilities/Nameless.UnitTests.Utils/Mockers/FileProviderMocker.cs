using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Nameless.Mockers;

public class FileProviderMocker : MockerBase<IFileProvider> {
    public FileProviderMocker WithFileInfo(IFileInfo fileInfo) {
        Mock.Setup(mock => mock.GetFileInfo(It.IsAny<string>()))
                 .Returns(fileInfo);

        return this;
    }

    public FileProviderMocker WithFileInfo(IFileInfo fileInfo, string subPath) {
        Mock.Setup(mock => mock.GetFileInfo(subPath))
                 .Returns(fileInfo);

        return this;
    }

    public FileProviderMocker WithWatch(IChangeToken changeToken) {
        Mock.Setup(mock => mock.Watch(It.IsAny<string>()))
                 .Returns(changeToken);

        return this;
    }

    public FileProviderMocker WithWatch(IChangeToken changeToken, string filter) {
        Mock.Setup(mock => mock.Watch(filter))
                 .Returns(changeToken);

        return this;
    }
}
