namespace Nameless.Security.Cryptography;

/// <summary>
///     <see cref="ICrypto"/> extension methods.
/// </summary>
public static class CryptoExtensions {
    /// <param name="self">
    ///     The current <see cref="ICrypto"/> instance.
    /// </param>
    extension(ICrypto self) {
        /// <summary>
        ///     Encrypts the specified string value.
        /// </summary>
        /// <param name="value">
        ///     The string value.
        /// </param>
        /// <returns>
        ///     The encrypted string value.
        /// </returns>
        public string Encrypt(string value) {
            return self.ExecuteAction(value);
        }

        /// <summary>
        ///     Decrypts the specified string value.
        /// </summary>
        /// <param name="value">
        ///     The string value.
        /// </param>
        /// <returns>
        ///     The decrypted string value.
        /// </returns>
        public string Decrypt(string value) {
            return self.ExecuteAction(value, encrypt: false);
        }

        private string ExecuteAction(string value, bool encrypt = true) {
            using var stream = new MemoryStream(CoreDefaults.Encoding.GetBytes(value));

            var result = encrypt
                ? self.Encrypt(stream)
                : self.Decrypt(stream);

            return CoreDefaults.Encoding.GetString(result);
        }
    }
}