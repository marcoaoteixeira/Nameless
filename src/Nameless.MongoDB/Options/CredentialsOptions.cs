#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.MongoDB.Options;

public sealed record CredentialsOptions {
    public static CredentialsOptions Default => new();

    public string Database { get; set; } = Internals.Constants.MONGO_CRED_DATABASE;

    public string Mechanism { get; set; } = Internals.Constants.MONGO_CRED_MECHANISM;

    public string Username { get; set; } = Internals.Constants.MONGO_CRED_USER;

    public string Password { get; set; } = Internals.Constants.MONGO_CRED_PASS;

#if NET6_0_OR_GREATER
        [MemberNotNullWhen(returnValue: true, nameof(Database), nameof(Mechanism), nameof(Username), nameof(Password))]
#endif
    public bool UseCredentials => !string.IsNullOrWhiteSpace(Database) &&
                                  !string.IsNullOrWhiteSpace(Mechanism) &&
                                  !string.IsNullOrWhiteSpace(Username) &&
                                  !string.IsNullOrWhiteSpace(Password);
}