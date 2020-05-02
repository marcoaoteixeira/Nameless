using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Nameless.Logging;
using MimeKit;
using MimeKit.Text;

namespace Nameless.Mailing.MailKit {

    /// <summary>
    /// The default implementation of <see cref="IMailingService"/>
    /// </summary>
    public sealed class MailingService : IMailingService {

        #region Private Read-Only Fields

        private readonly SmtpClientSettings _settings;

        #endregion

        #region Private Fields

        private ILogger _logger;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Logger value.
        /// </summary>
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="EmailService"/>.
        /// </summary>
        /// <param name="settings">The SMTP client settings.</param>
        public MailingService (SmtpClientSettings settings) {
            Prevent.ParameterNull (settings, nameof (settings));

            _settings = settings;
        }

        #endregion

        #region Private Methods

        private Task InnerSendAsync (MimeMessage message, CancellationToken token) {
            switch (_settings.DeliveryMethod) {
                case SmtpClientSettings.DeliveryMethods.PickupDirectory:
                    return SendViaPickupDirectoryAsync (message, token);

                case SmtpClientSettings.DeliveryMethods.Network:
                    return SendViaNetworkAsync (message, token);

                default:
                    throw new ArgumentException ("Invalid delivery method.");
            }
        }

        private Task SendViaPickupDirectoryAsync (MimeMessage message, CancellationToken token) {
            if (string.IsNullOrWhiteSpace (_settings.PickupDirectoryPath) || !Directory.Exists (_settings.PickupDirectoryPath)) {
                throw new InvalidOperationException ("Pickup directory not specified or invalid.");
            }

            var path = Path.Combine (_settings.PickupDirectoryPath, $"{Guid.NewGuid ()}.eml");
            var stream = new FileStream (path, FileMode.Create);
            return message
                .WriteToAsync (stream, headersOnly: false, cancellationToken: token)
                .ContinueWith ((continuation, state) => {
                    if (continuation.CanContinue () && state is FileStream currentStream) {
                        currentStream.Close ();
                        currentStream.Dispose ();
                    }
                }, state: stream);
        }

        private Task SendViaNetworkAsync (MimeMessage message, CancellationToken token) {
            SmtpClient client = null;
            try {
                client = new SmtpClient ();
                client.Connect (_settings.Host, _settings.Port, _settings.EnableSsl, cancellationToken: token);

                // Authenticate if possible and needed.
                if (_settings.UseCredentials && !string.IsNullOrWhiteSpace (_settings.UserName) && client.Capabilities.HasFlag (SmtpCapabilities.Authentication)) {
                    client.AuthenticationMechanisms.Remove ("XOAUTH2");
                    client.Authenticate (_settings.UserName, _settings.Password);
                }
            } catch (Exception ex) { Logger.Error (ex, ex.Message); throw; }

            // Send message
            return client
                .SendAsync (message, cancellationToken: token)
                .ContinueWith ((continuation, state) => {
                    if (continuation.Exception != null) {
                        Logger.Error (continuation.Exception.Flatten (), continuation.Exception.Message);
                    }

                    if (state is SmtpClient currentClient) {
                        if (currentClient.IsConnected) {
                            currentClient.Disconnect (quit: true);
                        }
                        currentClient.Dispose ();
                    }
                }, state: client);
        }

        #endregion

        #region ISmtpClient Members

        /// <inheritdoc/>
        public Task SendAsync (Message message, CancellationToken token = default (CancellationToken)) {
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