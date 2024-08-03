using System.ComponentModel;
using System.Reflection;

namespace Nameless {
    /// <summary>
    /// <see cref="PropertyInfo"/> extension methods.
    /// </summary>
    public static class PropertyInfoExtension {
        #region Public Static Methods

        /// <summary>
        /// If the property has a <see cref="DescriptionAttribute"/>, retrieves it description.
        /// </summary>
        /// <param name="self">The property.</param>
        /// <param name="fallbackToName">Whether returns the property name if description was not found.</param>
        /// <returns>The description, if exists.</returns>
        /// <exception cref="NullReferenceException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string GetDescription(this PropertyInfo self, bool fallbackToName = true) {
            var attr = self.GetCustomAttribute<DescriptionAttribute>();

            return attr is not null
                ? attr.Description
                : fallbackToName
                    ? self.Name
                    : string.Empty;
        }

    #endregion
    }
}
