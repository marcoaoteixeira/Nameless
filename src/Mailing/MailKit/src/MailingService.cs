using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Nameless.FileStorage;
using Nameless.Logging;

namespace Nameless.Mailing.MailKit {

    /// <summary>
    /// The default implementation of <see cref="IMailingService"/>
    /// </summary>
    public sealed class MailingService : IMailingService {

        #region Private Read-Only Fields

        private readonly IFileStorage _fileStorage;
        private readonly MailingSettings _settings;

        #endregion

        #region Public Properties

#pragma warning disable IDE0074
        private ILogger _logger;
        /// <summary>
        /// Gets or sets the Logger value.
        /// </summary>
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }
#pragma warning restore IDE0074

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="EmailService"/>.
        /// </summary>
        /// <param name="fileStorage">The file storage.</param>
        /// <param name="settings">The SMTP client settings.</param>
        public MailingService (IFileStorage fileStorage, MailingSettings settings = null) {
            Prevent.ParameterNull (fileStorage, nameof (fileStorage));

            _settings = settings ?? new MailingSettings ();
            _fileStorage = fileStorage;
        }

        #endregion

        #region Private Methods

        private Task InnerSendAsync (MimeMessage message, CancellationToken token) {
            return _settings.DeliveryMethod
            switch {
                MailingSettings.DeliveryMethods.PickupDirectory => SendOfflineAsync (message, token),
                    MailingSettings.DeliveryMethods.Network => SendOnlineAsync (message, token),
                    _ =>
                    throw new ArgumentException ("Invalid delivery method."),
            };
        }

        private async Task SendOfflineAsync (MimeMessage message, CancellationToken token) {
            if (string.IsNullOrWhiteSpace (_settings.PickupDirectoryFolder) || !Directory.Exists (_settings.PickupDirectoryFolder)) {
                throw new InvalidOperationException ("Pickup directory not specified or invalid.");
            }

            var now = DateTime.Now;
            var path = Path.Combine (_fileStorage.Root, _settings.PickupDirectoryFolder, $"{Guid.NewGuid ():N}_{now:yyyyMMddHHmmssfff}.eml");
            using var stream = new FileStream (path, FileMode.Create);
            await message.WriteToAsync (stream, headersOnly : false, cancellationToken : token);
        }

        private async Task SendOnlineAsync (MimeMessage message, CancellationToken token) {
            using var client = new SmtpClient ();
            try {
                await client.ConnectAsync (_settings.Host, _settings.Port, _settings.EnableSsl, cancellationToken : token);

                // Authenticate if possible and needed.
                if (_settings.UseCredentials && !string.IsNullOrWhiteSpace (_settings.UserName) && client.Capabilities.HasFlag (SmtpCapabilities.Authentication)) {
                    client.AuthenticationMechanisms.Remove ("XOAUTH2");
                    await client.AuthenticateAsync (_settings.UserName, _settings.Password, cancellationToken : token);
                }
            } catch (Exception ex) { Logger.Error (ex, ex.Message); throw; }

            // Send message
            await client.SendAsync (message, cancellationToken : token);
            await client.DisconnectAsync (quit: true, cancellationToken: token);
        }

        #endregion

        #region ISmtpClient Members

        /// <inheritdoc/>
        public Task SendAsync (Message message, CancellationToken token = default) {
            var mail = new MimeMessage {
            Body = new TextPart (message.IsBodyHtml ? TextFormat.Html : TextFormat.Plain) {
            Text = message.Body
            },
            Sender = MailboxAddress.Parse (message.Sender),
            Subject = message.Subject
            };

            // Set mail priority
            switch (message.Priority) {
                case MessagePriority.Low:
                    mail.Priority = MimeKit.MessagePriority.NonUrgent;
                    break;

                case MessagePriority.Medium:
                    mail.Priority = MimeKit.MessagePriority.Normal;
                    break;

                case MessagePriority.High:
                    mail.Priority = MimeKit.MessagePriority.Urgent;
                    break;
            }

            // Add recipients
            message.From.Each (_ => mail.From.Add (InternetAddress.Parse (_)));
            message.To.Each (_ => mail.To.Add (InternetAddress.Parse (_)));
            message.Cc.Each (_ => mail.Cc.Add (InternetAddress.Parse (_)));
            message.Bcc.Each (_ => mail.Bcc.Add (InternetAddress.Parse (_)));

            return InnerSendAsync (mail, token);
        }

        #endregion
    }
}