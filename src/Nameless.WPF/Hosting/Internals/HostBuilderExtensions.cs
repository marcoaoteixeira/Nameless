using Microsoft.Extensions.Hosting;

namespace Nameless.WPF.Hosting.Internals;

internal static class HostBuilderExtensions {
    extension(IHostBuilder self) {
        internal WinHostBuilder Wrap() {
            return new WinHostBuilder(self);
        }
    }
}