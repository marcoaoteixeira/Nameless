using System.Reflection;

namespace Nameless.WPF.Helpers;

public static class ObjectHelper {
    public static Dictionary<string, object?> Transform(object? obj) {
        if (obj is null) { return [];}

        var result = new Dictionary<string, object?>();

        var properties = obj.GetType()
                            .GetProperties(
                                BindingFlags.Instance |
                                BindingFlags.Public
                            );

        foreach (var property in properties) {
            result[property.Name] = property.GetValue(obj);
        }

        return result;
    }
}
