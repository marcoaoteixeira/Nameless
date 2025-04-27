using System.Reflection;

namespace Nameless.Validation;

/// <summary>
/// Validation attribute to use in conjunction with <see cref="IValidationService"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ValidateAttribute : Attribute {
    /// <summary>
    /// Checks if <paramref name="obj"/> is annotated with <see cref="ValidateAttribute"/>.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    /// <c>true</c> if <paramref name="obj"/> is annotated; otherwise <c>false</c>
    /// </returns>
    public static bool IsPresent(object? obj)
        => obj?.GetType()
              .GetCustomAttribute<ValidateAttribute>() is not null;
}