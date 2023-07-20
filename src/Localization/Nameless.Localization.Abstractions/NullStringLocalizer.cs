namespace Nameless.Localization {
    [Singleton]
    public sealed class NullStringLocalizer : IStringLocalizer {
        #region Private Static Read-Only Fields

        private static readonly NullStringLocalizer _instance = new();

        #endregion

        #region Public Static Properties

        /// <summary>
        /// Gets the unique instance of Localizer.
        /// </summary>
        public static IStringLocalizer Instance => _instance;

        #endregion

        #region Static Constructors

        // Explicit static constructor to tell the C# compiler
        // not to mark type as beforefieldinit
        static NullStringLocalizer() { }

        #endregion

        #region Private Constructors

        private NullStringLocalizer() { }

        #endregion

        #region IStringLocalizer Members

        public LocaleString this[string text, params object[] args]
            => new(Thread.CurrentThread.CurrentUICulture, text, text, args);

        public IEnumerable<LocaleString> List(bool includeParentCultures)
            => Enumerable.Empty<LocaleString>();

        #endregion
    }
}