using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.AspNetCore.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Management {

    [AllowAnonymous]
    public sealed class UserPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly IUserStore<User> _userStore;

        #endregion

        #region Public Properties

        [BindProperty]
        public UserInputModel Input { get; set; }
            
        #endregion

        #region Public Constructors

        public UserPageModel (IUserStore<User> userStore) {
            Prevent.ParameterNull (userStore, nameof (userStore));

            _userStore = userStore;
        }

        #endregion

        #region Public Methods

        public async Task<IActionResult> OnGetAsync (Guid id) {
            var user = await _userStore.FindByIdAsync (id.ToString (), CancellationToken.None);

            if (user == null) {
                return NotFound();
            }

            Input = new UserInputModel {
                ID = user.Id,
                FullName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                TwoFactorEnabled = user.TwoFactorEnabled
            };

            return Page ();
        }

        #endregion

        #region Public Classes

        public sealed class UserInputModel {
            #region Public Properties

            public Guid ID { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool TwoFactorEnabled { get; set; }
            
            #endregion
        }
            
        #endregion
    }
}