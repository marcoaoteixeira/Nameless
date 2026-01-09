namespace Nameless.Validation;

/// <summary>
///     Attributes that indicates that a class is subject to validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ValidateAttribute : Attribute {
    /// <summary>
    ///     Checks if <paramref name="obj" /> is annotated
    ///     with <see cref="ValidateAttribute" />.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="obj" /> is annotated;
    ///     otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsPresent(object? obj) {
        return obj is not null && obj.GetType().HasAttribute<ValidateAttribute>();
    }
}