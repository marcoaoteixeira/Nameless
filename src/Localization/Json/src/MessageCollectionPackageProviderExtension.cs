using Nameless.Helpers;
using Nameless.Localization.Json.Schemas;

namespace Nameless.Localization.Json {
    internal static class MessageCollectionPackageProviderExtension {
        #region Internal Static Methods

        internal static MessageCollectionPackage Create (this IMessageCollectionPackageProvider self, string cultureName) {
            if (self == null) { return null; }

            return AsyncHelper.RunSync (() => self.CreateAsync (cultureName));
        }

        #endregion
    }
}