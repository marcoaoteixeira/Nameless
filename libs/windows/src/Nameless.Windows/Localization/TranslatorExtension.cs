using Microsoft.Extensions.DependencyInjection;
using Nameless.Contracts;
using Nameless.Windows.AppConfiguration;
using Nameless.Windows.Helpers;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace Nameless.Windows.Localization;

/// <summary>
///     XAML markup extension that resolves a localized string from the active
///     <see cref="ITranslator"/> and returns a live <see cref="Binding"/>
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
public class TranslatorExtension : MarkupExtension {
    private static ITranslatorFactory TranslatorFactory {
        get => field ?? NullTranslatorFactory.Instance;
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
    ///     Initializes a new instance of <see cref="TranslatorExtension"/>.
    /// </summary>
    /// <param name="key">
    ///     The resource key to look up in the active <see cref="ITranslator"/>.
    /// </param>
    public TranslatorExtension(string key) {
        Key = key;
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider provider) {
        var rootObjectType = GetRootTarget(provider);
        var translator = TranslatorFactory.Create(rootObjectType);
        var keyBinding = new Binding($"[{Key}]") {
            Source = translator,
            Mode = BindingMode.OneWay
        };

        if (Parameters is null) {
            return keyBinding.ProvideValue(provider);
        }

        var multi = new MultiBinding {
            Converter = TranslatorMultiValueConverter.Instance
        };

        multi.Bindings.Add(keyBinding);
        multi.Bindings.Add(Parameters);

        return multi.ProvideValue(provider);
    }

    /// <summary>
    ///     Wires the shared static <see cref="ITranslatorFactory"/> used by all
    ///     <see cref="TranslatorExtension"/> instances. Call this once after the DI
    ///     container is built, passing the resolved <see cref="ITranslatorFactory"/>
    ///     singleton.
    /// </summary>
    /// <param name="provider">
    ///     The service provider.
    /// </param>
    public static void ResolveTranslatorFactory(IServiceProvider provider) {
        var appConfigurationManager = provider.GetRequiredService<IAppConfigurationManager>();

        if (!CultureResolver.TryGet(appConfigurationManager.Language, out var culture)) {
            CommonLog.Warning(
                provider.GetLogger(typeof(TranslatorExtension)),
                $"Unable to resolve culture '{appConfigurationManager.Language}'.",
                typeof(TranslatorExtension).Tag
            );
        }

        CultureInfo.CurrentCulture = culture ?? CultureInfo.CurrentCulture;
        CultureInfo.CurrentUICulture = culture ?? CultureInfo.CurrentUICulture;

        TranslatorFactory = provider.GetService<ITranslatorFactory>() ?? NullTranslatorFactory.Instance;
    }

    private static Type GetRootTarget(IServiceProvider serviceProvider) {
        var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        var root = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;

        // Inside a template/style, TargetObject is an internal placeholder (SharedDp),
        // not a DependencyObject. Return 'this' so WPF calls you again when the
        // template is applied to a real element.
        if (target?.TargetObject is not null &&
            target.TargetObject is not DependencyObject &&
            target.TargetObject.GetType().FullName == "System.Windows.SharedDp") {
            return typeof(UnknownRootObject);
        }

        return root?.RootObject.GetType() ?? typeof(UnknownRootObject);
    }

    internal readonly record struct UnknownRootObject;
}
