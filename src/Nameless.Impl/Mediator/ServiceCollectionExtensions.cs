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
        public IServiceCollection RegisterMediator(Action<MediatorRegistration>? registration = null) {
            var settings = ActionHelper.FromDelegate(registration);

            self.TryAddTransient<IMediator, MediatorImpl>();

            return self.RegisterEvents(settings)
                       .RegisterRequests(settings)
                       .RegisterStreams(settings);
        }

        private IServiceCollection RegisterEvents(MediatorRegistration settings) {
            self.TryAddTransient<IEventHandlerInvoker, EventHandlerInvoker>();

            var handlerService = typeof(IEventHandler<>);
            var handlerImplementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(handlerService, includeGenericDefinition: true)
                : settings.EventHandlers;

            self.RegisterHandlers(handlerService, handlerImplementations);

            return self;
        }

        private IServiceCollection RegisterRequests(MediatorRegistration settings) {
            self.TryAddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();

            var handlerService = typeof(IRequestHandler<,>);
            var handlerImplementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(handlerService, includeGenericDefinition: true)
                : settings.RequestHandlers;

            self.RegisterHandlers(handlerService, handlerImplementations);

            self.RegisterPipelineBehaviors(
                service: typeof(IRequestPipelineBehavior<,>),
                implementations: settings.RequestPipelineBehaviors
            );

            return self;
        }

        private IServiceCollection RegisterStreams(MediatorRegistration settings) {
            self.TryAddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();

            var handlerService = typeof(IStreamHandler<,>);
            var handlerImplementations = settings.UseAssemblyScan
                ? settings.ExecuteAssemblyScan(handlerService, includeGenericDefinition: true)
                : settings.StreamHandlers;

            self.RegisterHandlers(handlerService, handlerImplementations);

            self.RegisterPipelineBehaviors(
                service: typeof(IStreamPipelineBehavior<,>),
                implementations: settings.StreamPipelineBehaviors
            );

            return self;
        }

        private void RegisterHandlers(Type service, IReadOnlyCollection<Type> implementations) {
            if (implementations.Count == 0) { return; }

            // register all open generic types first.
            var open = implementations.Where(type => type.IsOpenGeneric);
            var openServiceDescriptors = open.Select(
                implementation => ServiceDescriptor.Transient(service, implementation)
            );

            self.TryAddEnumerable(openServiceDescriptors);

            // followed by all closed types.
            var close = implementations.Where(type => !type.IsOpenGeneric);
            var closeServiceDescriptors = close.SelectMany(
                implementation => implementation.GetInterfacesThatCloses(service).Select(
                    @interface => ServiceDescriptor.Transient(
                        @interface.FixTypeReference(),
                        implementation
                    )
                )
            );

            self.TryAdd(closeServiceDescriptors);
        }

        private void RegisterPipelineBehaviors(Type service, IReadOnlyCollection<Type> implementations) {
            if (implementations.Count == 0) { return; }

            var descriptors = implementations.SelectMany(
                implementation => implementation.GetInterfacesThatCloses(service).Select(
                    @interface => ServiceDescriptor.Transient(
                        @interface.FixTypeReference(),
                        implementation
                    )
                )
            );

            self.TryAddEnumerable(descriptors);
        }
    }
}