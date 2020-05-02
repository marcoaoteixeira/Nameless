using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Nameless.FileProvider;
using Nameless.Logging;

namespace Nameless.Localization.Json {
    public sealed class StringLocalizerFactory : IStringLocalizerFactory {
        #region Private Static Read-Only Fields

        private static readonly IDictionary<string, IStringLocalizer> Cache = new Dictionary<string, IStringLocalizer> ();

        #endregion

        #region Private Read-Only Fields

        private readonly IFileProvider _fileProvider;
        private readonly IPluralizationRuleProvider _pluralizationRuleProvider;
        private readonly LocalizationSettings _settings;

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        public StringLocalizerFactory (IFileProvider fileProvider, IPluralizationRuleProvider pluralizationRuleProvider, LocalizationSettings settings) {
            Prevent.ParameterNull (fileProvider, nameof (fileProvider));
            Prevent.ParameterNull (pluralizationRuleProvider, nameof (pluralizationRuleProvider));
            Prevent.ParameterNull (settings, nameof (settings));

            _fileProvider = fileProvider;
            _pluralizationRuleProvider = pluralizationRuleProvider;
            _settings = settings;
        }

        #endregion

        #region Private Static Methods

        private static string GetFilePath (string resourcesBaseFolder, string baseName, string location, string cultureName) {
            var basePath = Path.Combine (baseName, $"{location}.{cultureName}.json");
            var possiblePaths = LocalizationUtils.ExpandPath (resourcesBaseFolder, basePath).ToArray ();
            foreach (var possiblePath in possiblePaths) {
                if (File.Exists (possiblePath)) { return possiblePath; }
            }
            return null;
        }

        private static int DefaultPluralRule (int count) {
            return count > 1 ? 1 : 0;
        }

        #endregion

        #region IStringLocalizerProvider Members

        public IStringLocalizer Create (Type resourceType, string culture = null) => Create (resourceType.Namespace, resourceType.Name, culture);

        public IStringLocalizer Create (string baseName, string location, string culture = null) {
            var cultureName = culture ?? CultureInfo.CurrentUICulture.Name;
            if (string.IsNullOrWhiteSpace (cultureName)) { cultureName = _settings.DefaultCultureName; }

            var path = GetFilePath (_settings.ResourcesBaseFolder, baseName, location, cultureName);

            if (path == null) {
                Logger.Error ($"Couldn't find file for {location} and culture {cultureName}.");
                return NullStringLocalizer.Instance;
            }

            if (!Cache.ContainsKey (path)) {
                _pluralizationRuleProvider.TryGet (new CultureInfo (cultureName), out PluralizationRuleDelegate rule);

                Cache.Add (
                    key: path,
                    value: new StringLocalizer (_fileProvider, baseName, location, cultureName, rule ?? DefaultPluralRule, path)
                );
            }
            return Cache[path];
        }

        #endregion
    }
}