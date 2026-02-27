using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Lucene.Infrastructure;
using Nameless.Lucene.Infrastructure.Implementations;
using Nameless.Lucene.Mapping;

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
        /// <param name="configure">
        ///     The action to configure <see cref="LuceneOptions"/>.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other actions
        ///     can be chained.
        /// </returns>
        public IServiceCollection RegisterLucene(Action<LuceneRegistrationSettings> registration, Action<LuceneOptions>? configure = null) {
            return self.Configure(configure ?? (_ => { }))
                       .InnerRegisterLucene(registration);
        }

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
        public IServiceCollection RegisterLucene(Action<LuceneRegistrationSettings> registration, IConfiguration configuration) {
            var section = configuration.GetSection<LuceneOptions>();

            return self.Configure<LuceneOptions>(section)
                       .InnerRegisterLucene(registration);
        }

        private IServiceCollection InnerRegisterLucene(Action<LuceneRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            // All analyzer selectors should be resolved by the same interface
            // IAnalyzerSelector, hence using TryAddEnumerable
            self.TryAddEnumerable(
                descriptors: CreateAnalyzerSelectorDescriptors(settings)
            );
            // All entity mappings should be resolved by the same interface
            // IEntityMapping, hence using TryAddEnumerable
            self.TryAddEnumerable(
                descriptors: CreateEntityMappingDescriptors(settings)
            );

            self.TryAddSingleton<IAnalyzerProvider, AnalyzerProvider>();
            self.TryAddSingleton<IIndexProvider, IndexProvider>();
            self.TryAddSingleton<EntityDocumentBuilder>();
            self.TryAddSingleton<IRepository, Repository>();

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateAnalyzerSelectorDescriptors(LuceneRegistrationSettings settings) {
        return settings.AnalyzerSelectors.Select(implementation
            => ServiceDescriptor.Singleton(
                typeof(IAnalyzerSelector),
                implementation
            )
        );
    }

    private static IEnumerable<ServiceDescriptor> CreateEntityMappingDescriptors(LuceneRegistrationSettings settings) {
        return settings.Mappings.Select(implementation
            => ServiceDescriptor.Singleton(
                typeof(IEntityMapping),
                implementation
            )
        );
    }
}