using System.Text;
using Microsoft.Extensions.FileProviders;

namespace Nameless.Testing.Tools.Mockers;

public sealed class FileInfoMocker : MockerBase<IFileInfo> {
    public FileInfoMocker WithExists(bool exists) {
        MockInstance.Setup(mock => mock.Exists)
                    .Returns(exists);

        return this;
    }

    public FileInfoMocker WithLength(long length) {
        MockInstance.Setup(mock => mock.Length)
                    .Returns(length);

        return this;
    }

    public FileInfoMocker WithPhysicalPath(string physicalPath) {
        MockInstance.Setup(mock => mock.PhysicalPath)
                    .Returns(physicalPath);

        return this;
    }

    public FileInfoMocker WithName(string name) {
        MockInstance.Setup(mock => mock.Name)
                    .Returns(name);

        return this;
    }

    public FileInfoMocker WithLastModified(DateTimeOffset lastModified) {
        MockInstance.Setup(mock => mock.LastModified)
                    .Returns(lastModified);

        return this;
    }

    public FileInfoMocker WithIsDirectory(bool isDirectory) {
        MockInstance.Setup(mock => mock.IsDirectory)
                    .Returns(isDirectory);

        return this;
    }

    public FileInfoMocker WithCreateReadStream(Stream stream) {
        MockInstance.Setup(mock => mock.CreateReadStream())
                    .Returns(stream);

        return this;
    }

    public FileInfoMocker WithCreateReadStream(string content) {
        MockInstance.Setup(mock => mock.CreateReadStream())
                    .Returns(new MemoryStream(Encoding.UTF8.GetBytes(content)));

        return this;
    }
}