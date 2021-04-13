using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nameless.CQRS;
using Nameless.Skeleton.Web.Commands.Identity;

namespace Nameless.Skeleton.Web.Areas.Identity.Pages.Account {

    [AllowAnonymous]
    public sealed class ConfirmEmailPageModel : PageModel {
        #region Private Read-Only Fields

        private readonly ICommandQueryDispatcher _dispatcher;

        #endregion

        #region Public Constructors

        public ConfirmEmailPageModel (ICommandQueryDispatcher dispatcher) {
            Prevent.ParameterNull (dispatcher, nameof (dispatcher));

            _dispatcher = dispatcher;
        }

        #endregion

        #region Public Methods

        public async Task<IActionResult> OnGetAsync (string userId, string code) {
            var result = await _dispatcher.CommandAsync (new ConfirmUserEmailCommand {
                UserId = userId,
                EmailConfirmationToken = code
            });

            if (!result.Succeeded) {
                return RedirectToAction ("Error");
            }

            return Page ();
        }

        #endregion
    }
}