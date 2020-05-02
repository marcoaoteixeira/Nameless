using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Nameless.WebApplication.Identity {
    public partial class UserStore : IQueryableUserStore<User> {
        #region IQueryableUserStore<User> Members

        public IQueryable<User> Users =>
            throw new System.NotImplementedException ();

        #endregion
    }
}