using System.Globalization;
using Nameless.Helpers;
using Nameless.Localization.Json.Schema;

namespace Nameless.Localization.Json {

    public static class TranslationProviderExtension {

        #region Public Static Methods

        public static Translation Get(this ITranslationProvider self, CultureInfo culture) {
            Prevent.Against.Null(self, nameof(self));

            return AsyncHelper.RunSync(() => self.GetAsync(culture));
        }

        #endregion
    }
}
