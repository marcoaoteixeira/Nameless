using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Nameless {
    public static class EnumeratorExtension {
        #region Public Static Methods

        /// <summary>
        /// Retrieves the attributes from a enumator.
        /// </summary>
        /// <param name="self">The self enumerator.</param>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>A collection of attributes of type <typeparamref name="TAttribute" /></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute> (this Enum self) where TAttribute : Attribute {
            var attrs = self
                .GetType ()
                .GetField (self.ToString ())
                .GetCustomAttributes<TAttribute> (inherit: false);
            return attrs;
        }

        /// <summary>
        /// Gets the enumerator description, if exists.
        /// </summary>
        /// <param name="self">The self enumerator.</param>
        /// <returns>The enumerator description.</returns>
        public static string GetDescription (this Enum self) {
            var attr = GetAttributes<DescriptionAttribute> (self).SingleOrDefault ();
            return attr != null ? attr.Description : self.ToString ();
        }

        #endregion
    }
}