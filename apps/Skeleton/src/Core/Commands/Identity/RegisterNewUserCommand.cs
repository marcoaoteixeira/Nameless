using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class RegisterNewUserCommand : ICommand {
        #region Public Properties

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        #endregion
    }

    public sealed class RegisterNewUserCommandHandler : ICommandHandler<RegisterNewUserCommand> {
        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _userEmailStore;

        #endregion

        #region Public Constructors

        public RegisterNewUserCommandHandler (UserManager<User> userManager, IUserStore<User> userStore, IUserEmailStore<User> userEmailStore) {
            Prevent.ParameterNull (userManager, nameof (userManager));
            Prevent.ParameterNull (userStore, nameof (userStore));
            Prevent.ParameterNull (userEmailStore, nameof (userEmailStore));

            _userManager = userManager;
            _userStore = userStore;
            _userEmailStore = userEmailStore;
        }

        #endregion

        #region ICommandHandler<RegisterNewUserCommand>

        public async Task<ExecutionResult> HandleAsync (RegisterNewUserCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var user = new User {
                UserName = command.UserName
            };

            await _userStore.SetUserNameAsync (user, command.Email, CancellationToken.None);
            await _userEmailStore.SetEmailAsync (user, command.Email, CancellationToken.None);
            var createResult = await _userManager.CreateAsync (user, command.Password);

            var state = new {
                UserId = createResult.Succeeded ? await _userManager.GetUserIdAsync (user) : string.Empty,
                EmailConfirmationToken = createResult.Succeeded ? await _userManager.GenerateEmailConfirmationTokenAsync (user) : string.Empty
            };
            var errorMessage = string.Empty;
            if (!createResult.Succeeded) {
                errorMessage = string.Join (";", createResult.Errors.Select (_ => _.Description).ToArray ());
            }
            
            return !string.IsNullOrWhiteSpace (errorMessage) ? ExecutionResult.Failure (errorMessage) : ExecutionResult.Success ();
        }

        #endregion
    }
}