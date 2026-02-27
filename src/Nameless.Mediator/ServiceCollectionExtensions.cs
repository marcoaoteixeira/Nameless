using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

/// <summary>
///     <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    extension(IServiceCollection self) {
        /// <summary>
        ///     Registers the mediator services.
        /// </summary>
        /// <param name="registration">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterMediator(Action<MediatorRegistrationSettings> registration) {
            var settings = ActionHelper.FromDelegate(registration);

            self.RegisterEventHandlers(settings);

            self.RegisterRequestHandlers(settings);
            self.RegisterPipelineBehaviors(typeof(IRequestPipelineBehavior<,>), settings.RequestPipelineBehaviors);

            self.RegisterStreamHandlers(settings);
            self.RegisterPipelineBehaviors(typeof(IStreamPipelineBehavior<,>), settings.StreamPipelineBehaviors);

            self.TryAddTransient<IMediator, MediatorImpl>();

            return self;
        }

        private void RegisterEventHandlers(MediatorRegistrationSettings settings) {
            self.RegisterHandlers(typeof(IEventHandler<>), settings.EventHandlers)
                .TryAddTransient<IEventHandlerInvoker, EventHandlerInvoker>();
        }

        private void RegisterRequestHandlers(MediatorRegistrationSettings settings) {
            self.RegisterHandlers(typeof(IRequestHandler<,>), settings.RequestHandlers)
                .TryAddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();
        }

        private void RegisterStreamHandlers(MediatorRegistrationSettings settings) {
            self.RegisterHandlers(typeof(IStreamHandler<,>), settings.StreamHandlers)
                .TryAddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();
        }

        private IServiceCollection RegisterHandlers(Type service, IReadOnlyCollection<Type> implementations) {
            if (implementations.Count == 0) { return self; }

            var descriptors = CreateHandlerDescriptors(service, implementations);

            self.TryAddEnumerable(descriptors);

            return self;
        }

        private void RegisterPipelineBehaviors(Type service, IReadOnlyCollection<Type> implementations) {
            if (implementations.Count == 0) { return; }

            var descriptors = new List<ServiceDescriptor>();
            
            foreach (var implementation in implementations) {
                var interfaces = implementation.GetInterfacesThatCloses(service);

                foreach (var @interface in interfaces) {
                    descriptors.Add(ServiceDescriptor.Transient(
                        @interface.FixTypeReference(),
                        implementation
                    ));
                }
            }

            self.TryAddEnumerable(descriptors);
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateHandlerDescriptors(Type service, IReadOnlyCollection<Type> implementations) {
        var openGenerics = implementations.Where(implementation => implementation.IsOpenGeneric);
        foreach (var implementation in openGenerics) {
            yield return ServiceDescriptor.Transient(service, implementation);
        }

        var closed = implementations.Where(implementation => !implementation.IsOpenGeneric);
        foreach (var implementation in closed) {
            foreach (var @interface in implementation.GetInterfacesThatCloses(service)) {
                yield return ServiceDescriptor.Transient(
                    @interface.FixTypeReference(),
                    implementation
                );
            }
        }
    }
}