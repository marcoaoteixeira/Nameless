using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;
using Nameless.CommandQuery;
using Nameless.WebApplication.Domain.v1.Token.Commands;
using Nameless.WebApplication.Domain.v1.Token.Models.Input;

namespace Nameless.WebApplication.Web.Api.v1.Controllers {

    [Authorize]
    public sealed class TokenController : ApiControllerBase {

        #region Private Read-Only Fields

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public TokenController(ICommandDispatcher commandDispatcher, IMapper mapper) {
            Prevent.Null(commandDispatcher, nameof(commandDispatcher));
            Prevent.Null(mapper, nameof(mapper));

            _commandDispatcher = commandDispatcher;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RefreshAsync([FromBody]TokenInput input, CancellationToken cancellationToken = default) {
            var command = _mapper.Map<RefreshTokenCommand>(input);

            var executionResult = await _commandDispatcher.ExecuteAsync(command, cancellationToken);

            executionResult.PushErrorsIntoModelState(ModelState);

            return executionResult.Success ? Ok(executionResult.State) : BadRequest(ModelState);
        }

        #endregion
    }
}
