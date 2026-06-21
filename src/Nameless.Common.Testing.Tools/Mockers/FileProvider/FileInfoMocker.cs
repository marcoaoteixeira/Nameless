using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Nameless.Testing.Tools.Mockers.FileProvider;

public class FileInfoMocker : Mocker<IFileInfo> {
    public FileInfoMocker WithExists(bool returnValue) {
        MockInstance.Setup(mock => mock.Exists)
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithLength(long returnValue) {
        MockInstance.Setup(mock => mock.Length)
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithPhysicalPath(string returnValue) {
        MockInstance.Setup(mock => mock.PhysicalPath)
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithName(string returnValue) {
        MockInstance.Setup(mock => mock.Name)
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithLastModified(DateTimeOffset returnValue) {
        MockInstance.Setup(mock => mock.LastModified)
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithIsDirectory(bool returnValue) {
        MockInstance.Setup(mock => mock.IsDirectory)
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithCreateReadStream(Stream returnValue) {
        MockInstance.Setup(mock => mock.CreateReadStream())
                    .Returns(returnValue);

        return this;
    }

    public FileInfoMocker WithCreateReadStream(string returnValue) {
        MockInstance.Setup(mock => mock.CreateReadStream())
                    .Returns(new MemoryStream(Encoding.UTF8.GetBytes(returnValue)));

        return this;
    }
}