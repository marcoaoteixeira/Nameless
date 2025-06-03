using System.Collections;

namespace Nameless.PubSub;

/// <summary>
///     Represents a collection of arguments. Argument's key is case-sensitive.
/// </summary>
public abstract record Args : IEnumerable<KeyValuePair<string, object>> {
    private readonly Dictionary<string, object> _args = [];

    /// <summary>
    ///     Gets or sets an argument given a key.
    /// </summary>
    /// <param name="key">The argument key.</param>
    /// <returns>The value if it exists; otherwise <c>null</c>.</returns>
    public object? this[string key] {
        get => Get(key);
        set => Set(key, value);
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
        return _args.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <summary>
    /// Retrieves an argument by its name.
    /// </summary>
    /// <param name="name">The argument name.</param>
    /// <returns>
    /// The value of the argument if it exists; otherwise <c>null</c>.
    /// </returns>
    private object? Get(string name) {
        return _args.GetValueOrDefault(name);
    }

    /// <summary>
    /// Sets an argument by its name.
    /// </summary>
    /// <param name="name">The argument name.</param>
    /// <param name="value">The argument value.</param>
    private void Set(string name, object? value) {
        if (value is null) {
            _args.Remove(name);
            return;
        }

        _args[name] = value;
    }
}