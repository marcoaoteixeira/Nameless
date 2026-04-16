using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.WPF.Dialogs.Message.Impl;

namespace Nameless.WPF.Dialogs.Message;

/// <summary>
///     <see cref="IServiceCollection"/> extensions methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the <see cref="IMessageDialog"/> service.
        /// </summary>
        /// <returns>
        ///     The <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterMessageDialog() {
            self.TryAddSingleton<IMessageDialog, MessageDialog>();

            return self;
        }
    }
}
