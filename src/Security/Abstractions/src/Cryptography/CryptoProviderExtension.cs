using System.IO;

namespace Nameless.Security.Cryptography {
    public static class CryptoProviderExtension {
        #region Public Static Methods

        public static byte[] Encrypt (this ICryptoProvider self, byte[] buffer, CryptoOptions options = null) {
            Prevent.ParameterNull (buffer, nameof (buffer));

            if (self == null) { return null; }

            return self.Encrypt (new MemoryStream (buffer), options);
        }

        public static byte[] Decrypt (this ICryptoProvider self, byte[] buffer, CryptoOptions options = null) {
            Prevent.ParameterNull (buffer, nameof (buffer));

            if (self == null) { return null; }

            return self.Decrypt (new MemoryStream (buffer), options);
        }

        public static string Encrypt (this ICryptoProvider self, string value, CryptoOptions options = null) {
            Prevent.ParameterNull (value, nameof (value));

            if (self == null) { return null; }

            var encoding = (options ?? new CryptoOptions ()).Encoding;
            var buffer = encoding.GetBytes (value);
            var result = self.Encrypt (new MemoryStream (buffer), options);
            return encoding.GetString (result);
        }

        public static string Decrypt (this ICryptoProvider self, string value, CryptoOptions options = null) {
            Prevent.ParameterNull (value, nameof (value));

            if (self == null) { return null; }

            var encoding = (options ?? new CryptoOptions ()).Encoding;
            var buffer = encoding.GetBytes (value);
            var result = self.Decrypt (new MemoryStream (buffer), options);
            return encoding.GetString (result);
        }

        #endregion
    }
}