using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Lucene.Repository;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers all the services required by Lucene.
        /// </summary>
        /// <param name="configure">
        ///     The registration settings.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterLucene(Action<LuceneRegistration>? configure = null, IConfiguration? configuration = null) {
            var registration = ActionHelper.FromDelegate(configure);

            self.ConfigureOptions<LuceneOptions>(configuration);
            self.RegisterAnalyzerSelectors(registration);
            self.TryAddSingleton<IAnalyzerProvider, AnalyzerProvider>();
            self.TryAddSingleton<IIndexProvider, IndexProvider>();
            self.RegisterLuceneRepository(registration);

            return self;
        }

        private void RegisterAnalyzerSelectors(LuceneRegistration registration) {
            // All analyzer selectors should be resolved by the same interface
            // IAnalyzerSelector, hence using TryAddEnumerable
            self.TryAddEnumerable(registration.AnalyzerSelectors.Select(
                implementation => ServiceDescriptor.Singleton(typeof(IAnalyzerSelector), implementation)
            ));
        }

        private void RegisterLuceneRepository(LuceneRegistration registration) {
            if (!registration.UseRepository) { return; }

            self.RegisterEntityMappings(registration);
            self.TryAddTransient<IEntityDescriptorProvider, EntityDescriptorProvider>();
            self.TryAddTransient<IMapper, Mapper>();
            self.TryAddTransient<IRepository, RepositoryImpl>();
        }

        private void RegisterEntityMappings(LuceneRegistration registration) {
            var service = typeof(IEntityMapping<>);
            var descriptors =
                from mapping in registration.Mappings
                let interfaces = mapping.GetInterfacesThatCloses(service)
                from @interface in interfaces
                select ServiceDescriptor.Transient(@interface.FixTypeReference(), mapping);

            self.TryAdd(descriptors);
        }
    }
}