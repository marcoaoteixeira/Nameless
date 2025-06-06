using System.Reflection;

namespace Nameless.Validation.FluentValidation;

/// <summary>
///     Validation registration options.
/// </summary>
public record ValidationOptions {
    /// <summary>
    ///     Gets an array of assemblies to scan for validators.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];
}