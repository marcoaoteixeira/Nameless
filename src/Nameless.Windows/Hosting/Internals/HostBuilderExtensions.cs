using Microsoft.Extensions.Hosting;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Internals;

internal static class HostBuilderExtensions {
    extension(IHostBuilder self) {
        internal WinHostBuilder Wrap() {
            return new WinHostBuilder(self);
        }
    }
}