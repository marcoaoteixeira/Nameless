namespace Nameless.Localization {
    public interface IStringLocalizer {
        #region Properties

        LocaleString this[string text, params object[] args] { get; }

        #endregion

        #region Methods

        IEnumerable<LocaleString> List(bool includeParentCultures = false);

        #endregion
    }
}