using Nameless.AspNetCore.Identity.Stores.NHibernate.Models;
using Nameless.NHibernate;

namespace Nameless.AspNetCore.Identity.Stores.NHibernate.Mappings {
    public sealed class UserInRoleClassMapping : ClassMappingBase<UserRole> {
        #region Public Constructors

        public UserInRoleClassMapping (ClassMappingOptions options = null) : base ("users_in_roles", options) {
            const string UQ_USERINROLES_USERID_ROLEID = nameof (UQ_USERINROLES_USERID_ROLEID);

            ComposedId (map => {
                map.Property (prop => prop.UserID, mapping => {
                    mapping.Column ("user_id");
                    mapping.NotNullable (true);
                    mapping.Type<UUIDColumnToStringPropertyUserType> ();
                    mapping.UniqueKey (UQ_USERINROLES_USERID_ROLEID);
                });

                map.Property (prop => prop.RoleID, mapping => {
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