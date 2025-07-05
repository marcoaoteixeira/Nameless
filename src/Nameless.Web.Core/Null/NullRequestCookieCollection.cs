using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IRequestCookieCollection"/> that does not store any cookies.
/// </summary>
public sealed class NullRequestCookieCollection : IRequestCookieCollection {
    public static IRequestCookieCollection Instance { get; } = new NullRequestCookieCollection();

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public ICollection<string> Keys => [];

    /// <inheritdoc />
    public string this[string key] => string.Empty;

    static NullRequestCookieCollection() { }

    private NullRequestCookieCollection() { }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
        return Enumerable.Empty<KeyValuePair<string, string>>().GetEnumerator();
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
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) {
        value = null;

        return false;
    }
}