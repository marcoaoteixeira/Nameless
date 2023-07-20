using System.Globalization;
using Nameless.Helpers;
using Nameless.Localization.Json.Objects.Translation;

namespace Nameless.Localization.Json.Services {
    public static class TranslationProviderExtension {
        #region Public Static Methods

        public static Trunk Get(this ITranslationProvider self, CultureInfo culture)
            => AsyncHelper.RunSync(() => self.GetAsync(culture));

        #endregion
    }
}
