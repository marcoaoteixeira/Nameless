using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;
using Nameless.AspNetCore.Identity;
using Nameless.Skeleton.Web.Areas.Identity.Api.v1.Models.Administration;

namespace Nameless.Skeleton.Web.Areas.Identity.Api.v1.Controllers {

    [Area ("Identity")]
    [ApiVersion ("1")]
    [Route ("[area]/api/v{version:apiVersion}/[controller]")]
    public sealed class AdministrationController : WebApiController {
        #region Private Read-Only Fields

        private readonly IQueryableUserStore<User> _queryableUserStore;

        #endregion

        #region Public Constructors

        public AdministrationController (IQueryableUserStore<User> queryableUserStore) {
            Prevent.ParameterNull (queryableUserStore, nameof (queryableUserStore));

            _queryableUserStore = queryableUserStore;
        }

        #endregion

        #region Public Methods

        [HttpGet]
        [Route ("ListUsers")]
        public IActionResult ListUsers () {
            var users = _queryableUserStore.Users.ToArray ();

            var model = users.Select (_ => new UserModel {
                ID = _.Id,
                FullName = _.UserName,
                Email = _.Email,
                EmailConfirmed = _.EmailConfirmed,
                IsLocked = _.LockoutEnabled,
                LockoutEndDate = _.LockoutEnd.GetValueOrDefault (),
                AccessFailedCount = _.AccessFailedCount,
                TwoFactorEnabled = _.TwoFactorEnabled
            });

            return Ok (model);
        }

        #endregion
    }
}