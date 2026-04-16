namespace Nameless.ProducerConsumer;

public sealed class ConsumerContext(IDictionary<string, object?>? dictionary = null)
    : Context(dictionary ?? new Dictionary<string, object?>());