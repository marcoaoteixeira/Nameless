using NHibernate.Engine;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Defines a NHibernate pre-filter.
    /// </summary>
    public interface IFilterDefinitionPolicy {

        #region Methods

        /// <summary>
        /// Retrieves the NHibernate filter definition.
        /// </summary>
        /// <returns>An instance of <see cref="FilterDefinition"/>.</returns>
        FilterDefinition GetPolicy ();

        #endregion
    }
}