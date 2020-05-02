using System.Collections.Generic;
using System.Globalization;

namespace Nameless {
    public static class CultureInfoExtension {
        #region Public Static Methods

        public static IEnumerable<CultureInfo> GetTree (this CultureInfo self) {
            var currentCulture = new CultureInfo (self.Name);
            while (currentCulture.Name != string.Empty) {
                yield return currentCulture;
                currentCulture = currentCulture.Parent;
            }
        }

        #endregion
    }
}