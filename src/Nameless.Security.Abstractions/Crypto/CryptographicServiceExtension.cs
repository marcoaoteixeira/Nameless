namespace Nameless.Security.Crypto;

public static class CryptographicServiceExtension {
    public static string Encrypt(this ICryptographicService self, string value)
        => ExecuteAction(self, value, encrypt: true);

    public static string Decrypt(this ICryptographicService self, string value)
        => ExecuteAction(self, value, encrypt: false);

    private static string ExecuteAction(ICryptographicService cryptographicService, string value, bool encrypt = true) {
        using var stream = new MemoryStream(Root.Defaults.Encoding.GetBytes(value));
        var result = encrypt
            ? cryptographicService.Encrypt(stream)
            : cryptographicService.Decrypt(stream);

        return Root.Defaults.Encoding.GetString(result);
    }
}