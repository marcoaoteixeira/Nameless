using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.CQRS;
using Nameless.Skeleton.Web.Commands.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Account {

    [AllowAnonymous]
    public sealed class RegisterPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly ICommandQueryDispatcher _dispatcher;

        #endregion

        #region Public Properties

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        #endregion

        #region Public Constructors

        public RegisterPageModel (ICommandQueryDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        public IActionResult OnGet (string returnUrl = null) {
            ReturnUrl = returnUrl;

            return Page ();
        }

        public async Task<IActionResult> OnPostAsync (string returnUrl = null) {
            returnUrl = returnUrl ?? Url.Content ("~/");

            if (!ModelState.IsValid) { return Page (); }

            var result = await _dispatcher.CommandAsync (new RegisterNewUserCommand {
                UserName = Input.UserName,
                Email = Input.Email,
                Password = Input.Password
            });

            if (result.Succeeded) {
                var callbackUrl = Url.Page (
                    "/Account/ConfirmEmail",
                    pageHandler : null,
                    values : new {
                        userId = result.State.UserId,
                        code = result.State.EmailConfirmationToken
                    },
                    protocol : Request.Scheme
                );

                await _dispatcher.CommandAsync (new SendRegistrationEmailCommand {
                    UserId = result.State.UserId,
                    UserName = Input.UserName,
                    Email = Input.Email,
                    CallbackUrl = callbackUrl
                });

                return LocalRedirect (returnUrl);
            }

            var errors = result.ErrorMessage.Split (";");
            foreach (var error in errors) {
                ModelState.AddModelError (string.Empty, error);
            }

            return RedirectToPage ();
        }

        #endregion

        #region Public Classes

        public sealed class RegisterInputModel {
            #region Public Properties

            [Required]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType (DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType (DataType.Password)]
            [Compare (nameof (Password))]
            public string ConfirmPassword { get; set; }

            public bool TermsAgreement { get; set; }

            #endregion
        }

        #endregion
    }
}