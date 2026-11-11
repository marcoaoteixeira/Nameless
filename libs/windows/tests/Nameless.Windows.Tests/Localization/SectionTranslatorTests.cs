using System.ComponentModel;
using System.Globalization;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="SectionTranslator"/>'s key composition, delegation
///     and change-notification re-raise behavior (ADR-003). Internal type,
///     reachable from this test assembly via <c>InternalsVisibleTo</c>.
/// </summary>
[UnitTest]
public class SectionTranslatorTests {
    /// <summary>
    ///     Records the last indexer call and every subscribe/unsubscribe,
    ///     so tests can assert both delegation and subscription leaks.
    /// </summary>
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

                return string.IsNullOrWhiteSpace(key) ? string.Empty : $"[{key}]";
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

    [Fact]
    public void Constructor_WhenRootIsNull_Throws() {
        // act & assert
        Assert.Throws<ArgumentNullException>(() => new SectionTranslator(root: null!, "Section"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WhenSectionIsNullEmptyOrWhiteSpace_Throws(string? section) {
        // act & assert: ArgumentNullException for null, ArgumentException
        // for empty/white-space — both derive from ArgumentException.
        Assert.ThrowsAny<ArgumentException>(() => new SectionTranslator(new FakeTranslator(), section!));
    }

    [Fact]
    public void Indexer_ComposesSectionAndKey_BeforeDelegatingToRoot() {
        // arrange
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        // act
        var result = sut["Key"];

        // assert
        Assert.Equal("Section_Key", root.LastKey);
        Assert.Equal("[Section_Key]", result);
    }

    [Fact]
    public void Indexer_WithParameters_ComposesKeyAndForwardsParameters() {
        // arrange
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        // act
        _ = sut["Key", "arg1", "arg2"];

        // assert
        Assert.Equal("Section_Key", root.LastKey);
        Assert.Equal(["arg1", "arg2"], root.LastParameters);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Indexer_WhenKeyIsNullEmptyOrWhiteSpace_PassesItToRootUnchanged(string? key) {
        // arrange: the inner translator keeps deciding what a null/blank
        // key means (JsonTranslator returns "", NullTranslator returns the
        // key) — composing it would hide that decision.
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        // act
        _ = sut[key!];

        // assert
        Assert.Equal(key, root.LastKey);
    }

    [Fact]
    public void CurrentCulture_DelegatesToRoot() {
        // arrange
        var root = new FakeTranslator();
        root.SetCulture("pt-BR");
        var sut = new SectionTranslator(root, "Section");

        // act & assert
        Assert.Equal(root.CurrentCulture, sut.CurrentCulture);
    }

    [Fact]
    public void SetCulture_DelegatesToRoot() {
        // arrange
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        // act
        sut.SetCulture("pt-BR");

        // assert
        Assert.Equal(CultureInfo.GetCultureInfo("pt-BR"), root.LastSetCulture);
    }

    [Fact]
    public void SetCulture_ByName_DelegatesToRoot() {
        // arrange
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        // act
        sut.SetCulture("fr-FR");

        // assert
        Assert.Equal(CultureInfo.GetCultureInfo("fr-FR"), root.LastSetCulture);
    }

    [Fact]
    public void PropertyChanged_WhenRootRaises_ReRaisesWithSelfAsSender() {
        // arrange: forwarding add/remove straight to the root would make
        // WPF's PropertyChangedEventManager drop the event, because it
        // routes by sender — the wrapper must re-raise as itself.
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        object? observedSender = null;
        string? observedProperty = null;
        sut.PropertyChanged += (sender, args) => {
            observedSender = sender;
            observedProperty = args.PropertyName;
        };

        // act
        root.Raise();

        // assert
        Assert.Same(sut, observedSender);
        Assert.Equal("Item[]", observedProperty);
    }

    [Fact]
    public void PropertyChanged_SubscribesToRootLazily_OnlyWhileItHasSubscribersOfItsOwn() {
        // arrange
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        Assert.Equal(0, root.SubscriberCount);

        void Handler(object? sender, PropertyChangedEventArgs args) { }

        // act: first subscriber — subscribes to the root.
        sut.PropertyChanged += Handler;
        Assert.Equal(1, root.SubscriberCount);

        // act: last subscriber removed — unsubscribes from the root.
        sut.PropertyChanged -= Handler;

        // assert
        Assert.Equal(0, root.SubscriberCount);
    }

    [Fact]
    public void PropertyChanged_RepeatedSubscribeAndUnsubscribe_DoesNotLeakRootSubscriptions() {
        // arrange: guards against factory.Create<T>() resolving the same
        // cached SectionTranslator repeatedly and leaking a subscription
        // on the root each time.
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        void HandlerA(object? sender, PropertyChangedEventArgs args) { }
        void HandlerB(object? sender, PropertyChangedEventArgs args) { }

        // act
        sut.PropertyChanged += HandlerA;
        sut.PropertyChanged += HandlerB;

        Assert.Equal(1, root.SubscriberCount);

        sut.PropertyChanged -= HandlerA;
        Assert.Equal(1, root.SubscriberCount);

        sut.PropertyChanged -= HandlerB;

        // assert
        Assert.Equal(0, root.SubscriberCount);
    }

    [Fact]
    public void PropertyChanged_RemovingNeverAddedHandler_DoesNotUnsubscribeRealSubscriber() {
        // arrange: removing a handler that was never added (or already
        // removed) must not drop a counter below the real subscriber count
        // and unsubscribe from the root prematurely.
        var root = new FakeTranslator();
        var sut = new SectionTranslator(root, "Section");

        var observed = false;
        void RealHandler(object? sender, PropertyChangedEventArgs args) {
            observed = true;
        }

        void NeverAddedHandler(object? sender, PropertyChangedEventArgs args) { }

        sut.PropertyChanged += RealHandler;

        // act
        sut.PropertyChanged -= NeverAddedHandler;
        sut.PropertyChanged -= NeverAddedHandler;

        root.Raise();

        // assert
        Assert.True(observed);
        Assert.Equal(1, root.SubscriberCount);
    }
}
