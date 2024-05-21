using System.Globalization;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Microsoft.Json.Infrastructure;
using Nameless.Localization.Microsoft.Json.Objects;

namespace Nameless.Localization.Microsoft.Json {
    public sealed class StringLocalizerFactory : IStringLocalizerFactory {
        #region Private Read-Only Fields

        private readonly ICultureContext _cultureContext;
        private readonly ITranslationManager _translationManager;

        #endregion

        #region Public Constructors

        public StringLocalizerFactory(ICultureContext cultureContext, ITranslationManager translationManager) {
            _cultureContext = Guard.Against.Null(cultureContext, nameof(cultureContext));
            _translationManager = Guard.Against.Null(translationManager, nameof(translationManager));
        }

        #endregion

        #region Private Methods

        private Region GetRegion(CultureInfo culture, string resourceName, string resourcePath) {
            var key = $"[{resourceName}] {resourcePath}";
            var translation = _translationManager.GetTranslation(culture.Name);

            return translation.TryGetRegion(key, out var region)
                ? region
                : new Region(Name: key, Messages: []);
        }

        private StringLocalizer GetLocalizer(CultureInfo culture, string resourceName, string resourcePath) {
            var region = GetRegion(culture, resourceName, resourcePath);

            return new StringLocalizer(culture, resourceName, resourcePath, region, GetLocalizer);
        }

        #endregion

        #region IStringLocalizerFactory Members

        public IStringLocalizer Create(Type resourceSource)
            => GetLocalizer(_cultureContext.GetCurrentCulture(), resourceSource.Assembly.GetName().Name!, resourceSource.FullName!);

        public IStringLocalizer Create(string baseName, string location)
            => GetLocalizer(_cultureContext.GetCurrentCulture(), baseName, location);

        #endregion
    }
}
