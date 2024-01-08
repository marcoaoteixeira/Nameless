using MimeKit;
using MimeKit.Text;
using CoreRoot = Nameless.Root;

namespace Nameless.Messenger.Email.Impl {
    public sealed class EmailMessenger : IMessenger {
        #region Private Read-Only Fields

        private readonly IDeliveryHandler _deliveryHandler;

        #endregion

        #region Public Constructors

        public EmailMessenger(IDeliveryHandler deliveryHandler) {
            _deliveryHandler = Guard.Against.Null(deliveryHandler, nameof(deliveryHandler));
        }

        #endregion

        #region Private Methods

        private static MimeMessage CreateMailObject(MessageRequest request) {
            var format = request.Args.GetUseHtmlBody()
                ? TextFormat.Html
                : TextFormat.Plain;
            var mimeMessage = new MimeMessage {
                Body = new TextPart(format) {
                    Text = request.Content
                },
                Sender = MailboxAddress.Parse(request.From.First()),
                Subject = request.Subject,
                Priority = request.Priority switch {
                    Priority.Low => MessagePriority.NonUrgent,
                    Priority.Normal => MessagePriority.Normal,
                    Priority.High => MessagePriority.Urgent,
                    _ => MessagePriority.Normal
                }
            };

            return mimeMessage;
        }

        private static void SetRecipients(InternetAddressList addressList, string[] addresses) {
            foreach (var address in addresses) {
                if (InternetAddress.TryParse(address, out var recipient)) {
                    addressList.Add(recipient);
                }
            }
        }

        private static string[] SplitAddresses(string csv)
            => csv
                .Split(
                    separator: CoreRoot.Separators.COMMA,
                    options: StringSplitOptions.RemoveEmptyEntries
                );

        #endregion

        #region IMessenger Members

        public async Task<MessageResponse> SendAsync(MessageRequest request, CancellationToken cancellationToken = default) {
            Guard.Against.Null(request, nameof(request));

            if (request.From.IsNullOrEmpty()) {
                throw new InvalidOperationException("Missing sender address.");
            }

            if (request.To.IsNullOrEmpty()) {
                throw new InvalidOperationException("Missing recipient address.");
            }

            var mail = CreateMailObject(request);

            SetRecipients(mail.From, request.From);
            SetRecipients(mail.To, request.To);
            SetRecipients(mail.Cc, SplitAddresses(request.Args.GetCarbonCopy()));
            SetRecipients(mail.Bcc, SplitAddresses(request.Args.GetBlindCarbonCopy()));

            try { await _deliveryHandler.HandleAsync(mail, cancellationToken); } catch (Exception ex) { return MessageResponse.Failure(ex); }

            return MessageResponse.Success();
        }

        #endregion
    }
}
