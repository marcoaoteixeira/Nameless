
using Nameless.Persistence.NHibernate;
using NHibernate.Mapping.ByCode.Conformist;

namespace Nameless.AspNetCore.Identity.NHibernate {
    public sealed class UserRoleClassMapping : ClassMapping<UserRole> {
        #region Public Constructors

        public UserRoleClassMapping () {
            const string UQ_USERINROLES_USERID_ROLEID = nameof (UQ_USERINROLES_USERID_ROLEID);

            Table ("users_in_roles");

            ComposedId (map => {
                map.Property (prop => prop.UserId, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERINROLES_USERID_ROLEID);
                });

                map.Property (prop => prop.RoleId, mapping => {
                    mapping.Column ("role_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERINROLES_USERID_ROLEID);
                });
            });
        }

        #endregion
    }
}