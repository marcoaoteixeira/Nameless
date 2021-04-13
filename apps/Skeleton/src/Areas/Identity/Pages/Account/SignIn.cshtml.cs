using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.CQRS;
using Nameless.Skeleton.Web.Commands.Identity;
using Nameless.Skeleton.Web.Queries.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Account {

    [AllowAnonymous]
    public sealed class SignInPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly ICommandQueryDispatcher _dispatcher;

        #endregion

        #region Public Properties

        [BindProperty]
        public SignInInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<AuthenticationScheme> ExternalSignIns { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        #endregion

        #region Public Constructors

        public SignInPageModel (ICommandQueryDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        public async Task OnGetAsync (string returnUrl = null) {
            if (!string.IsNullOrEmpty (ErrorMessage)) {
                ModelState.AddModelError (string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content ("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync (IdentityConstants.ExternalScheme);

            ExternalSignIns = await _dispatcher.QueryAsync (new GetExternalAuthenticationSchemesQuery ());

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync (string returnUrl = null) {
            returnUrl = returnUrl ?? Url.Content ("~/");

            if (!ModelState.IsValid) { return Page (); }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var result = await _dispatcher.CommandAsync (new SignInCommand {
                Email = Input.Email,
                Password = Input.Password,
                RememberMe = Input.RememberMe
            });

            if (result.State.Succeeded && !result.State.RequiresTwoFactor) {
                return LocalRedirect (returnUrl);
            }

            if (result.State.RequiresTwoFactor) {
                return RedirectToPage ("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
            }

            ModelState.AddModelError (string.Empty, result.ErrorMessage);

            if (result.State.IsLockedOut) {
                return RedirectToPage ("./Lockout");
            }

            return Page ();
        }

        #endregion

        #region Public Classes

        public sealed class SignInInputModel {
            #region Public Properties

            [Display (Description = "E-mail")]
            [Required]
            public string Email { get; set; }

            [Display (Description = "Password")]
            [Required]
            [DataType (DataType.Password)]
            public string Password { get; set; }

            [Display (Description = "Remember me?")]
            public bool RememberMe { get; set; }

            #endregion
        }

        #endregion
    }
}