using System.ComponentModel;
using System.Reflection;

namespace Nameless;

/// <summary>
///     <see cref="Enum" /> extension methods.
/// </summary>
public static class EnumExtensions {
    /// <param name="self">The <see cref="Enum" /> value.</param>
    extension(Enum self) {
        /// <summary>
        ///     Retrieves an attributes from the current <see cref="Enum" />.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The <typeparamref name="TAttribute" /> annotated in the enum.</returns>
        public TAttribute? GetAttribute<TAttribute>()
            where TAttribute : Attribute {
            return self.GetType()
                       .GetField(self.ToString())?
                       .GetCustomAttribute<TAttribute>(inherit: false);
        }

        /// <summary>
        ///     If the current enum has a <see cref="DescriptionAttribute" />, it will
        ///     return the <see cref="DescriptionAttribute.Description" /> value or
        ///     <see cref="string.Empty" /> if it's not present.
        /// </summary>
        /// <param name="fallbackToString">
        ///     Whether it should return the string representation of the enum if
        ///     there is no description for it. No description means
        ///     <see cref="DescriptionAttribute" /> is not present or its
        ///     <see cref="DescriptionAttribute.Description" /> is <see langword="null"/> or
        ///     white spaces.
        /// </param>
        /// <returns>The enum description or its name.</returns>
        public string GetDescription(bool fallbackToString = true) {
            var attr = GetAttribute<DescriptionAttribute>(self);

            if (attr is not null) {
                return !string.IsNullOrWhiteSpace(attr.Description)
                    ? attr.Description
                    : fallbackToString
                        ? self.ToString()
                        : string.Empty;
            }

            return fallbackToString ? self.ToString() : string.Empty;
        }
    }
}