using System;
using System.Globalization;
using Nameless.Localization.Json.Schemas;

namespace Nameless.Localization.Json {
    /// <summary>
    /// Holds all mechanics to create string localizers.
    /// For performance sake, use only one instance of this class
    /// through the application life time.
    /// </summary>
    public sealed class StringLocalizerFactory : IStringLocalizerFactory {

        #region Private Read-Only Fields

        private readonly IMessageCollectionAggregationProvider _messageCollectionAggregationProvider;
        private readonly IPluralizationRuleProvider _pluralizationRuleProvider;

        #endregion

        #region Public Constructors

        public StringLocalizerFactory (IMessageCollectionAggregationProvider messageCollectionAggregationProvider, IPluralizationRuleProvider pluralizationRuleProvider) {
            Prevent.ParameterNull (messageCollectionAggregationProvider, nameof (messageCollectionAggregationProvider));
            Prevent.ParameterNull (pluralizationRuleProvider, nameof (pluralizationRuleProvider));

            _messageCollectionAggregationProvider = messageCollectionAggregationProvider;
            _pluralizationRuleProvider = pluralizationRuleProvider;
        }

        #endregion

        #region IStringLocalizerProvider Members

        public IStringLocalizer Create (Type resourceType, string cultureName = null) => Create (resourceType.Assembly.GetName ().Name, resourceType.FullName, cultureName);

        public IStringLocalizer Create (string baseName, string location, string cultureName = null) {
            cultureName = cultureName.OnBlank (CultureInfo.CurrentUICulture.Name);

            var messageCollectionName = $"[{baseName}] {location}";
            var aggregation = _messageCollectionAggregationProvider.Create (cultureName);
            if (aggregation == null || !aggregation.TryGetMessageCollection (messageCollectionName, out MessageCollection messageCollection)) {
                return EmptyStringLocalizer.Create (baseName, location, cultureName);
            }

            if (!_pluralizationRuleProvider.TryGet (new CultureInfo (cultureName), out PluralizationRuleDelegate pluralizationRule)) {
                pluralizationRule = PluralizationRuleProvider.DefaultRule;
            }

            return new StringLocalizer (baseName, location, cultureName, messageCollection, pluralizationRule);
        }

        #endregion
    }
}