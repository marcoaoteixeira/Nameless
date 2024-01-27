using System.Collections;
using System.Dynamic;

namespace Nameless.Collections {
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
        #region Private Read-Only Fields

        private readonly IDictionary _dictionary;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="HashtableDynamicObject"/>.
        /// </summary>
        /// <param name="dictionary">An instance of <see cref="IDictionary"/></param>
        public HashtableDynamicObject(IDictionary? dictionary = default) {
            _dictionary = dictionary ?? new Hashtable();
        }

        #endregion

        #region Public Override Methods

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object? result) {
            Guard.Against.Null(binder, nameof(binder));

            result = _dictionary[binder.Name];
            return _dictionary.Contains(binder.Name);
        }

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object? value) {
            Guard.Against.Null(binder, nameof(binder));

            _dictionary[binder.Name] = value;

            return true;
        }

        /// <inheritdoc/>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result) {
            Guard.Against.Null(binder, nameof(binder));
            Guard.Against.Null(indexes, nameof(indexes));

            if (indexes.Length != 1) {
                throw new ArgumentException("Only support a single indexer parameter", nameof(indexes));
            }

            result = _dictionary[indexes[0]];
            return _dictionary.Contains(indexes[0]);
        }

        /// <inheritdoc/>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value) {
            Guard.Against.Null(binder, nameof(binder));
            Guard.Against.Null(indexes, nameof(indexes));

            if (indexes.Length != 1) {
                throw new ArgumentException("Only support a single indexer parameter", nameof(indexes));
            }

            _dictionary[indexes[0]] = value;

            return true;
        }

        #endregion
    }
}