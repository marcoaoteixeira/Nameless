using MS_IStringLocalizer = Microsoft.Extensions.Localization.IStringLocalizer;
using MS_IStringLocalizerFactory = Microsoft.Extensions.Localization.IStringLocalizerFactory;

namespace Nameless.Localization.Microsoft {

    public sealed class StringLocalizerFactoryAdapter : MS_IStringLocalizerFactory {

        #region Private Read-Only Fields

        private readonly IStringLocalizerFactory _factory;

        #endregion

        #region Public Constructors

        public StringLocalizerFactoryAdapter(IStringLocalizerFactory factory) {
            Prevent.Against.Null(factory, nameof(factory));

            _factory = factory;
        }

        #endregion

        #region MS_IStringLocalizerFactory Members

        public MS_IStringLocalizer Create(Type resourceSource) => new StringLocalizerAdapter(_factory.Create(resourceSource));

        public MS_IStringLocalizer Create(string baseName, string location) => new StringLocalizerAdapter(_factory.Create(baseName, location));

        #endregion
    }
}
