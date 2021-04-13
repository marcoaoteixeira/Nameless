using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class DoExternalLoginSignInCommand : ICommand {
        #region Public Properties

        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }

        #endregion
    }

    public sealed class DoExternalLoginSignInCommandHandler : ICommandHandler<DoExternalLoginSignInCommand> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public DoExternalLoginSignInCommandHandler (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region ICommandHandler<DoExternalLoginSignInCommand>

        public async Task<ExecutionResult> HandleAsync (DoExternalLoginSignInCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var signInResult = await _signInManager.ExternalLoginSignInAsync (command.LoginProvider, command.ProviderKey, isPersistent : false, bypassTwoFactor : true);

            return new ExecutionResult (state: new {
                signInResult.IsLockedOut,
                signInResult.IsNotAllowed,
                signInResult.RequiresTwoFactor,
                signInResult.Succeeded
            }, errorMessage: (signInResult.IsLockedOut || signInResult.IsNotAllowed) ? "User locked out or not allowed." : null);
        }

        #endregion
    }
}