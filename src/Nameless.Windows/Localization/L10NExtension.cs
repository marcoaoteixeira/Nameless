using System.Windows.Data;
using System.Windows.Markup;

namespace Nameless.Windows.Localization;

/// <summary>
///     XAML markup extension that resolves a localized string from the active
///     <see cref="ILocalizer"/> and returns a live <see cref="Binding"/>
///     so bound controls refresh automatically on culture changes.
/// </summary>
/// <remarks>
///     Simple key (no format args):
///     <code>&lt;TextBlock Text="{loc:L10N WelcomeMessage}" /&gt;</code>
///     With runtime-bound format arguments:
///     <code>&lt;TextBlock Text="{loc:L10N Greeting, Parameters={Binding ViewModel.UserName}}" /&gt;</code>
///     When parameters are present a <see cref="MultiBinding"/> is used internally;
///     the translated string is treated as a
///     <see cref="string.Format(string,object[])"/> template.
/// </remarks>
public class L10NExtension : MarkupExtension {
    private static ILocalizer Localizer {
        get => field ?? NullLocalizer.Instance;
        set;
    }

    /// <summary>
    ///     Gets the resource key passed to the localizer.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Optional format arguments bound at runtime. Accepts any
    ///     <see cref="BindingBase"/> expression. The bound value may be a
    ///     scalar (used as <c>{0}</c>) or any non-string
    ///     <see cref="System.Collections.IEnumerable"/> (items spread as
    ///     positional arguments).
    /// </summary>
    public BindingBase? Parameters { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="L10NExtension"/>.
    /// </summary>
    /// <param name="key">
    ///     The resource key to look up in the active <see cref="ILocalizer"/>.
    /// </param>
    public L10NExtension(string key) {
        Key = key;
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider provider) {
        var keyBinding = new Binding($"[{Key}]") {
            Source = Localizer,
            Mode = BindingMode.OneWay
        };

        if (Parameters is null) {
            return keyBinding.ProvideValue(provider);
        }

        var multi = new MultiBinding {
            Converter = LocalizerFormatConverter.Instance
        };

        multi.Bindings.Add(keyBinding);
        multi.Bindings.Add(Parameters);

        return multi.ProvideValue(provider);
    }

    /// <summary>
    ///     Wires the shared static <see cref="ILocalizer"/> used by all
    ///     <see cref="L10NExtension"/> instances. Call this once after the DI
    ///     container is built, passing the resolved <see cref="ILocalizer"/>
    ///     singleton.
    /// </summary>
    /// <param name="localizer">
    ///     The localizer to use for all XAML bindings.
    /// </param>
    public static void SetLocalizer(ILocalizer localizer) {
        Localizer = localizer;
    }
}
