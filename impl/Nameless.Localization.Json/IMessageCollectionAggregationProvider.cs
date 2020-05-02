using Nameless.Localization.Json.Schemas;

namespace Nameless.Localization.Json {
    public interface IMessageCollectionAggregationProvider {
        #region Methods

        MessageCollectionAggregation Create (string cultureName);

        #endregion
    }
}