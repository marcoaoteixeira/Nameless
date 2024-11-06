namespace Nameless.MongoDB.Options;

public sealed record MongoOptions {
    public string Host { get; set; } = Internals.Constants.MONGO_HOST;

    public int Port { get; set; } = Internals.Constants.MONGO_PORT;

    public string? DatabaseName { get; set; }

    private string[]? _documentMappers;
    public string[] DocumentMappers {
        get => _documentMappers ??= [];
        set => _documentMappers = value;
    }

    private CredentialsSettings? _credentials;
    public CredentialsSettings Credentials {
        get => _credentials ??= new CredentialsSettings();
        set => _credentials = value;
    }
}