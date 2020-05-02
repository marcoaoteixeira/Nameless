using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Type;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Application tenant implementation of <see cref="IFilterDefinitionPolicy"/>
    /// </summary>
    public class EntityOwnerTrimmingFilterDefinitionPolicy : IFilterDefinitionPolicy {

        #region Public Static Read-Only Fields

        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        public static readonly string FilterName = "EntityOwnerTrimming";

        /// <summary>
        /// Gets the owner ID parameter name.
        /// </summary>
        public static readonly string OwnerParameterName = "owner";

        #endregion

        #region IFilterDefinitionUnit Members

        /// <inheritdoc />
        public FilterDefinition GetPolicy () {
            return new FilterDefinition (
                name: FilterName,
                defaultCondition: null,
                parameterTypes: new Dictionary<string, IType> {
                    { OwnerParameterName, Identifiers.Guid }
                },
                useManyToOne: false);
        }

        #endregion
    }
}