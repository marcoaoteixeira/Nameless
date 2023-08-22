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
        /// <param name="inherited">Marks as inherited attributes.</param>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The <typeparamref name="TAttribute" /> annotated in the enum.</returns>
        public static TAttribute? GetAttribute<TAttribute>(this Enum self, bool inherited = false) where TAttribute : Attribute {
            var field = self.GetType().GetField(self.ToString());

            if (field is null) { return null; }

            return field.GetCustomAttribute<TAttribute>(inherit: inherited);
        }

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