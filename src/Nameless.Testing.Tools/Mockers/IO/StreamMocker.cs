using Moq;

namespace Nameless.Testing.Tools.Mockers.IO;
public class StreamMocker : Mocker<Stream> {
    public StreamMocker WithCanRead(bool returnValue = true) {
        MockInstance
           .Setup(mock => mock.CanRead)
           .Returns(returnValue);

        return this;
    }

    public StreamMocker WithCanSeek(bool returnValue = true) {
        MockInstance
           .Setup(mock => mock.CanSeek)
           .Returns(returnValue);

        return this;
    }

    public StreamMocker WithSeek(long returnValue = 0) {
        MockInstance
           .Setup(mock => mock.Seek(It.IsAny<int>(), It.IsAny<SeekOrigin>()))
           .Returns(returnValue);

        return this;
    }
}
