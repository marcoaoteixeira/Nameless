using System;

namespace Nameless.Persistence.NHibernate {
    internal static class TypeExtension {
        #region Internal Static Methods

        internal static string GetQualifiedName (this Type self) => self != null ? $"{self.FullName}, {self.Assembly.GetName ().Name}" : null;

        #endregion
    }
}