using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wpf.Ui;

namespace Nameless.WPF.SnackBar;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterSnackBar() {
            self.TryAddSingleton<ISnackbarService, SnackbarService>();

            return self;
        }
    }
}
