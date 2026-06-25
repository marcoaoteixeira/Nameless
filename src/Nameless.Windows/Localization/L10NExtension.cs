using System.Windows.Data;
using System.Windows.Markup;

namespace Nameless.Windows.Localization;

public class L10NExtension : MarkupExtension {
    private static ILocalizer Localizer {
        get => field ?? NullLocalizer.Instance;
        set;
    }

    public string Key { get; }

    public L10NExtension(string key) {
        Key = key;
    }

    public override object ProvideValue(IServiceProvider provider) {
        var binding = new Binding($"[{Key}]") {
            Source = Localizer,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(provider);
    }

    public static void SetLocalizer(ILocalizer localizer) {
        Localizer = localizer;
    }
}
