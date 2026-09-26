using Microsoft.Extensions.Hosting;
using Nameless.Windows.Hosting.Wrappers;

namespace Nameless.Windows.Hosting.Internals;

internal static class HostExtensions {
    extension(IHost self) {
        internal WinHost Wrap() {
            return new WinHost(self);
        }
    }
}
