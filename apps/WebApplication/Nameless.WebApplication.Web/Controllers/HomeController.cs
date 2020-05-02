using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nameless.Localization;
using Nameless.WebApplication.Web.Models;

namespace Nameless.WebApplication.Web.Controllers {
    public class HomeController : Controller {
        #region Private Read-Only Fields

        private Localizer _localizer;
        public Localizer T {
            get { return _localizer ?? (_localizer = EmptyStringLocalizer.Create<HomeController> ().Get); }
            set { _localizer = value ?? EmptyStringLocalizer.Create<HomeController> ().Get; }
        }

        #endregion

        #region Public Methods

        public IActionResult Index () {
            return View ();
        }

        public IActionResult Privacy () {
            return View ();
        }

        public IActionResult SignIn () {
            return View ("Views/Account/SignIn.cshtml");
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}