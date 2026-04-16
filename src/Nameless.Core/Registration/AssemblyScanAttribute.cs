using System.Reflection;

namespace Nameless.Registration;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AssemblyScanAttribute : Attribute {
    public bool Ignore { get; set; }

    public static bool Include(Type type) {
        var attr = type.GetCustomAttribute<AssemblyScanAttribute>(inherit: false);

        if (attr is not null) {
            return !attr.Ignore;
        }

        return true;
    }
}
