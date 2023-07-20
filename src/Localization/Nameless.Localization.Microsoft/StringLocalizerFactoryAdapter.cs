namespace Nameless.Localization.Microsoft {
    public sealed class StringLocalizerFactoryAdapter : IMSStringLocalizerFactory {
        #region Private Read-Only Fields

        private readonly IStringLocalizerFactory _factory;

        #endregion

        #region Public Constructors

        public StringLocalizerFactoryAdapter(IStringLocalizerFactory factory) {
            _factory = Prevent.Against.Null(factory, nameof(factory));
        }

        #endregion

        #region IStringLocalizerFactory Members

        public IMSStringLocalizer Create(Type resourceSource)
            => new StringLocalizerAdapter(_factory.Create(resourceSource));

        public IMSStringLocalizer Create(string baseName, string location)
            => new StringLocalizerAdapter(_factory.Create(baseName, location));

        #endregion
    }
}
