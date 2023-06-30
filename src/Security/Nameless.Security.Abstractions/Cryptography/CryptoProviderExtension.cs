using System.Text;

namespace Nameless.Security.Cryptography {

    public static class CryptoProviderExtension {

        #region Private Static Methods

        private static string ExecuteAction(ICryptoProvider? cryptoProvider, string? value, bool encrypt = true) {
            Garda.Prevent.Null(cryptoProvider, nameof(cryptoProvider));

            if (string.IsNullOrWhiteSpace(value)) { return string.Empty; }

            var encoding = Encoding.UTF8;

            using var stream = new MemoryStream(encoding.GetBytes(value));
            var result = encrypt
                ? cryptoProvider.Encrypt(stream)
                : cryptoProvider.Decrypt(stream);

            return encoding.GetString(result);
        }

        #endregion

        #region Public Static Methods

        public static string Encrypt(this ICryptoProvider? self, string? value) => ExecuteAction(self, value, encrypt: true);

        public static string Decrypt(this ICryptoProvider? self, string? value) => ExecuteAction(self, value, encrypt: false);

        #endregion
    }
}
