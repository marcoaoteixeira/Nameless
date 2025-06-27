using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IQueryCollection"/> that does not store any query parameters.
/// </summary>
public sealed class NullQueryCollection : IQueryCollection {
    public static IQueryCollection Instance { get; } = new NullQueryCollection();

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public ICollection<string> Keys => [];

    /// <inheritdoc />
    public StringValues this[string key] => default;

    static NullQueryCollection() { }

    private NullQueryCollection() { }

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