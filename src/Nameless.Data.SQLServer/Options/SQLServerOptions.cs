#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Nameless.Data.SQLServer.Options;

public sealed record SQLServerOptions {
    public static SQLServerOptions Default => new();

    public string Server { get; set; } = "(localdb)\\MSSQLLocalDB";
        
    public string Database { get; set; } = "master";
        
    public string? Username { get; set; }
        
    public string? Password { get; set; }
        
    public bool UseAttachedDb { get; set; }
        
    public bool UseIntegratedSecurity { get; set; }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen(returnValue: true, nameof(Username), nameof(Password))]
#endif
    public bool UseCredentials
        => !string.IsNullOrWhiteSpace(Username) &&
           !string.IsNullOrWhiteSpace(Password) &&
           !UseIntegratedSecurity;

    public string GetConnectionString() {
        var connStr = string.Empty;

        connStr += $"Server={Server};";

        connStr += UseAttachedDb
            ? $"AttachDbFilename={Database};"
            : $"Database={Database};";

        connStr += UseCredentials
            ? $"User Id={Username};Password={Password};"
            : string.Empty;

        connStr += UseIntegratedSecurity
            ? "Integrated Security=true;"
            : string.Empty;

        return connStr;
    }
}