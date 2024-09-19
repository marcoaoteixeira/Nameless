using System.Reflection;

namespace Nameless.Validation.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ValidateAttribute : Attribute {
    public static bool IsPresent(object? obj)
        => obj?.GetType()
              .GetCustomAttribute<ValidateAttribute>(inherit: false) is not null;
}