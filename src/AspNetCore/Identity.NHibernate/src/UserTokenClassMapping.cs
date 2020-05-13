using Nameless.Persistence.NHibernate;
using NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class UserTokenClassMapping : ClassMapping<UserToken> {
        #region Public Constructors

        public UserTokenClassMapping () {
            const string UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME = nameof (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);

            Table ("user_tokens");

            ComposedId (map => {
                map.Property (prop => prop.UserId, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);
                });

                map.Property (prop => prop.LoginProvider, mapping => {
                    mapping.Column ("login_provider");
                    mapping.NotNullable (true);
                    mapping.Length (256);
                    mapping.Type (NHibernateUtil.String);
                    mapping.UniqueKey (UQ_USERTOKENS_USERID_LOGINPROVIDER_NAME);
                });

                map.Property (prop => prop.Name, mapping => {
                    mapping.Column ("name");
                    mapping.NotNullable (true);
                    mapping.Length (256);
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