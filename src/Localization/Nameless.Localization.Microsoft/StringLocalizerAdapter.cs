using Microsoft.Extensions.Localization;

namespace Nameless.Localization.Microsoft {
    public sealed class StringLocalizerAdapter : IMSStringLocalizer {
        #region Private Read-Only Fields

        private readonly IStringLocalizer _localizer;

        #endregion

        #region Public Constructors

        public StringLocalizerAdapter(IStringLocalizer localizer) {
            _localizer = Prevent.Against.Null(localizer, nameof(localizer));
        }

        #endregion

        #region MS_IStringLocalizer Members

        public LocalizedString this[string name]
            => _localizer[name].ToLocalizedString();

        public LocalizedString this[string name, params object[] arguments]
            => _localizer[name, args: arguments].ToLocalizedString();

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
            var items = _localizer.List(includeParentCultures);
            foreach (var item in items) {
                yield return item.ToLocalizedString();
            }
        }

        #endregion
    }
}
