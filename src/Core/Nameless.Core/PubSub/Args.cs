using System.Collections;

namespace Nameless.PubSub;

/// <summary>
/// Represents a collection of arguments. Argument's key is case-sensitive.
/// </summary>
public abstract record Args : IEnumerable<KeyValuePair<string, object>> {
    private readonly Dictionary<string, object> _args = [];

    /// <summary>
    /// Gets or sets an argument given a key.
    /// </summary>
    /// <param name="key">The argument key.</param>
    /// <returns>The value if it exists; otherwise <c>null</c>.</returns>
    public object? this[string key] {
        get => Get(key);
        set => Set(key, value);
    }

    private object? Get(string name)
        => _args.GetValueOrDefault(name);

    private void Set(string name, object? value) {
        if (value is null) {
            _args.Remove(name);
            return;
        }
        _args[name] = value;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        => _args.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _args.GetEnumerator();
}