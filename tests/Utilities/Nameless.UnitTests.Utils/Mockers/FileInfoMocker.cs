using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Nameless.Mockers;
public class FileInfoMocker : MockerBase<IFileInfo> {
    public FileInfoMocker WithExists(bool exists) {
        Mock.Setup(mock => mock.Exists)
                 .Returns(exists);

        return this;
    }

    public FileInfoMocker WithLength(long length) {
        Mock.Setup(mock => mock.Length)
                 .Returns(length);

        return this;
    }

    public FileInfoMocker WithPhysicalPath(string physicalPath) {
        Mock.Setup(mock => mock.PhysicalPath)
                 .Returns(physicalPath);

        return this;
    }

    public FileInfoMocker WithName(string name) {
        Mock.Setup(mock => mock.Name)
                 .Returns(name);

        return this;
    }

    public FileInfoMocker WithLastModified(DateTimeOffset lastModified) {
        Mock.Setup(mock => mock.LastModified)
                 .Returns(lastModified);

        return this;
    }

    public FileInfoMocker WithIsDirectory(bool isDirectory) {
        Mock.Setup(mock => mock.IsDirectory)
                 .Returns(isDirectory);

        return this;
    }

    public FileInfoMocker WithCreateReadStream(Stream readStream) {
        Mock.Setup(mock => mock.CreateReadStream())
                 .Returns(readStream);

        return this;
    }

    public FileInfoMocker WithCreateReadStream(string content) {
        Mock.Setup(mock => mock.CreateReadStream())
                 .Returns(new MemoryStream(Encoding.UTF8.GetBytes(content)));

        return this;
    }
}
