using Nameless.Registration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Fluent registration options for the localization subsystem.
/// </summary>
public class LocalizationRegistration : AssemblyScanAware<LocalizationRegistration> {
    /// <summary>
    ///     The concrete <see cref="ILocalizer"/> type to register.
    ///     <see langword="null"/> when assembly scanning resolves the type
    ///     instead.
    /// </summary>
    public Type? Localizer { get; private set; }

    /// <summary>
    ///     Explicitly sets the <see cref="ILocalizer"/> implementation type.
    ///     Overrides assembly scanning when called.
    /// </summary>
    /// <param name="type">
    ///     A concrete type that implements <see cref="ILocalizer"/>.
    /// </param>
    /// <returns>
    ///     This instance for fluent chaining.
    /// </returns>
    public LocalizationRegistration SetLocalizer(Type type) {
        Throws.When.Null(type);

        Localizer = type;

        return this;
    }
}