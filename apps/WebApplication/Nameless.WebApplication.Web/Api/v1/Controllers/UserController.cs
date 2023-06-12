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

        private readonly IDispatcherService _dispatcherService;
        private readonly IMapper _mapper;

        #endregion

        #region Public Constructors

        public UserController(IDispatcherService dispatcherService, IMapper mapper) {
            Prevent.Null(dispatcherService, nameof(dispatcherService));
            Prevent.Null(mapper, nameof(mapper));

            _dispatcherService = dispatcherService;
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

            var executionResult = await _dispatcherService.ExecuteAsync(command, cancellationToken);

            executionResult.PushErrorsIntoModelState(ModelState);

            return executionResult.Success ? Ok(executionResult.State) : BadRequest(ModelState);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromQuery]Guid userId, CancellationToken cancellationToken = default) {
            var query = new GetUserByIdQuery { UserId = userId };

            var result = await _dispatcherService.ExecuteAsync(query, cancellationToken);

            return result != default ? Ok(result) : NotFound();
        }

        #endregion
    }
}
