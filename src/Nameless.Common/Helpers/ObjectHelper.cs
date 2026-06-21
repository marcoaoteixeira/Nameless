using System.Reflection;

namespace Nameless.Helpers;

/// <summary>
///     <see cref="object"/> helper.
/// </summary>
public static class ObjectHelper {
    /// <summary>
    ///     Tries to transform an object into a dictionary by lookup all
    ///     public instance properties.
    /// </summary>
    /// <param name="obj">
    ///     The current object.
    /// </param>
    /// <returns>
    ///     A dictionary.
    /// </returns>
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
