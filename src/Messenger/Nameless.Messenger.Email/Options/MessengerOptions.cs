#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

using MailKit.Security;

namespace Nameless.Messenger.Email {
    /// <summary>
    /// The configuration for mailing client.
    /// </summary>
    public sealed record MessengerOptions {
        #region Public Static Properties

        public static MessengerOptions Default => new();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the SMTP server address. Default value is "localhost".
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Gets or sets the SMTP server port. Default value is 25.
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// Gets or sets the user name credential.
        /// </summary>
        public string? Username { get; }

        /// <summary>
        /// Gets or sets the password credential.
        /// </summary>
        public string? Password { get; }

        /// <summary>
        /// Gets or sets secure socket option to use if connection is SSL.
        /// Default value is <c><see cref="SecureSocketOptions.None"/></c>.
        /// </summary>
        public SecureSocketOptions SecureSocket { get; set; } = SecureSocketOptions.None;

        /// <summary>
        /// Gets or sets the delivery mode. Default value is
        /// <see cref="DeliveryMode.PickupDirectory" />.
        /// </summary>
        public DeliveryMode DeliveryMode { get; set; } = DeliveryMode.PickupDirectory;

        /// <summary>
        /// Gets or sets the pickup directory path, relative to the
        /// application file storage. Default value is
        /// "Messenger_PickupDirectory".
        /// </summary>
        public string PickupDirectoryFolderName { get; set; } = "Messenger_PickupDirectory";

        #endregion

        #region Public Constructors

        public MessengerOptions() {
            Username = Environment.GetEnvironmentVariable(Root.EnvTokens.MESSENGER_SMTP_USER);
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.MESSENGER_SMTP_PASS);
        }

        #endregion

        #region Public Methods

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
#endif
        public bool UseCredentials()
            => Username is not null &&
               Password is not null;

        #endregion
    }
}
