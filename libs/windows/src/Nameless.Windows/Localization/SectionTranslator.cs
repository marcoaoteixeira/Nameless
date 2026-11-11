using System.ComponentModel;
using System.Globalization;
using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Windows.Localization;

/// <summary>
///     <see cref="ITranslator"/> wrapper that scopes every lookup to one
///     section, composing <c>{Section}_{Key}</c> before delegating to the
///     root translator. Cached per section by <see cref="TranslatorFactory"/>.
/// </summary>
/// <remarks>
///     Re-raises the root's <see cref="INotifyPropertyChanged.PropertyChanged"/>
///     with <em>this</em> instance as the sender — forwarding the event
///     as-is would make WPF's <c>PropertyChangedEventManager</c> drop it,
///     because it routes by sender. Subscribes to the root lazily, only
///     while it has subscribers of its own, so repeated resolution cannot
///     leak handlers on the root.
/// </remarks>
[SkipAssemblyScan]
internal sealed class SectionTranslator : ITranslator {
    private readonly ITranslator _root;
    private readonly string _section;
    private readonly PropertyChangedForwarder _propertyChangedForwarder;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged {
        add => _propertyChangedForwarder.Add(value);
        remove => _propertyChangedForwarder.Remove(value);
    }

    /// <inheritdoc />
    public CultureInfo CurrentCulture => _root.CurrentCulture;

    /// <inheritdoc />
    public string this[string key] => this[key, parameters: []];

    /// <inheritdoc />
    public string this[string key, params object[] parameters] =>
        string.IsNullOrWhiteSpace(key)
            ? _root[key, parameters]
            : _root[TranslationKey.Compose(_section, key), parameters];

    /// <summary>
    ///     Initializes a new instance of <see cref="SectionTranslator"/>
    ///     class.
    /// </summary>
    /// <param name="root">
    ///     The root translator to delegate every lookup, culture switch
    ///     and change notification to.
    /// </param>
    /// <param name="section">
    ///     The section this instance scopes lookups to.
    /// </param>
    public SectionTranslator(ITranslator root, string section) {
        _root = Throws.When.Null(root);
        _section = Throws.When.NullOrWhiteSpace(section);
        _propertyChangedForwarder = new PropertyChangedForwarder(_root, this);
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) {
        _root.SetCulture(culture);
    }

    /// <inheritdoc />
    public void SetCulture(string cultureName) {
        _root.SetCulture(cultureName);
    }
}
