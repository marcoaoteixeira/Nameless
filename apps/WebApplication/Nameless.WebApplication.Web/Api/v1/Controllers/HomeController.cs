using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;

namespace Nameless.WebApplication.Web.Api.v1.Controllers {

    public sealed class HomeController : ApiControllerBase {

        #region Public Methods

        [HttpGet, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public Task<IActionResult> GetAsync() {
            IActionResult result = Ok(new[] { "Value A", "Value B", "Value C" });

            return Task.FromResult(result);
        }

        #endregion
    }
}
