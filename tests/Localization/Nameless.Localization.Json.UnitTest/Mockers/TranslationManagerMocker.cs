using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;
using Nameless.Mockers;

namespace Nameless.Localization.Json.Mockers;
public class TranslationManagerMocker : MockerBase<ITranslationManager> {
    public TranslationManagerMocker WithTranslation(Translation translation) {
        InnerMock
            .Setup(mock => mock.GetTranslation(translation.Culture))
            .Returns(translation);

        return this;
    }
}
