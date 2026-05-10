using System.Reflection;
using Nameless.Registration;

namespace Nameless.WPF.Hosting;

internal static class WinHostSettingsHelper {
    internal static Action<TSelf> Join<TSelf>(Action<TSelf>? configuration, IReadOnlyCollection<Assembly> assemblies)
        where TSelf : AssemblyScanAware<TSelf>, new() {
        return (Action<AssemblyScanAware<TSelf>>)Delegate.Combine(
            IncludeAssemblies,
            configuration
        );

        void IncludeAssemblies(AssemblyScanAware<TSelf> opts) {
            opts.IncludeAssemblies(assemblies);
        }
    }
}