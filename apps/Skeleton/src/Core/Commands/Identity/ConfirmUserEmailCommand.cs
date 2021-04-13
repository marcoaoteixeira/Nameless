using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class ConfirmUserEmailCommand : ICommand {
        #region Public Properties

        public string UserId { get; set; }
        public string EmailConfirmationToken { get; set; }

        #endregion
    }

    public sealed class ConfirmUserEmailCommandHandler : ICommandHandler<ConfirmUserEmailCommand> {
        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;

        #endregion

        #region Public Constructors

        public ConfirmUserEmailCommandHandler (UserManager<User> userManager) {
            Prevent.ParameterNull (userManager, nameof (userManager));

            _userManager = userManager;
        }

        #endregion

        #region ICommandHandler<ConfirmUserEmailCommand> Members

        public async Task<ExecutionResult> HandleAsync (ConfirmUserEmailCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var user = await _userManager.FindByIdAsync (command.UserId);

            if (user == null) {
                return ExecutionResult.Failure ("User not found.");
            }

            var emailConfirmedResult = await _userManager.ConfirmEmailAsync (user, command.EmailConfirmationToken);

            return !emailConfirmedResult.Succeeded ? ExecutionResult.Failure ("Error confirming e-mail") : ExecutionResult.Success ();
        }

        #endregion
    }
}