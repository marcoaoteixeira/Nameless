using System.ComponentModel;
using System.Globalization;
using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Default <see cref="ITranslator{T}"/> implementation. Delegates every
///     lookup, culture switch and change notification to the section-scoped
///     <see cref="ITranslator"/> that <see cref="ITranslatorFactory"/>
///     resolves for <typeparamref name="T"/> — the indexer needs no further
///     key composition, because that inner translator is already scoped to
///     <typeparamref name="T"/>'s section. Registered as an open-generic
///     singleton by <see cref="TranslatorHostFeature"/>, in the style of
///     <c>Logger&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T">
///     The anchor type whose simple name (arity stripped) maps to the
///     section this translator is scoped to.
/// </typeparam>
/// <remarks>
///     Re-raises the inner translator's
///     <see cref="INotifyPropertyChanged.PropertyChanged"/> with
///     <em>this</em> instance as the sender — forwarding the event as-is
///     would make WPF's <c>PropertyChangedEventManager</c> drop it, because
///     it routes by sender. Subscribes to the inner translator lazily, only
///     while it has subscribers of its own, so repeated
///     <c>factory.Create&lt;T&gt;()</c> calls cannot leak handlers.
/// </remarks>
[SkipAssemblyScan]
public sealed class Translator<T> : ITranslator<T> {
    private readonly ITranslator _inner;
    private readonly Lock _sync = new();

    private PropertyChangedEventHandler? _propertyChanged;
    private int _subscriberCount;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged {
        add {
            lock (_sync) {
                if (_subscriberCount == 0) {
                    _inner.PropertyChanged += OnInnerPropertyChanged;
                }

                _subscriberCount++;
                _propertyChanged += value;
            }
        }
        remove {
            lock (_sync) {
                if (value is null) {
                    return;
                }

                _propertyChanged -= value;
                _subscriberCount--;

                if (_subscriberCount <= 0) {
                    _subscriberCount = 0;
                    _inner.PropertyChanged -= OnInnerPropertyChanged;
                }
            }
        }
    }

    /// <inheritdoc />
    public CultureInfo CurrentCulture => _inner.CurrentCulture;

    /// <inheritdoc />
    public string this[string key] => _inner[key];

    /// <inheritdoc />
    public string this[string key, params object[] parameters] => _inner[key, parameters];

    /// <summary>
    ///     Initializes a new instance of <see cref="Translator{T}"/> class.
    /// </summary>
    /// <param name="factory">
    ///     The factory used to resolve the section-scoped inner translator
    ///     for <typeparamref name="T"/>.
    /// </param>
    public Translator(ITranslatorFactory factory) {
        Throws.When.Null(factory);

        _inner = factory.Create(typeof(T));
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) {
        _inner.SetCulture(culture);
    }

    /// <inheritdoc />
    public void SetCulture(string cultureName) {
        _inner.SetCulture(cultureName);
    }

    private void OnInnerPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        _propertyChanged?.Invoke(this, e);
    }
}
