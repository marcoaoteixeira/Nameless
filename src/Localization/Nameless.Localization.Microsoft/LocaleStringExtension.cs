using MS_LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace Nameless.Localization.Microsoft {

    public static class LocaleStringExtension {

        #region Public Static Methods

        public static MS_LocalizedString ToLocalizedString(this LocaleString self) {
            Prevent.Against.Null(self, nameof(self));

            return new MS_LocalizedString(self.Text, self.GetTranslation());
        }

        #endregion
    }
}
