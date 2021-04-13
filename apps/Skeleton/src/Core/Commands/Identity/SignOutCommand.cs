using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class SignOutCommand : ICommand {

    }

    public sealed class SignOutCommandHandler : ICommandHandler<SignOutCommand> {
        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Public Constructors

        public SignOutCommandHandler (SignInManager<User> signInManager) {
            Prevent.ParameterNull (signInManager, nameof (signInManager));

            _signInManager = signInManager;
        }

        #endregion

        #region ICommandHandler<SignOutCommand>

        public async Task<ExecutionResult> HandleAsync (SignOutCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            await _signInManager.SignOutAsync ();

            return ExecutionResult.Success ();
        }

        #endregion
    }
}