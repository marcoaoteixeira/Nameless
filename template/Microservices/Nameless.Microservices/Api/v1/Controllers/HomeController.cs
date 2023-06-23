using Microsoft.AspNetCore.Mvc;

namespace Nameless.Microservices.Api.v1.Controllers {
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class HomeController : ControllerBase {
        #region Public Methods

        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Index() {
            return Ok(new { message = "It works!" });
        }

        #endregion
    }
}