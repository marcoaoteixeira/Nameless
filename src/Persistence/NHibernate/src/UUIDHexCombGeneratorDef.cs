using System;

namespace Nameless.Persistence.NHibernate {
    /// <summary>
    /// Universally unique identifier implementation for <see cref="IGeneratorDef"/>.
    /// </summary>
    public sealed class UUIDHexCombGeneratorDef : global::NHibernate.Mapping.ByCode.IGeneratorDef {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="UUIDHexCombGeneratorDef"/>.
        /// </summary>
        /// <param name="format">The UUID format.</param>
        public UUIDHexCombGeneratorDef (string format) {
            Prevent.ParameterNull (format, nameof (format));

            Params = new { format };
        }

        #endregion

        #region IGeneratorDef Members

        /// <inheritdoc />
        public string Class => "uuid.hex";

        /// <inheritdoc />
        public object Params { get; }

        /// <inheritdoc />
        public Type DefaultReturnType => typeof (string);

        /// <inheritdoc />
        public bool SupportedAsCollectionElementId => false;

        #endregion
    }
}