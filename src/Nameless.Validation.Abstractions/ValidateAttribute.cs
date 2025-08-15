namespace Nameless.Validation;

/// <summary>
///     Validation attribute to use in conjunction with <see cref="IValidationService" />.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ValidateAttribute : Attribute {
    /// <summary>
    ///     Checks if <paramref name="obj" /> is annotated with <see cref="ValidateAttribute" />.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="obj" /> is annotated; otherwise <see langword="false"/>
    /// </returns>
    public static bool IsPresent(object? obj) {
        return obj is not null && obj.GetType().HasAttribute<ValidateAttribute>();
    }
}
