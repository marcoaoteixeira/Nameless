namespace Nameless.AspNetCore.Localization {
    public sealed class HtmlLocalizer : Microsoft.AspNetCore.Mvc.Localization.HtmlLocalizer {
        #region Private Read-Only Fields

        private readonly Microsoft.Extensions.Localization.IStringLocalizer _localizer;

        #endregion

        #region Public Properties

        public Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString this [string name, int count = -1, params object[] args] {
            get {
                var localizedString = _localizer is StringLocalizer localizer ? localizer[name, count, args] : _localizer[name, arguments : args];
                return ToHtmlString (localizedString);
            }
        }

        #endregion

        #region Public Constructors

        public HtmlLocalizer (Microsoft.Extensions.Localization.IStringLocalizer localizer) : base (localizer) {
            Prevent.ParameterNull (localizer, nameof (localizer));

            _localizer = localizer;
        }

        #endregion

        #region Public Override Methods

        public override Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString this [string name] => ToHtmlString (_localizer[name]);

        public override Microsoft.AspNetCore.Mvc.Localization.LocalizedHtmlString this [string name, params object[] arguments] => ToHtmlString (_localizer[name], arguments);

        #endregion
    }
}