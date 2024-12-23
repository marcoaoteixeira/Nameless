using System.Globalization;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Mockers;

namespace Nameless.Localization.Json.Mockers;

public class CultureProviderMocker : MockerBase<ICultureProvider> {
    public CultureProviderMocker WithCulture(CultureInfo culture) {
        Mock.Setup(mock => mock.GetCurrentCulture())
                    .Returns(culture);

        return this;
    }
}
