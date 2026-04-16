using Nameless.WPF.Client.Views.Pages;

namespace Nameless.WPF.Client;

internal static class StaticData {
    internal static class Pages {
        internal static class AppConfiguration {
            internal static Type Type => typeof(AppConfigurationPage);
            internal static string Name => "Configurações";
            internal static string ToolTip => "Configurações do Aplicativo";
        }
    }
}
