using System.Collections;

namespace Nameless.Bootstrap;

/// <summary>
///     Represents a key/value context for storing and retrieving
///     arbitrary data associated with the <see cref="IStep"/>.
/// </summary>
public sealed class FlowContext : IEnumerable<KeyValuePair<string, object?>> {
    private readonly Dictionary<string, object?> _data = [];

    /// <summary>
    ///     Gets or sets the value using a key.
    /// </summary>
    /// <param name="key">
    ///     The key.
    /// </param>
    /// <returns>
    ///     The value for a given key.
    /// </returns>
    public object? this[string key] {
        get => _data.GetValueOrDefault(key);
        set => _data[Guard.Against.NullOrWhiteSpace(key)] = value;
    }

    /// <summary>
    ///     Gets the keys of the data context.
    /// </summary>
    public IEnumerable<string> Keys => _data.Keys;

    /// <summary>
    ///     Adds a new value to the data context. If the key already exists,
    ///     them update it.
    /// </summary>
    /// <param name="kvp">
    ///     The key/value pair.
    /// </param>
    public void Add(KeyValuePair<string, object?> kvp) {
        _data[Guard.Against.NullOrWhiteSpace(kvp.Key)] = kvp.Value;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
        return _data.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}