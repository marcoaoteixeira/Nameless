using System.ComponentModel;
using Nameless.Contracts;

namespace Nameless.Windows.Localization;

/// <summary>
///     Lazily subscribes to a source's <see cref="INotifyPropertyChanged.PropertyChanged"/>
///     only while it has subscribers of its own, re-raising every event
///     with a fixed sender instead of forwarding it as-is — forwarding
///     as-is would make WPF's <c>PropertyChangedEventManager</c> drop the
///     event, because it routes by sender.
/// </summary>
/// <remarks>
///     Shared by <see cref="SectionTranslator"/> and <see cref="Translator{T}"/>
///     so the subscribe/unsubscribe bookkeeping (derived from whether the
///     backing delegate is <see langword="null"/>, never from a counter)
///     is written once.
/// </remarks>
internal sealed class PropertyChangedForwarder {
    private readonly INotifyPropertyChanged _source;
    private readonly object _sender;
    private readonly Lock _sync = new();

    private PropertyChangedEventHandler? _handlers;

    /// <summary>
    ///     Initializes a new instance of <see cref="PropertyChangedForwarder"/>
    ///     class.
    /// </summary>
    /// <param name="source">
    ///     The source whose <see cref="INotifyPropertyChanged.PropertyChanged"/>
    ///     is subscribed to lazily.
    /// </param>
    /// <param name="sender">
    ///     The sender every re-raised event reports instead of <paramref name="source"/>.
    /// </param>
    public PropertyChangedForwarder(INotifyPropertyChanged source, object sender) {
        _source = Throws.When.Null(source);
        _sender = Throws.When.Null(sender);
    }

    /// <summary>
    ///     Adds <paramref name="value"/> as a subscriber, subscribing to
    ///     <see cref="_source"/> first if it was the first subscriber.
    /// </summary>
    /// <param name="value">
    ///     The handler to add. A <see langword="null"/> handler is a no-op.
    /// </param>
    public void Add(PropertyChangedEventHandler? value) {
        if (value is null) { return; }

        lock (_sync) {
            var wasEmpty = _handlers is null;

            _handlers += value;

            if (wasEmpty) {
                _source.PropertyChanged += OnSourcePropertyChanged;
            }
        }
    }

    /// <summary>
    ///     Removes <paramref name="value"/> from the subscribers,
    ///     unsubscribing from <see cref="_source"/> once none are left.
    /// </summary>
    /// <param name="value">
    ///     The handler to remove. A <see langword="null"/> handler is a
    ///     no-op.
    /// </param>
    public void Remove(PropertyChangedEventHandler? value) {
        if (value is null) {
            return;
        }

        lock (_sync) {
            _handlers -= value;

            if (_handlers is null) {
                _source.PropertyChanged -= OnSourcePropertyChanged;
            }
        }
    }

    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e) {
        _handlers?.Invoke(_sender, e);
    }
}
