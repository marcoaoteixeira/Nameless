using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MimeKit;
using MimeKit.Text;
using Nameless.Infrastructure;
using CoreRoot = Nameless.Root;

namespace Nameless.Messenger.Email {
    public sealed class EmailMessenger : IMessenger {
        #region Private Read-Only Fields

        private readonly IApplicationContext _applicationContext;
        private readonly MessengerOptions _options;

        #endregion

        #region Private Fields

        private string _pickupDirectoryPath = null!;

        #endregion

        #region Public Properties

        private ILogger? _logger;
        public ILogger Logger {
            get => _logger ??= NullLogger.Instance;
            set => _logger = value;
        }

        #endregion

        #region Public Constructors

        public EmailMessenger(IApplicationContext environment, MessengerOptions? options = null) {
            _applicationContext = Guard.Against.Null(environment, nameof(environment));
            _options = options ?? MessengerOptions.Default;

            Initialize();
        }

        #endregion

        #region Private Methods

        private void Initialize() {
            // Ensure pickup directory existence
            _pickupDirectoryPath = Path.Combine(_applicationContext.BasePath, _options.PickupDirectoryFolder);
            if (!Directory.Exists(_pickupDirectoryPath)) {
                Directory.CreateDirectory(_pickupDirectoryPath);
            }
        }

        private Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
            => _options.DeliveryMode switch {
                DeliveryMode.PickupDirectory => ToPickupDirectoryAsync(message, cancellationToken),
                DeliveryMode.Network => ToNetworkAsync(message, cancellationToken),
                _ => throw new InvalidOperationException("Invalid delivery method."),
            };

        private async Task ToPickupDirectoryAsync(MimeMessage message, CancellationToken cancellationToken) {
            if (!Directory.Exists(_pickupDirectoryPath)) {
                throw new InvalidOperationException("Pickup directory not specified or invalid.");
            }

            var now = DateTime.UtcNow;
            var path = Path.Combine(_pickupDirectoryPath, $"{now:yyyyMMddHHmmss}_{Guid.NewGuid():N}.eml");
            using var stream = new FileStream(path, FileMode.Create);
            await message.WriteToAsync(stream, headersOnly: false, cancellationToken);
        }

        private async Task ToNetworkAsync(MimeMessage message, CancellationToken cancellationToken) {
            using var client = new SmtpClient();
            try {
                await client.ConnectAsync(_options.Host, _options.Port, _options.EnableSsl, cancellationToken);

                // Authenticate if possible and needed.
                if (_options.Credentials.UseCredentials && client.Capabilities.HasFlag(SmtpCapabilities.Authentication)) {
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_options.Credentials.UserName, _options.Credentials.Password, cancellationToken);
                }
            } catch (Exception ex) { Logger.LogError(ex, "{ex.Message}", ex.Message); throw; }

            // Send message
            await client.SendAsync(message, cancellationToken: cancellationToken);
            await client.DisconnectAsync(quit: true, cancellationToken);
        }

        #endregion

        #region IMessenger Members

        public async Task<Response> SendAsync(Request request, CancellationToken cancellationToken = default) {
            Guard.Against.Null(request, nameof(request));

            if (request.From.IsNullOrEmpty()) {
                throw new InvalidOperationException("Missing sender address.");
            }

            if (request.To.IsNullOrEmpty()) {
                throw new InvalidOperationException("Missing recipient address.");
            }

            var mail = new MimeMessage {
                Body = new TextPart(request.Args.GetUseHtmlBody() ? TextFormat.Html : TextFormat.Plain) {
                    Text = request.Message
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

            // Add recipients
            request.From.Each(_ => mail.From.Add(InternetAddress.Parse(_)));
            request.To.Each(_ => mail.To.Add(InternetAddress.Parse(_)));

            request.Args.GetCarbonCopy().Split(CoreRoot.Separators.SEMICOLON).Each(_ => mail.Cc.Add(InternetAddress.Parse(_)));
            request.Args.GetBlindCarbonCopy().Split(CoreRoot.Separators.SEMICOLON).Each(_ => mail.Bcc.Add(InternetAddress.Parse(_)));

            try {
                await SendAsync(mail, cancellationToken);
            } catch (Exception ex) {
                return Response.Failure(ex);
            }

            return Response.Successful();
        }

        #endregion
    }
}
