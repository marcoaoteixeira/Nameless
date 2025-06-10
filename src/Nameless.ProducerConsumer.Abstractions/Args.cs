using System.Collections;

namespace Nameless.ProducerConsumer;

/// <summary>
///     Represents a collection of arguments. Argument's key is case-sensitive.
/// </summary>
public record Args : IEnumerable<KeyValuePair<string, object>> {
    private readonly Dictionary<string, object?> _args = [];

    /// <summary>
    ///     Gets or sets an argument given a key.
    /// </summary>
    /// <param name="key">The argument key.</param>
    /// <returns>The value if it exists; otherwise <see langword="null"/>.</returns>
    public object? this[string key] {
        get => _args.GetValueOrDefault(key);
        set => _args[key] = value;
    }

    public void Add(string key, object? value) {
        Prevent.Argument.NullOrWhiteSpace(key);

        _args[key] = value;
    }

    /// <summary>
    ///     Removes an argument from the collection.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    ///     <see langword="true"/> if the argument was successfully removed; otherwise, <see langword="false"/>.
    ///     This will be <see langword="false"/> if the key was not found in the collection.
    /// </returns>
    public bool Remove(string key) {
        return _args.Remove(key);
    }

    /// <summary>
    ///     Clears all arguments in the collection.
    /// </summary>
    public void Clear() {
        _args.Clear();
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
        foreach (var kvp in _args) {
            if (kvp.Value is not null) {
                yield return new KeyValuePair<string, object>(kvp.Key, kvp.Value);
            }
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}