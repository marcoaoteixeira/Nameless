using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nameless.Helpers;
using Nameless.Mediator.Events;
using Nameless.Mediator.Pipelines;
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
        /// <param name="configure">
        ///     The configuration action.
        /// </param>
        /// <returns>
        ///     The current <see cref="IServiceCollection"/> so other
        ///     actions can be chained.
        /// </returns>
        public IServiceCollection RegisterMediator(Action<MediatorRegistration>? configure = null) {
            var registration = ActionHelper.FromDelegate(configure);

            self.TryAddTransient<IMediator, MediatorImpl>();

            return self.RegisterEvents(registration)
                       .RegisterRequests(registration)
                       .RegisterStreams(registration);
        }

        private IServiceCollection RegisterEvents(MediatorRegistration registration) {
            self.TryAddTransient<IEventHandlerInvoker, EventHandlerInvoker>();
            self.RegisterHandlers(typeof(IEventHandler<>), registration.EventHandlers);

            return self;
        }

        private IServiceCollection RegisterRequests(MediatorRegistration registration) {
            self.TryAddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();
            self.RegisterHandlers(typeof(IRequestHandler<,>), registration.RequestHandlers);

            var pipelines = registration.UseValidateRequestPipelineBehavior
                ? [typeof(ValidateRequestPipelineBehavior<,>), .. registration.RequestPipelineBehaviors]
                : registration.RequestPipelineBehaviors;

            self.RegisterPipelineBehaviors(typeof(IRequestPipelineBehavior<,>), pipelines);

            return self;
        }

        private IServiceCollection RegisterStreams(MediatorRegistration registration) {
            self.TryAddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();
            self.RegisterHandlers(typeof(IStreamHandler<,>), registration.StreamHandlers);

            var pipelines = registration.UseValidateStreamPipelineBehavior
                ? [typeof(ValidateStreamPipelineBehavior<,>), .. registration.StreamPipelineBehaviors]
                : registration.StreamPipelineBehaviors;

            self.RegisterPipelineBehaviors(typeof(IStreamPipelineBehavior<,>), pipelines);

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