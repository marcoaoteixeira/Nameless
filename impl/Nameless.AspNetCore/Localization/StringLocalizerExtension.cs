namespace Nameless.AspNetCore.Localization {
    public static class StringLocalizerExtension {
        #region Public Static Methods

        public static Microsoft.Extensions.Localization.LocalizedString Get (this Microsoft.Extensions.Localization.IStringLocalizer self, string text, int count = -1, params object[] args) {
            if (self == null) { return null; }

            return self is StringLocalizer localizer ? localizer[text, count, args] : self[text, arguments : args];
        }

        #endregion
    }
}