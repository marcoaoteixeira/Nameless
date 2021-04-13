using Nameless.Persistence.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class UserLoginClassMapping : ClassMapping<UserLogin> {
        #region Public Constructors

        public UserLoginClassMapping () {
            const string UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY = nameof (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);

            Table ("user_logins");

            ComposedId (map => {
                map.Property (prop => prop.UserId, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.Guid);
                    mapping.UniqueKey (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);
                });

                map.Property (prop => prop.LoginProvider, mapping => {
                    mapping.Column ("login_provider");
                    mapping.NotNullable (true);
                    mapping.Length (256);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERLOGINS_USERID_LOGINPROVIDER_PROVIDERKEY);
                });

                map.Property (prop => prop.ProviderKey, mapping => {
                    mapping.Column ("provider_key");
                    mapping.NotNullable (true);
                    mapping.Length (256);
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