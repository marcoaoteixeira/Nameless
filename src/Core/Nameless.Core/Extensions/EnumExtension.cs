using System.ComponentModel;
using System.Reflection;

namespace Nameless {
    /// <summary>
    /// <see cref="Enum"/> extension methods.
    /// </summary>
    public static class EnumExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the attributes from an <see cref="Enum"/>.
        /// </summary>
        /// <param name="self">The <see cref="Enum"/> value.</param>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The <typeparamref name="TAttribute" /> annotated in the enum.</returns>
        public static TAttribute? GetAttribute<TAttribute>(this Enum self) where TAttribute : Attribute
            => self.GetType()
                   .GetField(self.ToString())?
                   .GetCustomAttribute<TAttribute>(inherit: false);

        /// <summary>
        /// Gets the enumerator description, if exists.
        /// </summary>
        /// <param name="self">The self enumerator.</param>
        /// <returns>The enumerator description.</returns>
        public static string GetDescription(this Enum self) {
            var attr = GetAttribute<DescriptionAttribute>(self);

            return attr is not null ? attr.Description : self.ToString();
        }

        #endregion
    }
}