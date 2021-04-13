using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public class SignInCommand : ICommand {
        #region Public Properties

        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        #endregion
    }

    public sealed class SignInCommandHandler : ICommandHandler<SignInCommand> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public SignInCommandHandler (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region ICommandHandler<SignInCommand> Members

        public async Task<ExecutionResult> HandleAsync (SignInCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var signInResult = await _signInManager.PasswordSignInAsync (command.Email, command.Password, command.RememberMe, lockoutOnFailure : false);
            var state = new {
                signInResult.IsLockedOut,
                signInResult.IsNotAllowed,
                signInResult.RequiresTwoFactor
            };
            return (signInResult.IsLockedOut || signInResult.IsNotAllowed) ? ExecutionResult.Failure ("Invalid login attempt.") : ExecutionResult.Success (state);
        }

        #endregion
    }
}