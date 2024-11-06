using System.Collections;
using System.Dynamic;

namespace Nameless.NHibernate;

/// <summary>
/// Class to add support for dynamic fields with NHibernate.
/// </summary>
/// <remarks>https://ayende.com/blog/4776/support-dynamic-fields-with-nhibernate-and-net-4-0</remarks>
/// <example>
/// public class Entity {
///     private readonly IDictionary _attributes; // This will be our access field for NHibernate
///     private readonly HashtableDynamicObject _proxy;
///     public virtual dynamic Attributes => _proxy;
///     public Entity() {
///          _attributes = new Hashtable();
///          _proxy = new HashtableDynamicObject(_attributes);
///     }
/// }
/// </example>
public sealed class HashtableDynamicObject : DynamicObject {
    private readonly IDictionary _storage;

    /// <summary>
    /// Initializes a new instance of <see cref="HashtableDynamicObject"/>.
    /// </summary>
    /// <param name="dictionary">An instance of <see cref="IDictionary"/></param>
    public HashtableDynamicObject(IDictionary? dictionary = null) {
        _storage = dictionary ?? new Hashtable();
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="binder"/> is <c>null</c>.
    /// </exception>
    public override bool TryGetMember(GetMemberBinder binder, out object? result) {
        Prevent.Argument.Null(binder);

        result = _storage[binder.Name];
        return _storage.Contains(binder.Name);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="binder"/> is <c>null</c>.
    /// </exception>
    public override bool TrySetMember(SetMemberBinder binder, object? value) {
        Prevent.Argument.Null(binder);

        _storage[binder.Name] = value;

        return true;
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="binder"/> or
    /// <paramref name="indexes"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="indexes"/> contains 0 (zero) or more than one element.
    /// </exception>
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result) {
        Prevent.Argument.Null(binder);
        Prevent.Argument.Null(indexes);

        if (indexes.Length != 1) {
            throw new ArgumentException("Only support a single indexer parameter", nameof(indexes));
        }

        result = _storage[indexes[0]];
        return _storage.Contains(indexes[0]);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="binder"/> or
    /// <paramref name="indexes"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="indexes"/> contains 0 (zero) or more than one element.
    /// </exception>
    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) {
        Prevent.Argument.Null(binder);
        Prevent.Argument.Null(indexes);

        if (indexes.Length != 1) {
            throw new ArgumentException("Only support a single indexer parameter", nameof(indexes));
        }

        _storage[indexes[0]] = value;

        return true;
    }
}