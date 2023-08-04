using System.Globalization;

namespace Nameless.Localization.Microsoft.Json.Infrastructure {
    public interface ICultureContext {
        #region Methods

        CultureInfo GetCurrentCulture();

        #endregion
    }
}
