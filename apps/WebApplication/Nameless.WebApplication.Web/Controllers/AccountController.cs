using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;
using Nameless.AspNetCore.Identity;
using Nameless.WebApplication.Web.Models.Account;

namespace Nameless.WebApplication.Web.Controllers {

    public sealed class AccountController : MvcController {
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
            return View (new SignInViewModel {
                ReturnUrl = returnUrl ?? Url.Content ("~/")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn (SignInViewModel model) {
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync (model.Email, model.Password, model.RememberMe, lockoutOnFailure : false);

                if (result.Succeeded) {
                    return Redirect (model.ReturnUrl);
                }

                if (result.RequiresTwoFactor) {
                    return View ("SignInWithTwoFactor", new { model.ReturnUrl, model.RememberMe });
                }

                if (result.IsLockedOut) {
                    return View ("Lockout");
                }

                if (result.IsNotAllowed) {
                    ModelState.AddModelError (string.Empty, T["Invalid login attempt."]);
                    return View ();
                }
            }

            return View ();
        }

        [HttpGet]
        public async Task<IActionResult> SignOut (string returnUrl = null) {
            await _signInManager.SignOutAsync ();

            return Redirect (returnUrl ?? Url.Content ("~/"));
        }

        [HttpGet]
        public IActionResult Register () {
            return View (new RegisterViewModel());
        }

        #endregion
    }
}