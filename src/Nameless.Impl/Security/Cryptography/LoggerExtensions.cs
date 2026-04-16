using Microsoft.Extensions.Logging;

namespace Nameless.Security.Cryptography;

internal static class LoggerExtensions {
    extension(ILogger<RijndaelCrypto> self) {
        internal void EncryptionException(Exception exception) {
            Log.Failure(self, "RIJNDAEL_CRYPTO", $"{nameof(ICrypto)}.{nameof(ICrypto.Encrypt)}", exception);
        }

        internal void DecryptionFailure(Exception exception) {
            Log.Failure(self, "RIJNDAEL_CRYPTO", $"{nameof(ICrypto)}.{nameof(ICrypto.Decrypt)}", exception);
        }
    }
}