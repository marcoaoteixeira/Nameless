using System;
using NHibernate.Mapping.ByCode;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Universally unique identifier implementation for <see cref="IGeneratorDef"/>.
    /// </summary>
    public sealed class UUIDHexCombGeneratorDef : IGeneratorDef {
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
        public string Class {
            get { return "uuid.hex"; }
        }

        /// <inheritdoc />
        public object Params { get; }

        /// <inheritdoc />
        public Type DefaultReturnType {
            get { return typeof (string); }
        }

        /// <inheritdoc />
        public bool SupportedAsCollectionElementId {
            get { return false; }
        }

        #endregion
    }
}