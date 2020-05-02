using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Nameless.AspNetCore.Identity.Stores {
    public partial class UserStore : IQueryableUserStore<User> {
        #region IQueryableUserStore<User> Members

        public IQueryable<User> Users => Session.Query<User> ();

        #endregion
    }
}