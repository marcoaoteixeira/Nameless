using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Nameless.AspNetCore.Identity;
using Nameless.CQRS;
using Nameless.Mailing;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class SendResetPasswordEmailCommand : ICommand {
        #region Public Properties

        public string Email { get; set; }
        public string CallbackUrl { get; set; }

        #endregion
    }

    public sealed class SendResetPasswordEmailCommandHandler : ICommandHandler<SendResetPasswordEmailCommand> {
        #region Private Read-Only Fields

        private readonly UserManager<User> _userManager;
        private readonly IMailingService _mailingService;

        #endregion

        #region Public Constructors

        public SendResetPasswordEmailCommandHandler (UserManager<User> userManager, IMailingService mailingService) {
            Prevent.ParameterNull (userManager, nameof (userManager));
            Prevent.ParameterNull (mailingService, nameof (mailingService));

            _userManager = userManager;
            _mailingService = mailingService;
        }

        #endregion

        #region ICommandHandler<SendResetPasswordEmailCommand> Members

        public Task<ExecutionResult> HandleAsync (SendResetPasswordEmailCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var message = new Message {
                Subject = "Reset Password",
                Body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(command.CallbackUrl)}'>clicking here</a>."
            };
            message.From.Add ("sys.admin@application.com");
            message.To.Add (command.Email);

            return _mailingService
                .SendAsync (message)
                .ContinueWith (continuation => ExecutionResult.Success ());
        }

        #endregion
    }
}