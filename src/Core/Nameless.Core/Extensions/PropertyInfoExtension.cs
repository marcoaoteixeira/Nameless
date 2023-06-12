using System.ComponentModel;
using System.Reflection;

namespace Nameless {

    /// <summary>
    /// Extension methods for <see cref="PropertyInfo"/>
    /// </summary>
    public static class PropertyInfoExtension {

        #region Public Static Methods

        /// <summary>
        /// If the property has a <see cref="DescriptionAttribute"/>, retrieves it description.
        /// </summary>
        /// <param name="self">The property.</param>
        /// <returns>The description, if exists.</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="self"/> is <c>null</c>.</exception>
        public static string? GetDescription(this PropertyInfo self) {
            Prevent.Null(self, nameof(self));

            var attr = self.GetCustomAttribute<DescriptionAttribute>(inherit: false);
            return attr?.Description;
        }

        #endregion
    }
}
