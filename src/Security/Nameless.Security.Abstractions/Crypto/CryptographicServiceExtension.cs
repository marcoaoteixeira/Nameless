namespace Nameless.Security.Crypto {
    public static class CryptographicServiceExtension {
        #region Private Static Methods

        private static string ExecuteAction(ICryptographicService cryptographicService, string value, bool encrypt = true) {
            using var stream = new MemoryStream(Root.Defaults.Encoding.GetBytes(value));
            var result = encrypt
                ? cryptographicService.Encrypt(stream)
                : cryptographicService.Decrypt(stream);

            return Root.Defaults.Encoding.GetString(result);
        }

        #endregion

        #region Public Static Methods

        public static string Encrypt(this ICryptographicService self, string value)
            => ExecuteAction(self, value, encrypt: true);

        public static string Decrypt(this ICryptographicService self, string value)
            => ExecuteAction(self, value, encrypt: false);

        #endregion
    }
}
