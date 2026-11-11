using System.ComponentModel;
using System.Globalization;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="Translator{T}"/>'s delegation to the factory-resolved
///     inner translator and its change-notification re-raise behavior
///     (ADR-003).
/// </summary>
[UnitTest]
public class TranslatorOfTTests {
    private sealed class FakeTranslator : ITranslator {
        private PropertyChangedEventHandler? _propertyChanged;

        public string? LastKey { get; private set; }
        public object[]? LastParameters { get; private set; }
        public int SubscriberCount { get; private set; }
        public CultureInfo? LastSetCulture { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged {
            add { _propertyChanged += value; SubscriberCount++; }
            remove { _propertyChanged -= value; SubscriberCount--; }
        }

        public string this[string key] => this[key, parameters: []];

        public string this[string key, params object[] parameters] {
            get {
                LastKey = key;
                LastParameters = parameters;

                return $"[{key}]";
            }
        }

        public CultureInfo CurrentCulture => LastSetCulture ?? CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) {
            LastSetCulture = culture;
        }

        public void SetCulture(string cultureName) {
            SetCulture(CultureInfo.GetCultureInfo(cultureName));
        }

        public void Raise() {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }

    private sealed class FakeTranslatorFactory(ITranslator inner) : ITranslatorFactory {
        public ITranslator Create(Type anchor) => inner;

        public ITranslator Create(string section) => inner;
    }

    [Fact]
    public void Constructor_WhenFactoryIsNull_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new Translator<TranslatorOfTTests>(factory: null!));
    }

    [Fact]
    public void Indexer_DelegatesToTheFactoryResolvedInnerTranslator_WithNoFurtherComposition() {
        // arrange: the inner translator factory.Create(typeof(T)) resolves
        // is already section-scoped, so Translator<T> must not compose the
        // key again.
        var inner = new FakeTranslator();
        var sut = new Translator<TranslatorOfTTests>(new FakeTranslatorFactory(inner));

        // act
        var result = sut["Key", "arg"];

        // assert
        Assert.Equal("Key", inner.LastKey);
        Assert.Equal(["arg"], inner.LastParameters);
        Assert.Equal("[Key]", result);
    }

    [Fact]
    public void CurrentCulture_DelegatesToInner() {
        // arrange
        var inner = new FakeTranslator();
        inner.SetCulture("pt-BR");
        var sut = new Translator<TranslatorOfTTests>(new FakeTranslatorFactory(inner));

        // act & assert
        Assert.Equal(inner.CurrentCulture, sut.CurrentCulture);
    }

    [Fact]
    public void SetCulture_DelegatesToInner() {
        // arrange
        var inner = new FakeTranslator();
        var sut = new Translator<TranslatorOfTTests>(new FakeTranslatorFactory(inner));

        // act
        sut.SetCulture("pt-BR");

        // assert
        Assert.Equal(CultureInfo.GetCultureInfo("pt-BR"), inner.LastSetCulture);
    }

    [Fact]
    public void PropertyChanged_WhenInnerRaises_ReRaisesWithSelfAsSender() {
        // arrange: forwarding add/remove straight to the inner translator
        // would make WPF's PropertyChangedEventManager drop the event,
        // because it routes by sender.
        var inner = new FakeTranslator();
        var sut = new Translator<TranslatorOfTTests>(new FakeTranslatorFactory(inner));

        object? observedSender = null;
        string? observedProperty = null;
        sut.PropertyChanged += (sender, args) => {
            observedSender = sender;
            observedProperty = args.PropertyName;
        };

        // act
        inner.Raise();

        // assert
        Assert.Same(sut, observedSender);
        Assert.Equal("Item[]", observedProperty);
    }

    [Fact]
    public void PropertyChanged_RepeatedCreateOfT_DoesNotLeakSubscriptionsOnTheInnerTranslator() {
        // arrange: a fresh Translator<T> per factory.Create<T>() call, all
        // sharing the same cached inner translator (as TranslatorFactory
        // does) — each instance's own subscribe/unsubscribe must not leak
        // a handler on the shared inner translator.
        var inner = new FakeTranslator();
        var factory = new FakeTranslatorFactory(inner);

        void Handler(object? sender, PropertyChangedEventArgs args) { }

        for (var i = 0; i < 5; i++) {
            var translator = new Translator<TranslatorOfTTests>(factory);
            translator.PropertyChanged += Handler;
            translator.PropertyChanged -= Handler;
        }

        // assert
        Assert.Equal(0, inner.SubscriberCount);
    }
}
