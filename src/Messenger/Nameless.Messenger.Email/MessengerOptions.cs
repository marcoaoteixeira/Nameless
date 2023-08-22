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
        public string Host { get; init; } = "localhost";
        /// <summary>
        /// Gets or sets the SMTP server port. Default value is 25.
        /// </summary>
        public int Port { get; init; } = 25;
        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        public Credentials Credentials { get; init; } = new();
        /// <summary>
        /// Gets or sets whether should enable SSL. Default value is
        /// <c>false</c>.
        /// </summary>
        public bool EnableSsl { get; init; }
        /// <summary>
        /// Gets or sets the delivery mode. Default value is
        /// <see cref="DeliveryMode.PickupDirectory" />.
        /// </summary>
        public DeliveryMode DeliveryMode { get; init; } = DeliveryMode.PickupDirectory;
        /// <summary>
        /// Gets or sets the pickup directory path, relative to the
        /// application file storage. Default value is
        /// "App_Data/Messenger/PickupDirectory".
        /// </summary>
        public string PickupDirectoryFolder { get; init; } = Path.Combine("App_Data", "Messenger", "PickupDirectory");

        #endregion
    }

    public sealed record Credentials {
        #region Public Properties

        /// <summary>
        /// Gets or sets the user name credential.
        /// </summary>
        public string? UserName { get; }

        /// <summary>
        /// Gets or sets the password credential.
        /// </summary>
        public string? Password { get; }

        public bool UseCredentials
            => UserName is not null &&
               Password is not null;

        #endregion

        #region Public Constructors

        public Credentials() {
            UserName = Environment.GetEnvironmentVariable(Root.EnvTokens.MESSENGER_USER);
            Password = Environment.GetEnvironmentVariable(Root.EnvTokens.MESSENGER_PASS);
        }

        #endregion
    }
}
