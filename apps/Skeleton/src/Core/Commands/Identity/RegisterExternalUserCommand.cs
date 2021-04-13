using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class RegisterExternalUserCommand : ICommand {
        #region Public Properties

        public string UserName { get; set; }
        public string Email { get; set; }

        #endregion
    }

    public sealed class RegisterExternalUserCommandHandler : ICommandHandler<RegisterExternalUserCommand> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _userEmailStore;

        #endregion

        #region Public Constructors

        public RegisterExternalUserCommandHandler (SignInManager<User> signInManager, UserManager<User> userManager, IUserStore<User> userStore, IUserEmailStore<User> userEmailStore) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));
            Prevent.ParameterNull (userManager, nameof (userManager));
            Prevent.ParameterNull (userStore, nameof (userStore));
            Prevent.ParameterNull (userEmailStore, nameof (userEmailStore));

            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _userEmailStore = userEmailStore;
        }

        #endregion

        #region ICommandHandler<RegisterExternalUserCommand>

        public async Task<ExecutionResult> HandleAsync (RegisterExternalUserCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync ();
            if (info == null) {
                return new ExecutionResult (errorMessage: "Error loading external login information during confirmation.");
            }

            var user = new User {
                UserName = command.UserName
            };

            await _userStore.SetUserNameAsync (user, command.Email, CancellationToken.None);
            await _userEmailStore.SetEmailAsync (user, command.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync (user);

            if (result.Succeeded) {
                result = await _userManager.AddLoginAsync (user, info);
                if (result.Succeeded) {
                    await _signInManager.SignInAsync (user, isPersistent : false);
                    return new ExecutionResult (state: new {
                        SignInSucceeded = true
                    });
                }
            }
            var errorMessage = string.Empty;
            if (!result.Succeeded) {
                errorMessage = string.Join (";", result.Errors.Select (_ => _.Description).ToArray ());
            }

            return !string.IsNullOrWhiteSpace (errorMessage) ? ExecutionResult.Failure (errorMessage) : ExecutionResult.Success ();
        }

        #endregion
    }
}