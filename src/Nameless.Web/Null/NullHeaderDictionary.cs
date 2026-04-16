using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IHeaderDictionary"/> that does not store any headers.
/// </summary>
public sealed class NullHeaderDictionary : IHeaderDictionary {
    /// <summary>
    ///     Gets the singleton instance of <see cref="NullHeaderDictionary"/>.
    /// </summary>
    public static IHeaderDictionary Instance { get; } = new NullHeaderDictionary();

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public bool IsReadOnly => true;

    public StringValues this[string key] {
        get => default;
        set => _ = value;
    }

    /// <inheritdoc />
    public long? ContentLength {
        get => null;
        set => _ = value;
    }

    /// <inheritdoc />
    public ICollection<string> Keys => [];

    /// <inheritdoc />
    public ICollection<StringValues> Values => [];

    static NullHeaderDictionary() { }

    private NullHeaderDictionary() { }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() {
        return Enumerable.Empty<KeyValuePair<string, StringValues>>().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<string, StringValues> item) { }

    /// <inheritdoc />
    public void Clear() { }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, StringValues> item) {
        return false;
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<string, StringValues>[] array, int arrayIndex) { }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, StringValues> item) {
        return false;
    }

    /// <inheritdoc />
    public void Add(string key, StringValues value) { }

    /// <inheritdoc />
    public bool ContainsKey(string key) {
        return false;
    }

    /// <inheritdoc />
    public bool Remove(string key) {
        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, out StringValues value) {
        value = default;

        return false;
    }
}