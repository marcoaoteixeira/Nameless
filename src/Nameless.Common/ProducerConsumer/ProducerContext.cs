namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a producer context.
/// </summary>
/// <param name="dictionary">
///     The initialization dictionary.
/// </param>
public sealed class ProducerContext(Dictionary<string, object?>? dictionary = null) : Context(dictionary ?? []);