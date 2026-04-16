using Nameless.Resilience;

namespace Nameless.ProducerConsumer;

public interface IConsumer<in TMessage> {
    string Name { get; }

    string Topic { get; }
    
    RetryPolicyConfiguration? RetryPolicy { get; }

    Task ConsumeAsync(TMessage message, ConsumerContext context, CancellationToken cancellationToken);
}