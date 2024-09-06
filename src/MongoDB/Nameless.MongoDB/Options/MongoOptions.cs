namespace Nameless.MongoDB.Options;

public sealed record MongoOptions {
    public static MongoOptions Default => new();

    private CredentialsOptions? _credentials;

    public string Host { get; set; } = Root.Defaults.MONGO_HOST;

    public int Port { get; set; } = Root.Defaults.MONGO_PORT;

    public string? Database { get; set; }

    public CredentialsOptions Credentials {
        get => _credentials ??= CredentialsOptions.Default;
        set => _credentials = value;
    }
}