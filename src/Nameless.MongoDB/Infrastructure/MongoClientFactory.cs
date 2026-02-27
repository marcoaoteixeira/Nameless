using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Nameless.MongoDB.Infrastructure;

public class MongoClientFactory : IMongoClientFactory {
    private readonly IOptions<MongoOptions> _options;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MongoClientFactory"/> class.
    /// </summary>
    /// <param name="options">
    ///     MongoDB options to configure the client.
    /// </param>
    public MongoClientFactory(IOptions<MongoOptions> options) {
        _options = Throws.When.Null(options);
    }

    /// <inheritdoc />
    public IMongoClient CreateClient() {
        var options = _options.Value;
        var settings = new MongoClientSettings { Server = new MongoServerAddress(options.Host, options.Port) };

        ConfigureCredentials(settings, options);

        return new MongoClient(settings);

        static void ConfigureCredentials(MongoClientSettings clientSettings, MongoOptions opts) {
            if (!opts.Credentials.UseCredentials) {
                return;
            }

            var identity = new MongoInternalIdentity(
                opts.Credentials.Database,
                opts.Credentials.Username);

            var evidence = new PasswordEvidence(opts.Credentials.Password);

            var credential = new MongoCredential(
                opts.Credentials.Mechanism,
                identity,
                evidence);

            clientSettings.Credential = credential;
        }
    }
}