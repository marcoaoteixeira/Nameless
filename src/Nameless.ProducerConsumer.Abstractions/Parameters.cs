using System.Collections;

namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a collection of parameters. Parameter's name is case-insensitive.
/// </summary>
public sealed record Parameters : IEnumerable<KeyValuePair<string, object?>> {
    private readonly Dictionary<string, object?> _data = new(StringComparer.CurrentCultureIgnoreCase);

    /// <summary>
    ///     Gets or sets a parameter for a given name.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns>The value if it exists; otherwise <see langword="null"/>.</returns>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <see langword="null"/>.
    /// </exception>
    public object? this[string name] {
        get => _data.GetValueOrDefault(Guard.Against.NullOrWhiteSpace(name));
        set => _data[Guard.Against.NullOrWhiteSpace(name)] = value;
    }

    /// <summary>
    /// Adds a new parameter using the Key/Value pair.
    /// </summary>
    /// <param name="kvp">The key/value pair.</param>
    /// <exception cref="ArgumentException">
    /// if <paramref name="kvp"/> Key is empty, white spaces or
    /// the key already exists.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="kvp"/> Key is <see langword="null"/>.
    /// </exception>
    public void Add(KeyValuePair<string, object?> kvp) {
        _data.Add(Guard.Against.NullOrWhiteSpace(kvp.Key), kvp.Value);
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
