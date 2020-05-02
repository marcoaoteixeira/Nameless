using System;
using Microsoft.AspNetCore.Mvc;
using Nameless.Bookshelf.Web.Models;

namespace Nameless.Bookshelf.Web.Controllers {
    public class BookController : Controller {
        #region Public Methods

        [HttpGet]
        public IActionResult Show (Guid id) {
            return View ();
        }

        #endregion
    }
}
