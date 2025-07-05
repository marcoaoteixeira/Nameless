using System.Globalization;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Testing.Tools.Mockers;

namespace Nameless.Localization.Json.Mockers;

public class CultureProviderMocker : Mocker<ICultureProvider> {
    public CultureProviderMocker WithCulture(CultureInfo culture) {
        MockInstance
           .Setup(mock => mock.GetCurrentCulture())
           .Returns(culture);

        return this;
    }
}