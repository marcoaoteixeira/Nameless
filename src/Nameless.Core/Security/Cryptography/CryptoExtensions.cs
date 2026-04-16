namespace Nameless.Security.Cryptography;

public static class CryptoExtensions {
    extension(ICrypto self) {
        public string Encrypt(string value) {
            return self.ExecuteAction(value);
        }

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