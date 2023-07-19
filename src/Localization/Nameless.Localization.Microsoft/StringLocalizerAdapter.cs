using MS_IStringLocalizer = Microsoft.Extensions.Localization.IStringLocalizer;
using MS_LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace Nameless.Localization.Microsoft {

    public sealed class StringLocalizerAdapter : MS_IStringLocalizer {

        #region Private Read-Only Fields

        private readonly IStringLocalizer _localizer;

        #endregion

        #region Public Constructors

        public StringLocalizerAdapter(IStringLocalizer localizer) {
            Prevent.Against.Null(localizer, nameof(localizer));

            _localizer = localizer;
        }

        #endregion

        #region MS_IStringLocalizer Members

        public MS_LocalizedString this[string name] => _localizer[name].ToLocalizedString();

        public MS_LocalizedString this[string name, params object[] arguments] => _localizer[name, args: arguments].ToLocalizedString();

        public IEnumerable<MS_LocalizedString> GetAllStrings(bool includeParentCultures) {
            var items = _localizer.List(includeParentCultures);
            foreach (var item in items) {
                yield return item.ToLocalizedString();
            }
        }

        #endregion
    }
}
