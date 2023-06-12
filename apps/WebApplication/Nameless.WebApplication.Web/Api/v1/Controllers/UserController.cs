using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nameless.AspNetCore;
using Nameless.CommandQuery;
using Nameless.WebApplication.Domain.v1.Users.Commands;
using Nameless.WebApplication.Domain.v1.Users.Models.Input;
using Nameless.WebApplication.Domain.v1.Users.Queries;

namespace Nameless.WebApplication.Web.Api.v1.Controllers {

    [Authorize]
    public sealed class UserController : ApiControllerBase {

        #region Private Read-Only Fields

        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public UserController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher, IMapper mapper) {
            Prevent.Null(commandDispatcher, nameof(commandDispatcher));
            Prevent.Null(queryDispatcher, nameof(queryDispatcher));
            Prevent.Null(mapper, nameof(mapper));

            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
            _mapper = mapper;
        }

        #endregion

        #region Public Methods

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostAsync([FromBody] CreateUserInput input, CancellationToken cancellationToken = default) {
            var command = _mapper.Map<CreateUserCommand>(input);

            var executionResult = await _commandDispatcher.ExecuteAsync(command, cancellationToken);

            executionResult.PushErrorsIntoModelState(ModelState);

            return executionResult.Success ? Ok(executionResult.State) : BadRequest(ModelState);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromQuery]Guid userId, CancellationToken cancellationToken = default) {
            var query = new GetUserByIdQuery { UserId = userId };

            var result = await _queryDispatcher.ExecuteAsync(query, cancellationToken);

            return result != default ? Ok(result) : NotFound();
        }

        #endregion
    }
}
