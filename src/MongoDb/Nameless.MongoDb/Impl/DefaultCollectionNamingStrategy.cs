using System.Reflection;
using CoreRoot = Nameless.Root;

namespace Nameless.MongoDB.Impl {
    public sealed class DefaultCollectionNamingStrategy : ICollectionNamingStrategy {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="DefaultCollectionNamingStrategy"/>.
        /// </summary>
        public static ICollectionNamingStrategy Instance { get; } = new DefaultCollectionNamingStrategy();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static DefaultCollectionNamingStrategy() { }

        #endregion

        #region Private Constructors

        // Prevents the class from being constructed.
        private DefaultCollectionNamingStrategy() { }

        #endregion

        #region ICollectionNamingStrategy Members

        public string? GetCollectionName(Type? type) {
            Guard.Against.Null(type, nameof(type));

            var attr = type.GetCustomAttribute<CollectionNameAttribute>(inherit: false);
            if (attr is not null && !string.IsNullOrWhiteSpace(attr.Name)) { return attr.Name; }

            return type.FullName!
                    .Replace(CoreRoot.Separators.DOT.ToString(), string.Empty)
                    .CamelFriendly()
                    .Replace(CoreRoot.Separators.SPACE, CoreRoot.Separators.UNDERSCORE)
                    .ToLower();
        }

        #endregion
    }
}
