using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;

namespace Nameless.Skeleton.Web.Api.v1.Controllers {
    [ApiVersion ("1")]
    [Route ("api/v{version:apiVersion}/[controller]")]
    public class HomeController : WebApiController {
        public IActionResult List () {
            return Ok (new [] { 1, 2, 3 });
        }
    }
}