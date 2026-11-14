using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Nameless.Helpers;
using Nameless.Mediator;
using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;
using Nameless.Registration;

namespace Nameless.Hosting.Features.Mediator;

/// <summary>
///     Registers the mediator, scanning the context assemblies.
/// </summary>
public class MediatorHostFeature : HostFeature<MediatorFeatureOptions> {
    /// <inheritdoc />
    public override void Register(IHostApplicationBuilder builder, HostFeatureContext context) {
        var options = ActionHelper.FromDelegate(ConfigureFeature).WithAssemblies(context.Assemblies);
        var services = builder.Services;

        services.TryAddTransient<IMediator, MediatorImpl>();

        RegisterEventHandler(services, options);
        RegisterRequestHandler(services, options);
        RegisterStreamHandler(services, options);
    }

    private static void RegisterEventHandler(IServiceCollection services, MediatorFeatureOptions registration) {
        services.TryAddTransient<IEventHandlerInvoker, EventHandlerInvoker>();

        RegisterHandlers(
            services,
            typeof(IEventHandler<>),
            registration.EventHandlers,
            multiple: true
        );
    }

    private static void RegisterRequestHandler(IServiceCollection services, MediatorFeatureOptions registration) {
        services.TryAddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();

        // each access to the handlers collection executes the assembly
        // scan again, read it once.
        var handlerServices = new[] { typeof(IRequestHandler<,>), typeof(IRequestHandler<>) };
        var handlerImplementations = registration.RequestHandlers;
        foreach (var handlerService in handlerServices) {
            RegisterHandlers(services, handlerService, handlerImplementations, multiple: false);
        }

        var behaviourServices = new[] { typeof(IRequestPipelineBehaviour<,>), typeof(IRequestPipelineBehaviour<>) };
        var behaviourImplementations = registration.RequestPipelineBehaviors;
        foreach (var behaviourService in behaviourServices) {
            RegisterPipelineBehaviors(services, behaviourService, behaviourImplementations);
        }
    }

    private static void RegisterStreamHandler(IServiceCollection services, MediatorFeatureOptions registration) {
        services.TryAddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();

        RegisterHandlers(services, typeof(IStreamHandler<,>), registration.StreamHandlers, multiple: false);
        RegisterPipelineBehaviors(services, typeof(IStreamPipelineBehavior<,>), registration.StreamPipelineBehaviors);
    }

    private static void RegisterHandlers(IServiceCollection self, Type service, IReadOnlyCollection<Type> implementations, bool multiple) {
        // open generic implementations first, followed by the closed ones.
        var descriptors = CreateServiceDescriptors(
            service,
            [
                .. implementations.OrderBy(
                    implementation => implementation.IsGenericTypeDefinition ? 0 : 1
                )
            ]
        );

        if (multiple) {
            self.TryAddEnumerable(descriptors);

            return;
        }

        foreach (var descriptor in descriptors) {
            RegisterSingleHandler(self, descriptor);
        }
    }

    // A request (or stream) must have exactly one handler. Registering
    // the same implementation again is a no-op, so RegisterMediator stays
    // idempotent; a different implementation for the same service is a
    // configuration error.
    private static void RegisterSingleHandler(IServiceCollection self, ServiceDescriptor descriptor) {
        var current = self.FirstOrDefault(item => !item.IsKeyedService && item.ServiceType == descriptor.ServiceType);

        if (current is null) {
            self.Add(descriptor);

            return;
        }

        if (current.ImplementationType == descriptor.ImplementationType) { return; }

        var serviceName = descriptor.ServiceType.GetPrettyName();
        var implementationName = current.ImplementationType?.GetPrettyName() ??
                                 "a factory or instance registration";

        throw new InvalidOperationException(
            $"Service '{serviceName}' already has the handler '{implementationName}'; " +
            $"cannot register '{descriptor.ImplementationType?.GetPrettyName()}'. A " +
            "request or stream must have exactly one handler."
        );
    }

    // Pipeline behaviors keep the order they were registered in, it is
    // the order they are executed.
    private static void RegisterPipelineBehaviors(IServiceCollection services, Type service, IReadOnlyCollection<Type> implementations) {
        services.TryAddEnumerable(
            CreateServiceDescriptors(service, implementations)
        );
    }

    internal static ServiceDescriptor[] CreateServiceDescriptors(Type service, IReadOnlyCollection<Type> implementations) {
        return [
            ..
            from implementation in implementations
            from serviceType in GetServiceTypes(implementation, service)
            select ServiceDescriptor.Transient(serviceType, implementation)
        ];
    }

    // Manual registration (MediatorRegistration.With*) already rejects
    // invalid types, so an invalid type reaching this point was found by the
    // assembly scan: point the user to the way of excluding it.
    private static Type[] GetServiceTypes(Type implementation, Type service) {
        try { return MediatorTypeInspector.GetServiceTypes(service, implementation); }
        catch (InvalidOperationException ex) {
            throw new InvalidOperationException(
                $"{ex.Message} If '{implementation.GetPrettyName()}' was found by the " +
                "assembly scan and must not be registered, mark it with " +
                $"[{nameof(SkipAssemblyScanAttribute)}] or make it abstract.",
                ex
            );
        }
    }
}
