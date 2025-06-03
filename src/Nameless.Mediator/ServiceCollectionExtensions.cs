using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Helpers;
using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register mediator services.
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// Registers the mediator services.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceCollection"/>.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>
    /// The current <see cref="IServiceCollection"/> so other actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterMediatorServices(this IServiceCollection self, Action<MediatorOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new MediatorOptions();

        innerConfigure(options);

        return self.RegisterEventHandlers(options)

                   .RegisterRequestHandlers(options)
                   .RegisterRequestPipelineBehaviors(options)

                   .RegisterStreamHandlers(options)
                   .RegisterStreamPipelineBehaviors(options)

                   .RegisterMainServices();
    }

    private static IServiceCollection RegisterEventHandlers(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseEventHandler) { return self; }

        var serviceType = typeof(IEventHandler<>);
        var eventHandlers = options.Assemblies
                                   .GetImplementations([serviceType])
                                   .ToArray();

        return self.RegisterImplementations(serviceType, eventHandlers, options.Assemblies);
    }

    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseRequestHandler) { return self; }

        var serviceTypes = new[] { typeof(IRequestHandler<>), typeof(IRequestHandler<,>) };
        var requestHandlers = options.Assemblies
                                     .GetImplementations(serviceTypes)
                                     .ToArray();

        return self.RegisterImplementations(serviceTypes[0], requestHandlers, options.Assemblies)
                   .RegisterImplementations(serviceTypes[1], requestHandlers, options.Assemblies);
    }

    private static IServiceCollection RegisterRequestPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseRequestHandler) { return self; }

        return self.RegisterImplementations(
            typeof(IRequestPipelineBehavior<,>),
            options.RequestPipelineBehaviors,
            options.Assemblies
        );
    }

    private static IServiceCollection RegisterStreamHandlers(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseStreamHandler) { return self; }

        var service = typeof(IStreamHandler<,>);
        var streamHandlers = options.Assemblies
                                    .GetImplementations([service])
                                    .ToArray();

        return self.RegisterImplementations(service, streamHandlers, options.Assemblies);
    }

    private static IServiceCollection RegisterStreamPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseStreamHandler) { return self; }

        return self.RegisterImplementations(
            typeof(IStreamPipelineBehavior<,>),
            options.StreamPipelineBehaviors,
            options.Assemblies
        );
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        return self.AddScoped<IEventHandlerInvoker, EventHandlerInvoker>()
                   .AddScoped<IRequestHandlerInvoker, RequestHandlerInvoker>()
                   .AddScoped<IStreamHandlerInvoker, StreamHandlerInvoker>()
                   .AddScoped<IMediator, MediatorImpl>();
    }

    private static IServiceCollection RegisterImplementations(this IServiceCollection self, Type service, Type[] implementations, Assembly[] assemblies) {
        // This will expand the registrations with closed generic types
        foreach (var implementation in implementations) {
            if (!implementation.IsGenericTypeDefinition) {
                self.RegisterImplementation(service, implementation);
                continue;
            }

            // If the implementation is an open generic type,
            // we need to close it in order to be able to resolve it later.
            var closingTypeGroups = GenericTypeHelper.FindClosingTypes(implementation, assemblies);
            foreach (var closingTypeGroup in closingTypeGroups) {
                var concrete = implementation.MakeGenericType(closingTypeGroup);
                self.RegisterImplementation(service, concrete);
            }
        }

        return self;
    }

    private static void RegisterImplementation(this IServiceCollection self, Type service, Type implementation) {
        foreach (var implInterface in implementation.GetInterfaces()) {
            if (!service.IsAssignableFromGenericType(implInterface)) {
                continue;
            }

            self.AddScoped(implInterface, implementation);
        }
    }
}