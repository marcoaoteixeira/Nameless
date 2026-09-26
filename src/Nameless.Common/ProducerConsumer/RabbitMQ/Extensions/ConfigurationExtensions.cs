using Microsoft.Extensions.Configuration;
using Nameless.ProducerConsumer.RabbitMQ.Options;

namespace Nameless.ProducerConsumer.RabbitMQ;

/// <summary>
///     <see cref="IConfiguration"/> extension methods for RabbitMQ configuration.
/// </summary>
public static class ConfigurationExtensions {
    private const string RABBIT_MQ_SECTION_NAME = "RabbitMQ";

    /// <param name="self">
    ///     The current <see cref="IConfiguration"/> instance.
    /// </param>
    extension(IConfiguration self) {
        /// <summary>
        ///     Reads the RabbitMQ server options from the configuration.
        /// </summary>
        /// <returns>
        ///     A <see cref="ServerOptions"/> instance populated from the
        ///     <c>RabbitMQ:Server</c> configuration section.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     if the configuration section cannot be converted into
        ///     <see cref="ServerOptions"/>.
        /// </exception>
        public ServerOptions GetServerOptions() {
            var section = self.GetSection(RABBIT_MQ_SECTION_NAME)
                              .GetSection<ServerOptions>();

            return section.Get<ServerOptions>() ?? throw new InvalidOperationException(
                $"Couldn't convert configuration section into '{nameof(ServerOptions)}'."
            );
        }

        /// <summary>
        ///     Reads the queue options for the specified queue name from the
        ///     configuration.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <returns>
        ///     A <see cref="QueueOptions"/> instance for the given queue.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     if the configuration section for <paramref name="queueName"/>
        ///     is missing or cannot be converted into <see cref="QueueOptions"/>.
        /// </exception>
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

        /// <summary>
        ///     Reads the RabbitMQ prefetch options from the configuration.
        /// </summary>
        /// <returns>
        ///     A <see cref="PrefetchOptions"/> instance populated from the
        ///     <c>RabbitMQ:Prefetch</c> configuration section.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     if the configuration section cannot be converted into
        ///     <see cref="PrefetchOptions"/>.
        /// </exception>
        public PrefetchOptions GetPrefetchOptions() {
            var section = self.GetSection(RABBIT_MQ_SECTION_NAME)
                              .GetSection<PrefetchOptions>();

            return section.Get<PrefetchOptions>() ?? throw new InvalidOperationException(
                $"Couldn't convert configuration section into '{nameof(PrefetchOptions)}'."
            );
        }
    }
}
