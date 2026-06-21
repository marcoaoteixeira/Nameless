using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.Registration;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IQueryCollection"/> that does not store any query parameters.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.TrivialCode)]
[IgnoreAssemblyScan]
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