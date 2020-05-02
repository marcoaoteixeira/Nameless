using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;

namespace Nameless.WebApplication.Web.Areas.Bookshelf.Api.v1.Controllers {

    [Area ("Bookshelf")]
    [ApiVersion ("1")]
    [Route ("[area]/api/v{version:apiVersion}/[controller]")]
    public class BookController : WebApiController {
        #region Public Methods

        [HttpGet]
        public IActionResult Search () {
            return Ok (new { message = "Ok" });
        }

        #endregion
    }
}