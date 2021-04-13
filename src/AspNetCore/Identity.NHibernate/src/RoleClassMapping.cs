using Nameless.Persistence.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class RoleClassMapping : ClassMapping<Role> {
        #region Public Constructors

        public RoleClassMapping () {
            Table ("roles");

            Id (prop => prop.Id, mapper => {
                mapper.Column ("id");
                mapper.Type (NHibernateUtil.Guid);
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