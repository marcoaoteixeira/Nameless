using System.Globalization;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Mockers;

namespace Nameless.Localization.Json.Mockers;

public class CultureContextMocker : MockerBase<ICultureContext> {
    public CultureContextMocker WithCulture(CultureInfo culture) {
        InnerMock.Setup(mock => mock.GetCurrentCulture())
                    .Returns(culture);

        return this;
    }
}
