using Nameless.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode;

namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings {
    public sealed class RoleClassMapping : ClassMappingBase<Role> {
        #region Public Constructors

        public RoleClassMapping (ClassMappingOptions options = null) : base ("roles", options) {

            Id (prop => prop.ID, mapper => {
                mapper.Column ("id");
                mapper.Type<UUIDColumnToStringPropertyUserType> ();
                mapper.Generator (Generators.Assigned);
            });

            Property (prop => prop.Name, mapping => {
                mapping.Column ("name");
                mapping.Index ("idx_roles_name");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
                mapping.Unique (true);
            });

            Property (prop => prop.NormalizedName, mapping => {
                mapping.Column ("normalized_name");
                mapping.Index ("idx_roles_normalized_name");
                mapping.Length (512);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
            });
        }

        #endregion
    }
}