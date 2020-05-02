using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity {
    public partial class RoleStore : IQueryableRoleStore<Role> {
        #region IQueryableRoleStore<Role> Members

        public IQueryable<Role> Roles =>
            throw new System.NotImplementedException ();

        #endregion
    }
}