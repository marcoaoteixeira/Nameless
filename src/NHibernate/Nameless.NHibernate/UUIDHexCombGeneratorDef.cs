using NHibernate.Mapping.ByCode;

namespace Nameless.NHibernate {
    /// <summary>
    /// Singleton Pattern implementation for <see cref="UUIDHexCombGeneratorDef" />. (see: https://en.wikipedia.org/wiki/Singleton_pattern)
    /// </summary>
    [Singleton]
    public sealed class UUIDHexCombGeneratorDef : IGeneratorDef {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of <see cref="UUIDHexCombGeneratorDef" />.
        /// </summary>
        public static IGeneratorDef Instance { get; } = new UUIDHexCombGeneratorDef();

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static UUIDHexCombGeneratorDef() { }

        #endregion

        #region Private Constructors

        private UUIDHexCombGeneratorDef() { }

        #endregion

        #region IGeneratorDef Members

        /// <inheritdoc />
        public string Class => "uuid.hex";

        /// <inheritdoc />
        public object Params => new { format = "D" };

        /// <inheritdoc />
        public Type DefaultReturnType => typeof(string);

        /// <inheritdoc />
        public bool SupportedAsCollectionElementId => false;

        #endregion
    }
}
