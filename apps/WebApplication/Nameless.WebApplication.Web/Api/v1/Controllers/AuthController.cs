using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;
using Nameless.CommandQuery;
using Nameless.WebApplication.Domain.v1.Auth.Commands;
using Nameless.WebApplication.Domain.v1.Auth.Models.Input;

namespace Nameless.WebApplication.Web.Api.v1.Controllers {

    public sealed class AuthController : ApiControllerBase {

        #region Private Read-Only Fields

        private readonly IDispatcherService _dispatcherService;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public AuthController(IDispatcherService dispatcherService, IMapper mapper) {
            Prevent.Null(dispatcherService, nameof(dispatcherService));
            Prevent.Null(mapper, nameof(mapper));

            _dispatcherService = dispatcherService;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        [HttpPost, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] AuthenticationInput input, CancellationToken cancellationToken = default) {
            var command = _mapper.Map<AuthenticationCommand>(input);

            var executionResult = await _dispatcherService.ExecuteAsync(command, cancellationToken);

            return executionResult.Success
                ? Ok(executionResult.State)
                : BadRequest(executionResult.ToErrorOutput());
        }

        #endregion
    }
}
