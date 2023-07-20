using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nameless.Microservice.Api.v1.Models;

namespace Nameless.Microservice.Api.v1.Controllers {
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public sealed class HomeController : ControllerBase {
        #region Public Methods

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(WorkingOutput))]
        public IActionResult Index() {
            return Ok(new WorkingOutput { Message = "It works!" });
        }

        #endregion
    }
}