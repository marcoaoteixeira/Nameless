using System.ComponentModel;
using System.Reflection;

namespace Nameless;

/// <summary>
///     <see cref="PropertyInfo" /> extension methods.
/// </summary>
public static class PropertyInfoExtensions {
    /// <summary>
    ///     If the current property has a <see cref="DescriptionAttribute" />, it will
    ///     return the <see cref="DescriptionAttribute.Description" /> value or
    ///     <see cref="string.Empty" /> if it's not present.
    /// </summary>
    /// <param name="self">The current property.</param>
    /// <param name="fallbackToName">
    ///     Whether it should return the name of the property if there is no description for it.
    ///     No description means <see cref="DescriptionAttribute" /> is not present or its
    ///     <see cref="DescriptionAttribute.Description" /> is <c>null</c> or white spaces.
    /// </param>
    /// <returns>The property description or its name.</returns>
    public static string GetDescription(this PropertyInfo self, bool fallbackToName = true) {
        var attr = self.GetCustomAttribute<DescriptionAttribute>(false);

        if (attr is not null) {
            return !string.IsNullOrWhiteSpace(attr.Description)
                ? attr.Description
                : fallbackToName
                    ? self.Name
                    : string.Empty;
        }

        return fallbackToName ? self.Name : string.Empty;
    }
}