using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public static IServiceCollection RegisterMediator(this IServiceCollection self, Action<MediatorOptions>? configure = null) {
        // We are not going to register the configuration
        // action since there are no services that use
        // it after they have been registered.
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
        if (!options.UseEventHandlers) { return self; }

        var service = typeof(IEventHandler<>);
        var implementations = options.Assemblies
                                     .GetImplementations(service)
                                     .ToArray();

        return self.RegisterHandlerImplementations(service, implementations, options.Assemblies);
    }

    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseRequestHandlers) { return self; }

        var services = new[] { typeof(IRequestHandler<>), typeof(IRequestHandler<,>) };
        var implementations = options.Assemblies
                                     .GetImplementations(services)
                                     .ToArray();

        return self.RegisterHandlerImplementations(services[0], implementations, options.Assemblies)
                   .RegisterHandlerImplementations(services[1], implementations, options.Assemblies);
    }

    private static IServiceCollection RegisterRequestPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        return options.UseRequestHandlers
            ? self.RegisterPipelineBehaviors(typeof(IRequestPipelineBehavior<,>), options.RequestPipelineBehaviors)
            : self;
    }

    private static IServiceCollection RegisterStreamHandlers(this IServiceCollection self, MediatorOptions options) {
        if (!options.UseStreamHandlers) { return self; }

        var service = typeof(IStreamHandler<,>);
        var implementations = options.Assemblies
                                     .GetImplementations(service)
                                     .ToArray();

        return self.RegisterHandlerImplementations(service, implementations, options.Assemblies);
    }

    private static IServiceCollection RegisterStreamPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        return options.UseStreamHandlers
            ? self.RegisterPipelineBehaviors(typeof(IStreamPipelineBehavior<,>), options.StreamPipelineBehaviors)
            : self;
    }

    private static IServiceCollection RegisterMainServices(this IServiceCollection self) {
        self.TryAddTransient<IEventHandlerInvoker, EventHandlerInvoker>();
        self.TryAddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();
        self.TryAddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();
        self.TryAddTransient<IMediator, MediatorImpl>();

        return self;
    }

    private static IServiceCollection RegisterHandlerImplementations(this IServiceCollection self, Type service, Type[] implementations, Assembly[] assemblies) {
        // This will expand the registrations with closed generic types
        foreach (var implementation in implementations) {
            if (!implementation.IsOpenGeneric()) {
                self.RegisterHandlerImplementationCore(service, implementation);
                continue;
            }

            // If the implementation is an open generic type,
            // we need to close it in order to be able to resolve it later.
            var typesThatCloseGroup = GenericTypeHelper.GetTypesThatClose(implementation, assemblies);
            foreach (var typesThatClose in typesThatCloseGroup) {
                var concrete = implementation.MakeGenericType(typesThatClose);
                self.RegisterHandlerImplementationCore(service, concrete);
            }
        }

        return self;
    }

    private static void RegisterHandlerImplementationCore(this IServiceCollection self, Type service, Type implementation) {
        foreach (var @interface in implementation.GetInterfaces()) {
            if (!service.IsAssignableFromGenericType(@interface)) {
                continue;
            }

            self.TryAddTransient(@interface, implementation);
        }
    }

    private static IServiceCollection RegisterPipelineBehaviors(this IServiceCollection self, Type service, Type[] implementations) {
        // Pipelines need a slightly different way to be registered.
        foreach (var pipelineBehavior in implementations) {

            // if the pipeline is not an open generic, we need to find all
            // interfaces that closes and register them.
            if (!pipelineBehavior.IsOpenGeneric()) {
                var interfaces = pipelineBehavior.GetInterfacesThatClose(service);
                foreach (var @interface in interfaces) {
                    self.TryAddTransient(@interface, pipelineBehavior);
                }

                continue;
            }

            // if we reach here, means that the pipeline behavior is an open generic type.
            var genericDefinitions = pipelineBehavior.GetInterfaces()
                                                     .Where(service.IsAssignableFromGenericType)
                                                     .Select(type => type.GetGenericTypeDefinition());
            foreach (var genericDefinition in genericDefinitions) {
                self.TryAddTransient(genericDefinition, pipelineBehavior);
            }
        }

        return self;
    }
}