using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Nameless.Diagnostics;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the <see cref="IActivitySourceProvider"/> service.
        /// </summary>
        /// <param name="configure">
        ///     A callback delegate to configure the
        ///     <see cref="ActivitySourceOptions"/>.
        /// </param>
        /// <returns>
        ///     The current instance of <see cref="IServiceCollection"/> so
        ///     other actions can be chained.
        /// </returns>
        public IServiceCollection RegisterActivitySourceProvider(Action<ActivitySourceOptions>? configure = null) {
            self.Configure(configure ?? (_ => { }));

            self.TryAddSingleton<IActivitySourceProvider, ActivitySourceProvider>();

            return self;
        }
    }
}