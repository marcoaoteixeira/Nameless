using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;

namespace Nameless.Windows.Documents;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterDocumentServices(Action<DocumentServicesRegistration>? configure = null) {
            var registration = ActionHelper.FromDelegate(configure);

            self.TryAddSingleton<IDocumentService, DocumentService>();
            self.RegisterDocumentReaders(registration);
            self.RegisterDocumentConverters(registration);

            return self;
        }

        private void RegisterDocumentConverters(DocumentServicesRegistration registration) {
            var service = typeof(IDocumentConverter);
            var implementations = registration.UseAssemblyScan
                ? registration.ExecuteAssemblyScan(service)
                : registration.DocumentConverters;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Singleton(service, implementation)
            );

            self.TryAddEnumerable(descriptors);
        }

        private void RegisterDocumentReaders(DocumentServicesRegistration registration) {
            var service = typeof(IDocumentReader);
            var implementations = registration.UseAssemblyScan
                ? registration.ExecuteAssemblyScan(service)
                : registration.DocumentReaders;

            var descriptors = implementations.Select(
                implementation => ServiceDescriptor.Singleton(service, implementation)
            );

            self.TryAddEnumerable(descriptors);
        }
    }
}