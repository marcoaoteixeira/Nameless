namespace Nameless.AspNetCore.Localization {
    public static class HtmlLocalizerExtension {
        #region Public Static Methods

        public static Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString Get (this Microsoft.AspNetCore.Mvc.Localization.IHtmlLocalizer self, string text, int count = -1, params object[] args) {
            if (self == null) { return null; }

            return self is HtmlLocalizer localizer ? localizer[text, count, args] : self[text, arguments : args];
        }

        #endregion
    }
}