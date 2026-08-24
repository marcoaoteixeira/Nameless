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
        /// <param name="registration">
        ///     The registration settings.
        /// </param>
        /// <param name="configuration">
        ///     The configuration.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterLucene(Action<LuceneRegistration>? registration = null, IConfiguration? configuration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.ConfigureOptions<LuceneOptions>(configuration);
            self.RegisterAnalyzerSelectors(settings);
            self.TryAddSingleton<IAnalyzerProvider, AnalyzerProvider>();
            self.TryAddSingleton<IIndexProvider, IndexProvider>();
            self.RegisterLuceneRepository(settings);

            return self;
        }
        
        private void RegisterAnalyzerSelectors(LuceneRegistration settings) {
            var service = typeof(IAnalyzerSelector);
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan<IAnalyzerSelector>()
                : settings.AnalyzerSelectors;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Singleton(service, implementation)
            );

            // All analyzer selectors should be resolved by the same interface
            // IAnalyzerSelector, hence using TryAddEnumerable
            self.TryAddEnumerable(descriptors);
        }

        private void RegisterLuceneRepository(LuceneRegistration settings) {
            if (!settings.UseRepository) { return; }

            self.RegisterEntityMappings(settings);
            self.TryAddTransient<IEntityDescriptorProvider, EntityDescriptorProvider>();
            self.TryAddTransient<IMapper, Mapper>();
            self.TryAddTransient<IRepository, RepositoryImpl>();
        }

        private void RegisterEntityMappings(LuceneRegistration settings) {
            var service = typeof(IEntityMapping<>);
            var implementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(typeof(IEntityMapping<>))
                : settings.Mappings;

            var descriptors =
                from mapping in implementations
                let interfaces = mapping.GetInterfacesThatCloses(service)
                from @interface in interfaces
                select ServiceDescriptor.Transient(@interface.FixTypeReference(), mapping);

            self.TryAdd(descriptors);
        }
    }
}