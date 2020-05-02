using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.Orm.NHibernate {
    /// <summary>
    /// Abstract implementation of <see cref="ClassMapping{T}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    public abstract class EntityClassMappingBase<TEntity> : ClassMapping<TEntity>
        where TEntity : EntityBase {

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="EntityClassMappingBase{TEntity}"/>.
        /// </summary>
        /// <param name="tableName">The name of the DB table.</param>
        /// <param name="keyName">The name of the DB table primary key.</param>
        protected EntityClassMappingBase (string tableName, string keyName) {
            Table (tableName);

            Id (property => property.ID
                , mapping => {
                    mapping.Access (Accessor.Field);
                    mapping.Column (keyName);
                    mapping.Generator (Generators.Guid);
                    mapping.Type (Identifiers.Guid);
                });

            Property (property => property.CreationDate
                , mapping => {
                    mapping.Access (Accessor.Field);
                    mapping.Column ("creation_date");
                });

            Property (property => property.ModificationDate
                , mapping => {
                    mapping.Access (Accessor.Field);
                    mapping.Column ("modification_date");
                });

            Property (property => property.Owner
                , mapping => {
                    mapping.Access (Accessor.Field);
                    mapping.Column ("owner");
                    mapping.Type (Identifiers.Guid);
                });

            Filter (EntityOwnerTrimmingFilterDefinitionPolicy.FilterName, filterMapping => {
                filterMapping.Condition ($"owner = :{EntityOwnerTrimmingFilterDefinitionPolicy.OwnerParameterName}");
            });
        }

        #endregion
    }
}