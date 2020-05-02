using Nameless.NHibernate;
using NHibernate;

namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings {
    public sealed class UserClaimClassMapping : ClassMappingBase<UserClaim> {
        #region Public Constructors

        public UserClaimClassMapping (ClassMappingOptions options = null) : base ("user_claims", options) {
            const string UQ_USERCLAIMS_USERID_TYPE = nameof (UQ_USERCLAIMS_USERID_TYPE);
            
            ComposedId (map => {
                map.Property (prop => prop.UserID, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERCLAIMS_USERID_TYPE);
                });

                map.Property (prop => prop.Type, mapping => {
                    mapping.Column ("type");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERCLAIMS_USERID_TYPE);
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