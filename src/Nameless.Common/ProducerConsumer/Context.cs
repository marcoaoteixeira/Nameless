using System.Collections;
using System.Collections.Concurrent;

namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a context that can hold key/value.
/// </summary>
public abstract class Context : IEnumerable<KeyValuePair<string, object?>> {
    private readonly ConcurrentDictionary<string, object?> _props = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Gets or sets a value to the context for a given key.
    /// </summary>
    /// <param name="key">
    ///     The key. Cannot be null, empty or only whitespace.
    /// </param>
    /// <returns>
    ///     The context value.
    /// </returns>
    public object? this[string key] {
        get => _props.GetValueOrDefault(Throws.When.NullOrWhiteSpace(key));
        set => _props[Throws.When.NullOrWhiteSpace(key)] = value;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Context"/> class.
    /// </summary>
    /// <param name="dictionary">
    ///     The initialization dictionary.
    /// </param>
    protected Context(Dictionary<string, object?> dictionary) {
        foreach (var kvp in dictionary) {
            this[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    ///     Adds a key/value to the context. If the key already exists,
    ///     it gets replaced.
    /// </summary>
    /// <param name="kvp">
    ///     The key/value pair.
    /// </param>
    public void Add(KeyValuePair<string, object?> kvp) {
        this[kvp.Key] = kvp.Value;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
        return _props.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}