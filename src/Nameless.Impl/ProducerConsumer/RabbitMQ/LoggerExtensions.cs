using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.ProducerConsumer.RabbitMQ.Infrastructure;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

internal static class LoggerExtensions {
    extension<TMessage>(ILogger<Consumer<TMessage>> self) {
        internal void Shutdown(string consumerTag, string replyText) {
            Log.ConsumerShutdown(self, consumerTag, replyText);
        }

        internal void DeserializeEnvelopeFailure(Exception exception) {
            Log.ConsumerDeserializeEnvelopeFailure(self, exception);
        }

        internal void DeserializeMessageFailure(Exception exception) {
            Log.ConsumerDeserializeMessageFailure(self, exception);
        }

        internal void Started(string consumerTag, string? reply) {
            Log.ConsumerStarted(self, consumerTag, reply);
        }
    }

    extension(ILogger<Producer> self) {
        internal void Failure(Exception ex) {
            Log.Failure(self, "PRODUCER", $"{nameof(Producer)}.{nameof(Producer.ProduceAsync)}", ex);
        }

        internal void ChannelSemaphoreDisposed() {
            Log.ProducerChannelSemaphoreDisposed(self);
        }
    }

    extension(ILogger<ConnectionManager> self) {
        internal void BrokerUnreachable(ServerOptions server, Exception ex) {
            Log.ConnectionManagerBrokerUnreachable(
                self,
                serverInfo: JsonSerializer.Serialize(server),
                ex
            );
        }
    }
}