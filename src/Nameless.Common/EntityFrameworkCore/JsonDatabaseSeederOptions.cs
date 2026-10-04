using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     JSON database seeder options.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.PocoStructure)]
public class JsonDatabaseSeederOptions {
    /// <summary>
    ///     Gets or sets the JSON resource path.
    /// </summary>
    /// <remarks>
    ///
    /// </remarks>
    public required string RelativePath { get; set; }

    /// <summary>
    ///     Whether it should throw <see cref="MissingDatabaseSeederResourceException"/>
    ///     when JSON resource is missing.
    /// </summary>
    public bool ThrowOnMissing { get; set; }

    /// <summary>
    ///     Gets or sets the serializer options.
    /// </summary>
    public JsonSerializerOptions JsonOptions { get; set; } = new() {
        Converters = { new JsonStringEnumConverter() }
    };
}

/// <summary>
///     Missing Database Seeder Resource Exception
/// </summary>
public class MissingDatabaseSeederResourceException : Exception {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="MissingDatabaseSeederResourceException"/> class.
    /// </summary>
    /// <param name="path">
    ///     The resource path.
    /// </param>
    public MissingDatabaseSeederResourceException(string path)
        : base($"Unable to locate database seeder resource '{path}'.", innerException: null) { }
}