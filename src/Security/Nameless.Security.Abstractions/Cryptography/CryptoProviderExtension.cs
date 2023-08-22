namespace Nameless.Security.Cryptography {
    public static class CryptoProviderExtension {
        #region Private Static Methods

        private static string ExecuteAction(ICryptoProvider? cryptoProvider, string value, bool encrypt = true) {
            Guard.Against.Null(cryptoProvider, nameof(cryptoProvider));

            using var stream = new MemoryStream(Root.Defaults.Encoding.GetBytes(value));
            var result = encrypt
                ? cryptoProvider.Encrypt(stream)
                : cryptoProvider.Decrypt(stream);

            return Root.Defaults.Encoding.GetString(result);
        }

        #endregion

        #region Public Static Methods

        public static string Encrypt(this ICryptoProvider? self, string value)
            => ExecuteAction(self, value, encrypt: true);

        public static string Decrypt(this ICryptoProvider? self, string value)
            => ExecuteAction(self, value, encrypt: false);

        #endregion
    }
}
