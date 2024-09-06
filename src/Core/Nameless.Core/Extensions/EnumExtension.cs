using System.ComponentModel;
using System.Reflection;

namespace Nameless;

/// <summary>
/// <see cref="Enum"/> extension methods.
/// </summary>
public static class EnumExtension {
    /// <summary>
    /// Retrieves an attributes from the current <see cref="Enum"/>.
    /// </summary>
    /// <param name="self">The <see cref="Enum"/> value.</param>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    /// <returns>The <typeparamref name="TAttribute" /> annotated in the enum.</returns>
    public static TAttribute? GetAttribute<TAttribute>(this Enum self)
        where TAttribute : Attribute {
        var field = self.GetType()
                        .GetField(self.ToString());

        return field?.GetCustomAttribute<TAttribute>(inherit: false);
    }

    /// <summary>
    /// If the current enum has a <see cref="DescriptionAttribute"/>, it will
    /// return the <see cref="DescriptionAttribute.Description"/> value or
    /// <see cref="string.Empty"/> if it's not present.
    /// </summary>
    /// <param name="self">The current enum.</param>
    /// <param name="fallbackToName">
    /// Whether it should return the name of the enum if there is no description for it.
    /// No description means <see cref="DescriptionAttribute"/> is not present or its
    /// <see cref="DescriptionAttribute.Description"/> is <c>null</c> or white spaces.
    /// </param>
    /// <returns>The enum description or its name.</returns>
    public static string GetDescription(this Enum self, bool fallbackToName = true) {
        var attr = GetAttribute<DescriptionAttribute>(self);

        if (attr is not null) {
            return !string.IsNullOrWhiteSpace(attr.Description)
                ? attr.Description
                : fallbackToName
                    ? self.ToString()
                    : string.Empty;
        }

        return fallbackToName ? self.ToString() : string.Empty;
    }
}