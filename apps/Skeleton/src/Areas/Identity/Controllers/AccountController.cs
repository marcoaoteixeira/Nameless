using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Nameless.AspNetCore;
using Nameless.AspNetCore.Identity;
using Nameless.Skeleton.Web.Areas.Identity.Models;

namespace Nameless.Skeleton.Web.Areas.Identity.Controllers {
    [Area (Constants.IDENTITY_AREA_NAME)]
    public sealed class AccountController : MvcController {
        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _userEmailStore;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailSender;

        #endregion

        #region Public Constructors

        public AccountController (UserManager<User> userManager, IUserStore<User> userStore, IUserEmailStore<User> userEmailStore, SignInManager<User> signInManager) {
            Prevent.ParameterNull (userManager, nameof (userManager));
            Prevent.ParameterNull (userStore, nameof (userStore));
            Prevent.ParameterNull (userEmailStore, nameof (userEmailStore));
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _userManager = userManager;
            _userStore = userStore;
            _userEmailStore = userEmailStore;
            _signInManager = signInManager;
        }

        #endregion

        #region Public Methods

        [HttpGet]
        public IActionResult Register (string returnUrl = null) {
            return View (new RegisterViewModel {
                ReturnUrl = returnUrl ?? Url.Content ("~/")
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register (RegisterViewModel model) {
            var user = new User ();

            await _userStore.SetUserNameAsync (user, model.UserName, CancellationToken.None);
            await _userEmailStore.SetEmailAsync (user, model.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync (user, model.Password);

            // Fail fast
            if (!result.Succeeded) {
                foreach (var error in result.Errors) {
                    ModelState.AddModelError (string.Empty, error.Description);
                }
                return View ();
            }

            await SendConfirmationEmailAsync (user, model.ReturnUrl);

            return View ();
        }

        [HttpGet]
        public IActionResult ConfirmRegistration () {
            return View ();
        }

        #endregion

        #region Private Methods

        private async Task SendConfirmationEmailAsync (User user, string returnUrl) {
            var userId = await _userManager.GetUserIdAsync (user);
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync (user);
            var encodedConfirmationToken = WebEncoders.Base64UrlEncode (Encoding.UTF8.GetBytes (confirmationToken));

            var callbackUrl = Url.Action (nameof (ConfirmRegistration), ControllerHelper.GetControllerName (this), new {
                area = Constants.IDENTITY_AREA_NAME,
                userId,
                code = encodedConfirmationToken,
                returnUrl
            }, Request.Protocol);

        }

        #endregion
    }
}