using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.WPF.DependencyInjection;

namespace Nameless.WPF.Documents;

public static class ServiceCollectionExtensions {
    extension(IServiceCollection self) {
        public IServiceCollection RegisterDocumentServices(Action<DocumentServicesRegistration>? configure = null) {
            var registration = ActionHelper.FromDelegate(configure);

            self.RegisterDocumentReaders(registration);
            self.RegisterDocumentConverters(registration);
            self.TryAddSingleton<IDocumentService, DocumentService>();

            return self;
        }

        private void RegisterDocumentConverters(DocumentServicesRegistration registration) {
            var service = typeof(IDocumentConverter);
            var implementations = registration.UseAssemblyScan
                ? registration.ExecuteAssemblyScan(service)
                : registration.DocumentConverters;

            var descriptors = implementations.Select(
                implementation => implementation.CreateServiceDescriptor(service)
            );

            self.TryAddEnumerable(descriptors);
        }

        private void RegisterDocumentReaders(DocumentServicesRegistration registration) {
            var service = typeof(IDocumentReader);
            var implementations = registration.UseAssemblyScan
                ? registration.ExecuteAssemblyScan(service)
                : registration.DocumentReaders;

            var descriptors = implementations.Select(
                implementation => implementation.CreateServiceDescriptor(service)
            );

            self.TryAddEnumerable(descriptors);
        }
    }
}