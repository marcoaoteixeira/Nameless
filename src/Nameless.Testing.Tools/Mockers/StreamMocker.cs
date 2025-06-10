using Moq;

namespace Nameless.Testing.Tools.Mockers;
public class StreamMocker : MockerBase<Stream> {
    public StreamMocker WithCanRead(bool result = true) {
        MockInstance
           .Setup(mock => mock.CanRead)
           .Returns(result);

        return this;
    }

    public StreamMocker WithCanSeek(bool result = true) {
        MockInstance
           .Setup(mock => mock.CanSeek)
           .Returns(result);

        return this;
    }

    public StreamMocker WithSeek(long result = 0) {
        MockInstance
           .Setup(mock => mock.Seek(It.IsAny<int>(), It.IsAny<SeekOrigin>()))
           .Returns(result);

        return this;
    }
}
