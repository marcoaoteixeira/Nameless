﻿using System.Reflection;

namespace Nameless.NoSQL.MongoDb {

    public sealed class CollectionNamingStrategy : ICollectionNamingStrategy {

        #region Private Static Read-Only Fields

        private static readonly ICollectionNamingStrategy _instance = new CollectionNamingStrategy();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="CollectionNamingStrategy"/>.
        /// </summary>
        public static ICollectionNamingStrategy Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static CollectionNamingStrategy() { }

        #endregion

        #region Private Constructors

        // Prevents the class from being constructed.
        private CollectionNamingStrategy() { }

        #endregion

        #region ICollectionNamingStrategy Members

        public string? GetCollectionName(Type? type) {
            if (type == default) { throw new ArgumentNullException(nameof(type)); }

            var attr = type.GetCustomAttribute<CollectionNameAttribute>(inherit: false);
            if (attr != default && !string.IsNullOrWhiteSpace(attr.Name)) { return attr.Name; }

            return type.FullName!
                    .Replace(".", string.Empty)
                    .CamelFriendly()
                    .Replace(' ', '_')
                    .ToLower();
        }

        #endregion
    }
}
