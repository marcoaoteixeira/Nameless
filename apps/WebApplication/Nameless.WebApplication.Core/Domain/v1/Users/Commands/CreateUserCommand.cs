using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Nameless.CommandQuery;
using Nameless.Infrastructure;
using Nameless.WebApplication.Domain.v1.Users.Models.Output;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Domain.v1.Users.Commands {

    public sealed class CreateUserCommand : ICommand {

        #region Public Properties

        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
        public string? PhoneNumber { get; set; }

        #endregion
    }

    public sealed class CreateUserCommandHandler : CommandHandlerBase<CreateUserCommand> {

        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;

        #endregion

        #region Public Constructors

        public CreateUserCommandHandler(UserManager<User> userManager, IMapper mapper, IValidator<CreateUserCommand> validator)
            : base(mapper, validator) {
            Prevent.Null(userManager, nameof(userManager));

            _userManager = userManager;
        }

        #endregion

        #region Protected Override Methods

        protected override async Task<ExecutionResult> InnerHandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default) {
            var user = Mapper.Map<User>(command);

            var result = await _userManager.CreateAsync(user, command.Password);
            var errors = Mapper.Map<ExecutionResult.Error[]>(result.Errors);
            var state = Mapper.Map<CreateUserOutput>(user);

            return result.Succeeded ? ExecutionResult.Successful(state) : ExecutionResult.Failure(errors);
        }

        #endregion
    }
}
