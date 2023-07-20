using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Nameless.FileStorage;
using Nameless.Logging;
using Nameless.Services;
using Nameless.Services.Impl;

namespace Nameless.Messenger.Email {
    public sealed class MessengerService : IMessengerService {
        #region Private Read-Only Fields

        private readonly IFileStorage _fileStorage;
        private readonly IClockService _clock;
        private readonly MessengerOptions _opts;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="MessengerService"/>.
        /// </summary>
        /// <param name="fileStorage">The file storage.</param>
        /// <param name="options">The SMTP client settings.</param>
        public MessengerService(IFileStorage fileStorage, MessengerOptions options, IClockService? clock = null) {
            Prevent.Against.Null(fileStorage, nameof(fileStorage));

            _fileStorage = fileStorage;
            _opts = options ?? MessengerOptions.Default;
            _clock = clock ?? ClockService.Instance;
        }

        #endregion

        #region Private Methods

        private Task SendAsync(MimeMessage message, CancellationToken cancellationToken) {
            return _opts.DeliveryMethod switch {
                MessengerOptions.DeliveryMethods.PickupDirectory => ToPickupDirectoryAsync(message, cancellationToken),
                MessengerOptions.DeliveryMethods.Network => ToNetworkAsync(message, cancellationToken),
                _ => throw new InvalidOperationException("Invalid delivery method."),
            };
        }

        private async Task ToPickupDirectoryAsync(MimeMessage message, CancellationToken cancellationToken) {
            if (string.IsNullOrWhiteSpace(_opts.PickupDirectoryFolder) || !Directory.Exists(_opts.PickupDirectoryFolder)) {
                throw new InvalidOperationException("Pickup directory not specified or invalid.");
            }

            var now = _clock.UtcNow;
            var path = Path.Combine(_fileStorage.Root, _opts.PickupDirectoryFolder, $"{now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.eml");
            using var stream = new FileStream(path, FileMode.Create);
            await message.WriteToAsync(stream, headersOnly: false, cancellationToken);
        }

        private async Task ToNetworkAsync(MimeMessage message, CancellationToken cancellationToken) {
            using var client = new SmtpClient();
            try {
                await client.ConnectAsync(_opts.Host, _opts.Port, _opts.EnableSsl, cancellationToken);

                // Authenticate if possible and needed.
                if (_opts.UseCredentials && !string.IsNullOrWhiteSpace(_opts.UserName) && client.Capabilities.HasFlag(SmtpCapabilities.Authentication)) {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_opts.UserName, _opts.Password, cancellationToken);
                }
            } catch (Exception ex) { Logger.Error(ex, ex.Message); throw; }

            // Send message
            await client.SendAsync(message, cancellationToken: cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);
        }

        #endregion

        #region IMessenger Members

        public async Task<MessageResponse> DispatchAsync(MessageRequest request, CancellationToken cancellationToken = default) {
            Prevent.Against.Null(request, nameof(request));

            if (request.From.IsNullOrEmpty()) {
                throw new InvalidOperationException("Missing sender address.");
            }

            if (request.To.IsNullOrEmpty()) {
                throw new InvalidOperationException("Missing recipient address.");
            }

            if (!request.Properties.TryGetValue(MessageProperties.UseHtmlBody, out var useHtmlBody)) {
                useHtmlBody = false;
            }

            var mail = new MimeMessage {
                Body = new TextPart((bool)useHtmlBody ? TextFormat.Html : TextFormat.Plain) {
                    Text = request.Message
                },
                Sender = MailboxAddress.Parse(request.From.First()),
                Subject = request.Subject,
                Priority = request.Priority switch {
                    Priority.Low => MessagePriority.NonUrgent,
                    Priority.Medium => MessagePriority.Normal,
                    Priority.High => MessagePriority.Urgent,
                    _ => MessagePriority.Normal
                }
            };

            // Add recipients
            request.From.Each(_ => mail.From.Add(InternetAddress.Parse(_)));
            request.To.Each(_ => mail.To.Add(InternetAddress.Parse(_)));

            if (request.Properties.TryGetValue(MessageProperties.CarbonCopy, out var cc)) {
                ((string)cc).Split(';').Each(_ => mail.Cc.Add(InternetAddress.Parse(_)));
            }
            if (request.Properties.TryGetValue(MessageProperties.BlindCarbonCopy, out var bcc)) {
                ((string)bcc).Split(';').Each(_ => mail.Bcc.Add(InternetAddress.Parse(_)));
            }

            try { await SendAsync(mail, cancellationToken); }
            catch (Exception ex) { return MessageResponse.Failure(ex); }

            return MessageResponse.Successful();
        }

        #endregion
    }
}
