using Microsoft.Extensions.Localization;

namespace Nameless.Localization.Microsoft.Json {
    [Singleton]
    public sealed class NullStringLocalizer : IStringLocalizer {
        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of Localizer.
        /// </summary>
        public static IStringLocalizer Instance { get; } = new NullStringLocalizer();

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

        public LocalizedString this[string name]
            => new(name, name);

        public LocalizedString this[string name, params object[] arguments]
            => new(name, string.Format(name, arguments));

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => [];

        #endregion
    }
}
