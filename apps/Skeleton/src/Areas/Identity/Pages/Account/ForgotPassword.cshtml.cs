using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.CQRS;
using Nameless.Skeleton.Web.Commands.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Account {

    [AllowAnonymous]
    public sealed class ForgotPasswordPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly ICommandQueryDispatcher _dispatcher;

        #endregion

        #region Public Properties

        [BindProperty]
        public ForgotPasswordInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        #region Public Constructors

        public ForgotPasswordPageModel (ICommandQueryDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        public IActionResult OnGet (string returnUrl = null) {
            ReturnUrl = returnUrl;

            return Page ();
        }

        public async Task<IActionResult> OnPostAsync () {
            if (!ModelState.IsValid) { return Page (); }

            var resetPasswordResult = await _dispatcher.CommandAsync (new GenerateResetPasswordEmailTokenCommand {
                Email = Input.Email
            });

            if (resetPasswordResult.Succeeded) {
                var callbackUrl = Url.Page (
                    "/Account/ResetPassword",
                    pageHandler : null,
                    values : new { code = resetPasswordResult.State.EmailResetCodeToken },
                    protocol : Request.Scheme
                );

                await _dispatcher.CommandAsync (new SendResetPasswordEmailCommand {
                    Email = Input.Email,
                    CallbackUrl = callbackUrl
                });
            }

            return RedirectToPage ("./ForgotPasswordConfirmation");
        }

        #endregion

        #region Public Classes

        public sealed class ForgotPasswordInputModel {
            #region Public Properties

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            #endregion
        }

        #endregion
    }
}