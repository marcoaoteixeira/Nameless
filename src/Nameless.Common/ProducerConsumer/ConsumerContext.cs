namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a consumer context.
/// </summary>
/// <param name="dictionary">
///     The initialization dictionary.
/// </param>
public sealed class ConsumerContext(Dictionary<string, object?>? dictionary = null) : Context(dictionary ?? []);