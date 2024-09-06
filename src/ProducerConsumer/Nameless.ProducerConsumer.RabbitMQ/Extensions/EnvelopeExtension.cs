using System.Text;
using System.Text.Json;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class EnvelopeExtension {
    #region Public Static Methods

    public static ReadOnlyMemory<byte> CreateBuffer(this Envelope self) {
        var json = JsonSerializer.Serialize(self);

        return Encoding.UTF8.GetBytes(json);
    }

    #endregion
}