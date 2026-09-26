using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.ProducerConsumer.RabbitMQ;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Internal)]
internal static class Constants {
    internal static TimeSpan MessageConfirmationTimeout { get; } = TimeSpan.FromMilliseconds(15_000);

    internal static class Queues {
        internal const string Default = "q.default";
    }
}
