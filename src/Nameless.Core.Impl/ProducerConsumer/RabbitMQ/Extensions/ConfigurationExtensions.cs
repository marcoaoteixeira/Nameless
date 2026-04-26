using Microsoft.Extensions.Configuration;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

public static class ConfigurationExtensions {
    private const string RABBIT_MQ_SECTION_NAME = "RabbitMQ";

    extension(IConfiguration self) {
        public ServerOptions GetServerOptions() {
            var section = self.GetSection(RABBIT_MQ_SECTION_NAME)
                              .GetSection<ServerOptions>();

            return section.Get<ServerOptions>() ?? throw new InvalidOperationException(
                $"Couldn't convert configuration section into '{nameof(ServerOptions)}'."
            );
        }

        public QueueOptions GetQueueOptions(string queueName) {
            var section = self.GetSection(RABBIT_MQ_SECTION_NAME)
                              .GetSection<QueueOptions>()
                              .GetChildren()
                              .SingleOrDefault(section => section.Key == queueName)
                          ?? throw new InvalidOperationException($"Missing configuration section for queue '{queueName}'.");
            
            return section.Get<QueueOptions>() ?? throw new InvalidOperationException(
                $"Couldn't convert configuration section into '{nameof(QueueOptions)}'."
            );
        }

        public PrefetchOptions GetPrefetchOptions() {
            var section = self.GetSection(RABBIT_MQ_SECTION_NAME)
                              .GetSection<PrefetchOptions>();

            return section.Get<PrefetchOptions>() ?? throw new InvalidOperationException(
                $"Couldn't convert configuration section into '{nameof(PrefetchOptions)}'."
            );
        }
    }
}
