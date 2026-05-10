using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wpf.Ui;

namespace Nameless.WPF.Extensions;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
/// <remarks>
///     Common extension methods for build WPF applications.
/// </remarks>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/> instance.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the Content Dialog services.
        /// </summary>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> instance so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterContentDialogService() {
            self.TryAddSingleton<IContentDialogService, ContentDialogService>();

            return self;
        }
    }
}
