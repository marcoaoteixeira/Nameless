using Nameless.NHibernate;
using NHibernate;

namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings {
    public sealed class RoleClaimClassMapping : ClassMappingBase<RoleClaim> {
        #region Public Constructors

        public RoleClaimClassMapping (ClassMappingOptions options = null) : base ("role_claims", options) {
            const string UQ_ROLECLAIMS_ROLEID_TYPE = nameof (UQ_ROLECLAIMS_ROLEID_TYPE);

            ComposedId (map => {
                map.Property (prop => prop.RoleID, mapping => {
                    mapping.Column ("role_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_ROLECLAIMS_ROLEID_TYPE);
                });

                map.Property (prop => prop.Type, mapping => {
                    mapping.Column ("type");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_ROLECLAIMS_ROLEID_TYPE);
                });
            });

            Property (prop => prop.Value, mapping => {
                mapping.Column ("value");
                mapping.Length (2048);
                mapping.NotNullable (false);
                mapping.Type (NHibernateUtil.String);
            });
        }

        #endregion
    }
}