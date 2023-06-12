using MS_IStringLocalizer = Microsoft.Extensions.Localization.IStringLocalizer;
using MS_LocalizedString = Microsoft.Extensions.Localization.LocalizedString;

namespace Nameless.Localization.Microsoft {

    [Singleton]
    public sealed class NullStringLocalizer : MS_IStringLocalizer {

        #region Private Static Read-Only Fields

        private static readonly MS_IStringLocalizer _instance = new NullStringLocalizer();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of Localizer.
        /// </summary>
        public static MS_IStringLocalizer Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullStringLocalizer() { }

        #endregion

        #region Private Constructors

        private NullStringLocalizer() { }

        #endregion

        #region MS_NullStringLocalizer Members

        public MS_LocalizedString this[string name] => new(name, name);

        public MS_LocalizedString this[string name, params object[] arguments] => new(name, string.Format(name, arguments));

        public IEnumerable<MS_LocalizedString> GetAllStrings(bool includeParentCultures) => Array.Empty<MS_LocalizedString>();

        #endregion
    }
}
