namespace Nameless.ProducerConsumer;

public sealed class ProducerContext(IDictionary<string, object?>? dictionary = null)
    : Context(dictionary ?? new Dictionary<string, object?>());