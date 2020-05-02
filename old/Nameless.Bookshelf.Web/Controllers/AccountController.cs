using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nameless.Bookshelf.Identity;
using Nameless.Bookshelf.Web.Models;

namespace Nameless.Bookshelf.Web.Controllers {
    public class AccountController : Controller {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public AccountController (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult SignIn (string returnUrl = null) {
            return View (returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> SignIn (SignInViewModel model) {
            if (!ModelState.IsValid) {
                return View ();
            }

            var signInResult = await _signInManager.PasswordSignInAsync (
                userName: model.Email,
                password: model.Password,
                isPersistent: model.RememberMe.GetValueOrDefault (),
                lockoutOnFailure: false
            );

            if (!signInResult.Succeeded) {
                ModelState.AddModelError (string.Empty, "Invalid login attempt.");
                return View ();
            }

            return Redirect (model.ReturnUrl);
        }

        #endregion
    }
}