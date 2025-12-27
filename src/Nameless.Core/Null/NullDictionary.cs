using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Nameless.Null;

/// <summary>
///     Null implementation of <see cref="IDictionary{TKey, TValue}"/> that does not store any key-value pairs.
/// </summary>
/// <typeparam name="TKey">Type of the key.</typeparam>
/// <typeparam name="TValue">Type of the value.</typeparam>
public sealed class NullDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
    /// <summary>
    ///     Gets the unique instance of <see cref="NullDictionary{TKey, TValue}"/>.
    /// </summary>
    public static IDictionary<TKey, TValue> Instance { get; } = new NullDictionary<TKey, TValue>();

    /// <inheritdoc />
    public int Count => 0;

    /// <inheritdoc />
    public bool IsReadOnly => true;

    /// <inheritdoc />
    public TValue this[TKey key] {
        get => default!;
        set => _ = value;
    }

    /// <inheritdoc />
    public ICollection<TKey> Keys => [];

    /// <inheritdoc />
    public ICollection<TValue> Values => [];

    // Explicit static constructor to tell the C# compiler
    // not to mark type as beforefieldinit
    static NullDictionary() { }

    private NullDictionary() { }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item) { }

    /// <inheritdoc />
    public void Clear() { }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item) {
        return false;
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item) {
        return false;
    }

    /// <inheritdoc />
    public void Add(TKey key, TValue value) { }

    /// <inheritdoc />
    public bool ContainsKey(TKey key) {
        return false;
    }

    /// <inheritdoc />
    public bool Remove(TKey key) {
        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue(TKey key, [MaybeNullWhen(returnValue: false)] out TValue value) {
        value = default;

        return false;
    }
}