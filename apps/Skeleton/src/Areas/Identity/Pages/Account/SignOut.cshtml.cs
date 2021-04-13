using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.CQRS;
using Nameless.Skeleton.Web.Commands.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Account {

    [AllowAnonymous]
    public sealed class SignOutPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly ICommandQueryDispatcher _dispatcher;

        #endregion

        #region Public Constructors

        public SignOutPageModel (ICommandQueryDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        public IActionResult OnGet () {
            return Page ();
        }

        public async Task<IActionResult> OnPostAsync (string returnUrl = null) {
            await _dispatcher.CommandAsync (new SignOutCommand ());

            if (returnUrl != null) {
                return LocalRedirect (returnUrl);
            }

            return RedirectToPage ();
        }

        #endregion
    }
}