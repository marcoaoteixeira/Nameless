using System.Collections;
using Microsoft.AspNetCore.Http.Features;

namespace Nameless.Web.Null;

/// <summary>
///     Null implementation of <see cref="IFeatureCollection"/> that does not store any features.
/// </summary>
public sealed class NullFeatureCollection : IFeatureCollection {
    public static IFeatureCollection Instance { get; } = new NullFeatureCollection();

    /// <inheritdoc />
    public object? this[Type key] {
        get => null;
        set => _ = value;
    }

    /// <inheritdoc />
    public int Revision => 0;

    /// <inheritdoc />
    public bool IsReadOnly => true;

    static NullFeatureCollection() { }

    private NullFeatureCollection() { }

    /// <inheritdoc />
    public void Set<TFeature>(TFeature? instance) { }

    /// <inheritdoc />
    public TFeature? Get<TFeature>() {
        return default;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() {
        return Enumerable.Empty<KeyValuePair<Type, object>>().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}