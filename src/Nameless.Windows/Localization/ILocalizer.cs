using System.ComponentModel;
using System.Globalization;

namespace Nameless.Windows.Localization;

/// <summary>
///     Contract for a runtime-switchable localization provider.
/// </summary>
public interface ILocalizer : INotifyPropertyChanged {
    /// <summary>
    ///     Returns the localized string for <paramref name="key"/> using the
    ///     current culture. Returns the key itself when no match is found.
    /// </summary>
    /// <param name="key">
    ///     The resource key.
    /// </param>
    string this[string key] { get; }

    /// <summary>
    ///     Returns the localized string for <paramref name="key"/> formatted
    ///     with <paramref name="parameters"/>. Returns the key itself when no
    ///     match is found.
    /// </summary>
    /// <param name="key">
    ///     The resource key.
    /// </param>
    /// <param name="parameters">
    ///     Format arguments applied to the translated string.
    /// </param>
    string this[string key, params object[] parameters] { get; }

    /// <summary>
    ///     Gets the currently active culture.
    /// </summary>
    CultureInfo CurrentCulture { get; }

    /// <summary>
    ///     Switches the active culture and notifies all bound controls to
    ///     refresh.
    /// </summary>
    /// <param name="culture">
    ///     The culture to activate.
    /// </param>
    void SetCulture(CultureInfo culture);

    /// <summary>
    ///     Switches the active culture by name (e.g. <c>"pt-BR"</c>) and
    ///     notifies all bound controls to refresh.
    /// </summary>
    /// <param name="cultureName">
    ///     BCP 47 culture tag.
    /// </param>
    void SetCulture(string cultureName);
}