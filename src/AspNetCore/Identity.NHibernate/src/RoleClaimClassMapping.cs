using Nameless.Persistence.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class RoleClaimClassMapping : ClassMapping<RoleClaim> {
        #region Public Constructors

        public RoleClaimClassMapping () {
            const string UQ_ROLECLAIMS_ROLEID_TYPE = nameof (UQ_ROLECLAIMS_ROLEID_TYPE);

            Table ("role_claims");

            ComposedId (map => {
                map.Property (prop => prop.RoleId, mapping => {
                    mapping.Column ("role_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_ROLECLAIMS_ROLEID_TYPE);
                });

                map.Property (prop => prop.ClaimType, mapping => {
                    mapping.Column ("claim_type");
                    mapping.NotNullable (true);
                    mapping.Length (256);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_ROLECLAIMS_ROLEID_TYPE);
                });
            });

            Property (prop => prop.ClaimValue, mapping => {
                mapping.Column ("claim_value");
                mapping.Length (2048);
                mapping.NotNullable (false);
                mapping.Type (NHibernateUtil.String);
            });
        }

        #endregion
    }
}