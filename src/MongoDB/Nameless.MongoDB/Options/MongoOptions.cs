namespace Nameless.MongoDB.Options {
    public sealed class MongoOptions {
        #region Public Static Read-Only Properties

        public static MongoOptions Default => new();

        #endregion

        #region Private Fields

        private CredentialsOptions? _credentials;

        #endregion

        #region Public Properties

        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 27017;
        public string? Database { get; set; }
        public CredentialsOptions Credentials {
            get => _credentials ??= CredentialsOptions.Default;
            set => _credentials = value ?? CredentialsOptions.Default;
        }

        #endregion
    }
}
