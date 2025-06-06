namespace Nameless.Security.Crypto;

public static class CryptographicServiceExtensions {
    public static string Encrypt(this ICryptographicService self, string value) {
        return self.ExecuteAction(value);
    }

    public static string Decrypt(this ICryptographicService self, string value) {
        return self.ExecuteAction(value, encrypt: false);
    }

    private static string ExecuteAction(this ICryptographicService cryptographicService, string value, bool encrypt = true) {
        using var stream = new MemoryStream(Defaults.Encoding.GetBytes(value));
        var result = encrypt
            ? cryptographicService.Encrypt(stream)
            : cryptographicService.Decrypt(stream);

        return Defaults.Encoding.GetString(result);
    }
}