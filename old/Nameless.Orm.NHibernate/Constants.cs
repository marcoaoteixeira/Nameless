using NHibernate.Type;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Identifier types
    /// </summary>
    public static class Identifiers {

        #region Public Static Read-Only Fields

        /// <summary>
        /// Int64 identifier type.
        /// </summary>
        public static readonly IIdentifierType Int64 = new Int64Type ();

        /// <summary>
        /// Guid identifier type.
        /// </summary>
        public static readonly IIdentifierType Guid = new GuidType ();

        #endregion
    }
}