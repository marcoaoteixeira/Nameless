using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class GenerateResetPasswordEmailTokenCommand : ICommand {
        #region Public Properties

        public string Email { get; set; }

        #endregion
    }

    public sealed class GenerateResetPasswordEmailTokenCommandHandler : ICommandHandler<GenerateResetPasswordEmailTokenCommand> {
        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;

        #endregion

        #region Public Constructors

        public GenerateResetPasswordEmailTokenCommandHandler (UserManager<User> userManager) {
            Prevent.ParameterNull (userManager, nameof (userManager));

            _userManager = userManager;
        }

        #endregion

        #region ICommandHandler<GenerateResetPasswordEmailTokenCommandCommand> Members

        public async Task<ExecutionResult> HandleAsync (GenerateResetPasswordEmailTokenCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var user = await _userManager.FindByEmailAsync (command.Email);

            if (user == null) {
                return ExecutionResult.Failure ("User not found.");
            }

            if (!await _userManager.IsEmailConfirmedAsync (user)) {
                return ExecutionResult.Failure ("User email not confirmed.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync (user);

            return ExecutionResult.Success (new { EmailResetCodeToken = code });
        }

        #endregion
    }
}