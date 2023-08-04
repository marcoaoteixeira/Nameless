using Nameless.Localization.Microsoft.Json.Objects;

namespace Nameless.Localization.Microsoft.Json.Infrastructure {
    public interface ITranslationManager {
        #region Methods

        Translation GetTranslation(string culture);

        #endregion
    }
}
