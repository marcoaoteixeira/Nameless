using System.Globalization;

namespace Nameless.Localization.Json.Infrastructure;

public interface ICultureContext {
    CultureInfo GetCurrentCulture();
}