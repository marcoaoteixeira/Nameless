using System.Collections;

namespace Nameless.Validation;

/// <summary>
/// Provides data context to the validation.
/// </summary>
public sealed record DataContext : IEnumerable<KeyValuePair<string, object?>> {
    private readonly Dictionary<string, object?> _data = [];

    /// <summary>
    /// Gets or sets the value using a key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    /// The value for a given key.
    /// </returns>
    public object? this[string key] {
        get => _data.GetValueOrDefault(key);
        set => _data[key] = value;
    }

    /// <summary>
    /// Gets the keys of the data context.
    /// </summary>
    public IEnumerable<string> Keys => _data.Keys;

    /// <summary>
    /// Gets the values of the data context.
    /// </summary>
    public IEnumerable<object?> Values => _data.Values;

    /// <summary>
    /// Clears the data context.
    /// </summary>
    public void Clear() {
        _data.Clear();
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
