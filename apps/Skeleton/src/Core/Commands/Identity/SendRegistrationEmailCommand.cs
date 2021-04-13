using System;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Nameless.CQRS;
using Nameless.Mailing;

namespace Nameless.Skeleton.Web.Commands.Identity {
    public sealed class SendRegistrationEmailCommand : ICommand {
        #region Public Properties

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CallbackUrl { get; set; }

        #endregion
    }

    public sealed class SendRegistrationEmailCommandHandler : ICommandHandler<SendRegistrationEmailCommand> {
        #region Private Read-Only Fields

        private readonly IMailingService _mailingService;

        #endregion

        #region Public Constructors

        public SendRegistrationEmailCommandHandler (IMailingService mailingService) {
            Prevent.ParameterNull (mailingService, nameof (mailingService));

            _mailingService = mailingService;
        }

        #endregion

        #region ICommandHandler<SendRegistrationEmailCommand>

        public Task<ExecutionResult> HandleAsync (SendRegistrationEmailCommand command, IProgress<int> progress = null, CancellationToken token = default) {
            var message = new Message {
                Subject = "Confirm your email",
                Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(command.CallbackUrl)}'>clicking here</a>."
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