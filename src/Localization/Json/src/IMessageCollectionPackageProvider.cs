using Nameless.Localization.Json.Schemas;

namespace Nameless.Localization.Json {
    public interface IMessageCollectionPackageProvider {
        #region Methods

        MessageCollectionPackage Create (string cultureName);

        #endregion
    }
}