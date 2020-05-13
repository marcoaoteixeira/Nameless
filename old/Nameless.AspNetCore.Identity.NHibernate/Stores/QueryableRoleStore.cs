using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class RoleStore : IQueryableRoleStore<Role> {
        #region IQueryableRoleStore<Role> Members

        public IQueryable<Role> Roles => Session.Query<Role> ();

        #endregion
    }
}