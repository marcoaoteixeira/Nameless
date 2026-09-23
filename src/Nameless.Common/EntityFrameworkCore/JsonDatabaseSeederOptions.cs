using System.Diagnostics.CodeAnalysis;
using System.Reflection;
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
    ///     Whether it should use an embedded resource instead
    ///     of a physical file resource.
    /// </summary>
    public bool UseEmbeddedResource { get; set; }

    /// <summary>
    ///     Gets or sets the assembly where the resource resides.
    /// </summary>
    /// <remarks>
    ///     If <see langword="null"/>, then uses the executing assembly.
    /// </remarks>
    public Assembly? Assembly { get; set; }

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
    /// <param name="embedded">
    ///     Whether it is an embedded resource or a file.
    /// </param>
    public MissingDatabaseSeederResourceException(string path, bool embedded = false)
        : base($"Unable to locate {(embedded ? "embedded" : "file")} database seeder resource '{path}'.", innerException: null) { }
}