using System.Text;
using System.Text.Json;

namespace Nameless.ProducerConsumer.RabbitMQ {

    internal static class MessageExtension {

        #region Internal Static Methods

        internal static byte[] AsBuffer(this Message self) {
            if (self == null) { return Array.Empty<byte>(); }

            var json = JsonSerializer.Serialize(self);

            return Encoding.UTF8.GetBytes(json);
        }

        #endregion
    }
}
