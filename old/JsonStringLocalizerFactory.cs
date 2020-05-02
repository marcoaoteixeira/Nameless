using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Localization;
using Nameless.Caching;
using Nameless.Environment;
using Nameless.Logging;

namespace Nameless.Localization.Json {
    public class JsonStringLocalizerFactory : IStringLocalizerFactory {
        #region Private Read-Only Fields

        private readonly ICache _cache;
        private readonly IHostingEnvironment _environment;
        private readonly LocalizationSettings _settings;

        #endregion

        #region Private Fields

        private string _resourcesFolder;

        #endregion

        #region Public Properties

        private ILogger _logger;
        public ILogger Logger {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        #endregion

        #region Public Constructors

        public JsonStringLocalizerFactory (ICache cache, IHostingEnvironment environment, LocalizationSettings settings) {
            Prevent.ParameterNull (cache, nameof (cache));
            Prevent.ParameterNull (environment, nameof (environment));
            Prevent.ParameterNull (settings, nameof (settings));

            _cache = cache;
            _environment = environment;
            _settings = settings;

            Initialize ();
        }

        #endregion

        #region Private Methods

        private void Initialize () {
            _resourcesFolder = Path.Combine (
                _environment.ApplicationBasePath,
                _settings.ResourcesFolderRelativePath
            );
        }

        #endregion

        #region Private Methods

        // Gets the first occurrence of the resource file.
        private string GetResourceLocation (string basePath, string resourceName, string cultureName) {
            var possibleResourceNames = LocalizationUtils.ExpandPath (resourceName);
            var cultures = LocalizationUtils.GetCultures (new CultureInfo (cultureName));
            foreach (var possibleResourceName in possibleResourceNames) {
                foreach (var culture in cultures) {
                    if (ResourceFileExists (basePath, possibleResourceName, cultureName)) {
                        return Path.Combine (basePath, possibleResourceName);
                    }
                }
            }
            Logger.Warning ($"Resource file not found for resource named: {resourceName}");
            return null;
        }

        private bool ResourceFileExists (string basePath, string resourceName, string cultureName = null) {
            var path = !string.IsNullOrWhiteSpace (cultureName)
                ? Path.Combine (basePath, $"{resourceName}.{cultureName}.json")
                : Path.Combine (basePath, $"{resourceName}.json");
            return File.Exists (path);
        }

        private IStringLocalizer GetStringLocalizer (string basePath, string resourceName, string cultureName) {
            var cacheKey = $"[{cultureName}]: {basePath} | {resourceName}";

            if (!_cache.IsTracked (cacheKey)) {
                var resourceLocation = GetResourceLocation (basePath, resourceName, cultureName);
                var resourceFilePath = JsonStringLocalizer.FormatResourceFilePath (resourceLocation, cultureName);

                if (resourceLocation == null) { return NullStringLocalizer.Instance; }

                _cache.Set (
                    key: cacheKey,
                    obj: new JsonStringLocalizer (resourceLocation, cultureName),
                    evictionCallback: null,
                    dependency: new FileCacheDependency (resourceFilePath)
                );
            }
            return (JsonStringLocalizer)_cache.Get (cacheKey);
        }

        #endregion

        #region IStringLocalizerFactory Members

        public IStringLocalizer Create (Type resourceSource) {
            Prevent.ParameterNull (resourceSource, nameof (resourceSource));

            return GetStringLocalizer (
                basePath: _resourcesFolder,
                resourceName: resourceSource.FullName,
                cultureName: (_settings.ForceCulture ?? CultureInfo.CurrentUICulture.Name)
            );
        }

        public IStringLocalizer Create (string baseName, string location) {
            Prevent.ParameterNull (baseName, nameof (baseName));
            Prevent.ParameterNullOrWhiteSpace (location, nameof (location));

            return GetStringLocalizer (
                basePath: Path.Combine (_resourcesFolder, baseName),
                resourceName: location,
                cultureName: (_settings.ForceCulture ?? CultureInfo.CurrentUICulture.Name)
            );
        }

        #endregion
    }
}