using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json.Infrastructure;

public interface ITranslationManager {
    Translation GetTranslation(string culture);
}