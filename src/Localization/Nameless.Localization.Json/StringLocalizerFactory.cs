using System.Globalization;
using Nameless.Localization.Json.Objects.Translation;
using Nameless.Localization.Json.Services;

namespace Nameless.Localization.Json
{
    /// <summary>
    /// Holds all mechanics to create string localizers.
    /// For performance sake, use only one instance of this class
    /// through the application life time.
    /// </summary>
    public sealed class StringLocalizerFactory : IStringLocalizerFactory {
        #region Private Read-Only Fields

        private readonly ICultureContext _cultureContext;
        private readonly ITranslationProvider _translationProvider;

        #endregion

        #region Public Constructors

        public StringLocalizerFactory(ICultureContext cultureContext, ITranslationProvider translationProvider) {
            _cultureContext = Prevent.Against.Null(cultureContext, nameof(cultureContext));
            _translationProvider = Prevent.Against.Null(translationProvider, nameof(translationProvider));
        }

        #endregion

        #region Private Methods

        private Branch GetBranch(CultureInfo culture, string resourceName, string resourcePath) {
            var key = $"[{resourceName}] {resourcePath}";
            var translation = _translationProvider.Get(culture);

            return translation.TryGetValue(key, out var branch)
                ? branch
                : new(key);
        }

        private StringLocalizer GetLocalizer(CultureInfo culture, string resourceName, string resourcePath) {
            var container = GetBranch(culture, resourceName, resourcePath);

            return new StringLocalizer(culture, resourceName, resourcePath, container, GetLocalizer);
        }

        #endregion

        #region ILocalizerFactory Members

        /// <inheritdocs />
        /// <remarks>
        /// If we're talking about a resource named Something.Somewhere.Thing (Assembly name)
        /// and a resource string for the Thingable class, so, the <paramref name="resourceName"/>
        /// will be "Something.Somewhere.Thing" and the <paramref name="sourcePath"/> will be
        /// "Something.Somewhere.Thing.Thingable". Also, the key in the translation file will be
        /// "[Something.Somewhere.Thing] Something.Somewhere.Thing.Thingable"
        /// </remarks>
        public IStringLocalizer Create(Type resource)
            => GetLocalizer(_cultureContext.GetCurrentCulture(), resource.Assembly.GetName().Name!, resource.FullName!);

        /// <inheritdocs />
        /// <remarks>
        /// If we're talking about a resource named Something.Somewhere.Thing (Assembly name)
        /// and a resource string for the Thingable class, so, the <paramref name="resourceName"/>
        /// will be "Something.Somewhere.Thing" and the <paramref name="resourcePath"/> will be
        /// "Something.Somewhere.Thing.Thingable". Also, the key in the translation file will be
        /// "[Something.Somewhere.Thing] Something.Somewhere.Thing.Thingable"
        /// </remarks>
        public IStringLocalizer Create(string resourceName, string resourcePath)
            => GetLocalizer(_cultureContext.GetCurrentCulture(), resourceName, resourcePath);

        #endregion
    }
}
