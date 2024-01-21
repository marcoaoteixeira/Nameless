using System.Reflection;
using CoreRoot = Nameless.Root;

namespace Nameless.MongoDB.Impl {
    public sealed class CollectionNamingStrategy : ICollectionNamingStrategy {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="CollectionNamingStrategy"/>.
        /// </summary>
        public static ICollectionNamingStrategy Instance { get; } = new CollectionNamingStrategy();

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

        public string GetCollectionName(Type type) {
            Guard.Against.Null(type, nameof(type));

            var attr = type.GetCustomAttribute<CollectionNameAttribute>(inherit: false);
            if (attr is not null) {
                return attr.Name;
            }

            return string.Join(
                separator: CoreRoot.Separators.UNDERSCORE,
                type.Name.SplitUpperCase()
            ).ToLower();
        }

        #endregion
    }
}
