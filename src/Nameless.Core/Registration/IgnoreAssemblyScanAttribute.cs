using System.Reflection;

namespace Nameless.Registration;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class IgnoreAssemblyScanAttribute : Attribute {
    public static bool IsNotPresent(Type type) {
        return type.GetCustomAttribute<IgnoreAssemblyScanAttribute>(inherit: false) is null;
    }
}
