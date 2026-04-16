using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IFormCollection"/> that does not store any form data.
/// </summary>
public sealed class NullFormCollection : IFormCollection {
    public static IFormCollection Instance { get; } = new NullFormCollection();

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public ICollection<string> Keys => [];

    /// <inheritdoc />
    public StringValues this[string key] => default;

    /// <inheritdoc />
    public IFormFileCollection Files => NullFormFileCollection.Instance;

    static NullFormCollection() { }

    private NullFormCollection() { }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() {
        return Enumerable.Empty<KeyValuePair<string, StringValues>>().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public bool ContainsKey(string key) {
        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, out StringValues value) {
        value = default;

        return false;
    }
}