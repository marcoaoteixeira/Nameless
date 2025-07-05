using System.Reflection;

namespace Nameless.MongoDB;

/// <summary>
/// Options for configuring the MongoDB connection.
/// </summary>
public sealed record MongoOptions {
    /// <summary>
    /// Gets or sets the assemblies to scan for document mappers.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];

    /// <summary>
    /// Gets or sets the host for the MongoDB connection.
    /// Default is "localhost".
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the port for the MongoDB connection.
    /// Default is 27017.
    /// </summary>
    public int Port { get; set; } = 27017;

    /// <summary>
    /// Gets or sets the name of the database to connect to. 
    /// </summary>
    public string? DatabaseName { get; set; }

    /// <summary>
    /// Gets or sets the credentials settings for the MongoDB connection.
    /// </summary>
    public CredentialsSettings Credentials { get; set; } = new();
}