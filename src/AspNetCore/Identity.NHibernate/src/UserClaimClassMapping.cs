using Nameless.Persistence.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class UserClaimClassMapping : ClassMapping<UserClaim> {
        #region Public Constructors

        public UserClaimClassMapping () {
            const string UQ_USERCLAIMS_USERID_TYPE = nameof (UQ_USERCLAIMS_USERID_TYPE);

            Table ("user_claims");

            ComposedId (map => {
                map.Property (prop => prop.UserId, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERCLAIMS_USERID_TYPE);
                });

                map.Property (prop => prop.ClaimType, mapping => {
                    mapping.Column ("claim_type");
                    mapping.NotNullable (true);
                    mapping.Length (256);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERCLAIMS_USERID_TYPE);
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