using Microsoft.Extensions.Hosting;

namespace Nameless.WPF.Hosting.Internals;

internal static class HostExtensions {
    extension(IHost self) {
        internal WinHost Wrap() {
            return new WinHost(self);
        }
    }
}
