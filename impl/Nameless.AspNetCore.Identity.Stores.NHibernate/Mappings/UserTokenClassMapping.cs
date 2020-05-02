using Nameless.NHibernate;
using NHibernate;

namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings {
    public sealed class UserTokenClassMapping : ClassMappingBase<UserToken> {
        #region Public Constructors

        public UserTokenClassMapping (ClassMappingOptions options = null) : base ("user_tokens", options) {
            const string UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME = nameof (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);

            ComposedId (map => {
                map.Property (prop => prop.UserID, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);
                });

                map.Property (prop => prop.LoginProvider, mapping => {
                    mapping.Column ("login_provider");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);
                });

                map.Property (prop => prop.Name, mapping => {
                    mapping.Column ("name");
                    mapping.NotNullable (true);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);
                });
            });

            Property (prop => prop.Value, mapping => {
                mapping.Column ("value");
                mapping.Length (2048);
                mapping.NotNullable (true);
                mapping.Type (NHibernateUtil.String);
            });
        }

        #endregion
    }
}