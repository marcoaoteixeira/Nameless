using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Lucene.Repository.Mappings;

namespace Nameless.Lucene.Repository;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterLuceneRepository(Action<LuceneRepositoryRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAdd(
                descriptors: CreateEntityMappingServiceDescriptors(settings)
            );

            self.TryAddTransient<IEntityDescriptorProvider, EntityDescriptorProvider>();
            self.TryAddTransient<IMapper, Mapper>();
            self.TryAddTransient<IRepository, RepositoryImpl>();

            return self;
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateEntityMappingServiceDescriptors(LuceneRepositoryRegistrationSettings settings) {
        var service = typeof(IEntityMapping<>);
        var implementations = settings.UseAssemblyScan
            ? settings.ExecuteAssemblyScan(typeof(IEntityMapping<>))
            : settings.Mappings;
        
        foreach (var mapping in implementations) {
            var interfaces = mapping.GetInterfacesThatCloses(service);
            
            foreach (var @interface in interfaces) {
                yield return ServiceDescriptor.Transient(@interface, mapping);
            }
        }
    }
}
