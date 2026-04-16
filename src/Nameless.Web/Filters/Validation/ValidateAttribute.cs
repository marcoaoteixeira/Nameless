using System.Reflection;

namespace Nameless.Web.Filters.Validation;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ValidateAttribute : Attribute {
    public static bool IsPresent(object? instance) {
        return instance is not null && IsPresent(instance.GetType());
    }

    public static bool IsPresent(Type type) {
        return type.GetCustomAttribute<ValidateAttribute>() is not null;
    }
}
