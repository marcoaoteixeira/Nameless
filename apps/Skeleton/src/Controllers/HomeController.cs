using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nameless.Skeleton.Web.Models;

namespace Nameless.Skeleton.Web.Controllers {
    public class HomeController : Controller {
        #region Public Methods

        public IActionResult Index () {

            return View ();
        }

        public IActionResult Privacy () {
            return View ();
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AccessDenied () {
            return View ();
        }

        #endregion
    }
}