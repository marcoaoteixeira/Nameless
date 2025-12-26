namespace Nameless.Security;

public static class CryptoExtensions {
    extension(ICrypto self) {
        public string Encrypt(string value) {
            return self.ExecuteAction(value);
        }

        public string Decrypt(string value) {
            return self.ExecuteAction(value, encrypt: false);
        }

        private string ExecuteAction(string value, bool encrypt = true) {
            using var stream = new MemoryStream(Defaults.Encoding.GetBytes(value));
            var result = encrypt
                ? self.Encrypt(stream)
                : self.Decrypt(stream);

            return Defaults.Encoding.GetString(result);
        }
    }
}