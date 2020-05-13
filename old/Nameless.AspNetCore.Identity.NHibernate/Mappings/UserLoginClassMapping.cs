using Nameless.NHibernate;
using NHibernate;

namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings {
    public sealed class UserLoginClassMapping : ClassMappingBase<UserLogin> {
        #region Public Constructors

        public UserLoginClassMapping (ClassMappingOptions options = null) : base ("user_logins", options) {
            const string UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY = nameof (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);

            ComposedId (map => {
                map.Property (prop => prop.UserID, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);
                });

                map.Property (prop => prop.LoginProvider, mapping => {
                    mapping.Column ("login_provider");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);
                });

                map.Property (prop => prop.ProviderKey, mapping => {
                    mapping.Column ("provider_key");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);
                });
            });

            Property (prop => prop.ProviderDisplayName, mapping => {
                mapping.Column ("provider_display_name");
                mapping.Length (512);
                mapping.NotNullable (false);
                mapping.Type (NHibernateUtil.String);
            });
        }

        #endregion
    }
}