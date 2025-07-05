namespace Nameless.Security;

public static class CryptoExtensions {
    public static string Encrypt(this ICrypto self, string value) {
        return self.ExecuteAction(value);
    }

    public static string Decrypt(this ICrypto self, string value) {
        return self.ExecuteAction(value, encrypt: false);
    }

    private static string ExecuteAction(this ICrypto cryptographicService, string value, bool encrypt = true) {
        using var stream = new MemoryStream(Defaults.Encoding.GetBytes(value));
        var result = encrypt
            ? cryptographicService.Encrypt(stream)
            : cryptographicService.Decrypt(stream);

        return Defaults.Encoding.GetString(result);
    }
}