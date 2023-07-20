using Microsoft.Extensions.Localization;

namespace Nameless.Localization.Microsoft {
    public static class LocaleStringExtension {
        #region Public Static Methods

        public static LocalizedString ToLocalizedString(this LocaleString self)
            => new(self.Text, self.GetTranslation());

        #endregion
    }
}
