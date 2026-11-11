using System.ComponentModel;
using System.Globalization;

namespace Nameless.Windows.Localization;

/// <summary>
///     Contract for a runtime-switchable localization provider.
/// </summary>
public interface ITranslator : INotifyPropertyChanged {
    /// <summary>
    ///     Returns the localized string for <paramref name="key"/> using the
    ///     current culture. Returns <c>"[key]"</c> when no match is found.
    /// </summary>
    /// <param name="key">
    ///     The resource key.
    /// </param>
    string this[string key] { get; }

    /// <summary>
    ///     Returns the localized string for <paramref name="key"/> formatted
    ///     with <paramref name="parameters"/>. Returns <c>"[key]"</c> when no
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

/// <summary>
///     Marker interface that scopes <see cref="ITranslator"/> lookups to the
///     section mapped from <typeparamref name="T"/>, in the style of
///     <c>ILogger&lt;T&gt;</c> and <c>IStringLocalizer&lt;T&gt;</c>. Resolve
///     through DI or <see cref="ITranslatorFactory"/>; it adds no members
///     beyond <see cref="ITranslator"/>.
/// </summary>
/// <typeparam name="T">
///     The anchor type whose simple name (arity stripped) maps to the
///     section this translator is scoped to. See
///     <see cref="TranslationKey.GetSection(Type)"/>.
/// </typeparam>
public interface ITranslator<out T> : ITranslator;