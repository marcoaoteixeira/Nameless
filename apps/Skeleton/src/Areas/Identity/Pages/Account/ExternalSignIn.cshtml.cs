using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.CQRS;
using Nameless.Skeleton.Web.Commands.Identity;
using Nameless.Skeleton.Web.Queries.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Account {

    [AllowAnonymous]
    public sealed class ExternalSignInPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly ICommandQueryDispatcher _dispatcher;

        #endregion

        #region Public Properties

        [BindProperty]
        public InputModel Input { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        #endregion

        #region Public Constructors

        public ExternalSignInPageModel (ICommandQueryDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        public IActionResult OnGet () {
            return RedirectToPage ("./SignIn");
        }

        public async Task<IActionResult> OnPostAsync (string provider, string returnUrl = null) {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page ("./ExternalLogin", pageHandler: "Callback", values : new { returnUrl });
            var result = await _dispatcher.CommandAsync (new ConfigureExternalPropertiesCommand {
                Provider = provider,
                    RedirectUrl = redirectUrl
            });
            return new ChallengeResult (provider, result.State.Properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync (string returnUrl = null, string remoteError = null) {
            returnUrl = returnUrl ?? Url.Content ("~/");
            if (remoteError != null) {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage ("./SignIn", new { ReturnUrl = returnUrl });
            }

            var info = await _dispatcher.QueryAsync (new GetExternalLoginInfoQuery ());
            if (info == null) {
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage ("./SignIn", new { ReturnUrl = returnUrl });
            }

            var signInResult = await _dispatcher.CommandAsync (new DoExternalLoginSignInCommand {
                LoginProvider = info.LoginProvider,
                ProviderKey = info.ProviderKey
            });

            if (signInResult.State.Success) {
                return LocalRedirect (returnUrl);
            }

            if (signInResult.State.IsLockedOut) {
                return RedirectToPage ("./Lockout");
            }

            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;
            if (info.Principal.HasClaim (_ => _.Type == ClaimTypes.Email)) {
                Input = new InputModel {
                    Email = info.Principal.FindFirstValue (ClaimTypes.Email)
                };
            }
            return Page ();
        }

        public async Task<IActionResult> OnPostConfirmationAsync (string returnUrl = null) {
            returnUrl = returnUrl ?? Url.Content ("~/");
            // Get the information about the user from the external login provider
            var info = await _dispatcher.QueryAsync (new GetExternalLoginInfoQuery ());
            if (info == null) {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage ("./Login", new { ReturnUrl = returnUrl });
            }

            if (!ModelState.IsValid) {
                ProviderDisplayName = info.ProviderDisplayName;
                ReturnUrl = returnUrl;
                return Page ();
            }

            
        }

        #endregion

        #region Public Inner Classes

        public class InputModel {

            #region Public Properties

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            #endregion
        }

        #endregion
    }
}