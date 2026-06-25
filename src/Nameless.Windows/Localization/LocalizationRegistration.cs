using Nameless.Registration;

namespace Nameless.Windows.Localization;

public class LocalizationRegistration : AssemblyScanAware<LocalizationRegistration> {
    public Type? Localizer { get; private set; }

    public LocalizationRegistration SetLocalizer(Type type) {
        Throws.When.Null(type);

        Localizer = type;

        return this;
    }
}