﻿using System;
using System.Collections;
using System.Dynamic;

namespace Nameless.Dynamic {

    /// <summary>
    /// Represents a hash table implementation of <see cref="DynamicObject"/>
    /// </summary>
    /// <remarks>https://ayende.com/blog/4776/support-dynamic-fields-with-nhibernate-and-net-4-0</remarks>
    public sealed class HashtableDynamicObject : DynamicObject {

        #region Private Read-Only Fields

        private readonly IDictionary _dictionary;

        #endregion Private Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="HashtableDynamicObject"/>.
        /// </summary>
        /// <param name="dictionary">An instance of <see cref="IDictionary"/></param>
        public HashtableDynamicObject(IDictionary dictionary = null) {
            _dictionary = dictionary ?? new Hashtable();
        }

        #endregion Public Constructors

        #region Public Override Methods

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            result = _dictionary[binder.Name];
            return _dictionary.Contains(binder.Name);
        }

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            _dictionary[binder.Name] = value;
            return true;
        }

        /// <inheritdoc/>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
            Prevent.ParameterNull(indexes, nameof(indexes));

            if (indexes.Length != 1) {
                throw new ArgumentException("Only support a single indexer parameter", "indexes");
            }

            result = _dictionary[indexes[0]];
            return _dictionary.Contains(indexes[0]);
        }

        /// <inheritdoc/>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value) {
            Prevent.ParameterNull(indexes, nameof(indexes));

            if (indexes.Length != 1) {
                throw new ArgumentException("Only support a single indexer parameter", "indexes");
            }

            _dictionary[indexes[0]] = value;
            return true;
        }

        #endregion Public Override Methods
    }
}