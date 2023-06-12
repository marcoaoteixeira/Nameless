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

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public AuthController(ICommandDispatcher commandDispatcher, IMapper mapper) {
            Prevent.Null(commandDispatcher, nameof(commandDispatcher));
            Prevent.Null(mapper, nameof(mapper));

            _commandDispatcher = commandDispatcher;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        [HttpPost, AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostAsync([FromBody] AuthenticationInput input, CancellationToken cancellationToken = default) {
            var command = _mapper.Map<AuthenticationCommand>(input);

            var executionResult = await _commandDispatcher.ExecuteAsync(command, cancellationToken);

            return executionResult.Success
                ? Ok(executionResult.State)
                : BadRequest(executionResult.ToErrorOutput());
        }

        #endregion
    }
}
