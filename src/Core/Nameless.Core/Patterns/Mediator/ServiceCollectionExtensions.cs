using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Patterns.Mediator.Events;
using Nameless.Patterns.Mediator.Requests;
using Nameless.Patterns.Mediator.Streams;

namespace Nameless.Patterns.Mediator;

public record MediatorOptions {
    public bool UseAutoRegister { get; set; } = true;
    public Assembly[] SupportAssemblies { get; set; } = [];

    /// <summary>
    /// Gets an array of event handler implementations.
    /// </summary>
    /// <remarks>
    /// If <see cref="UseAutoRegister"/> is set to <c>true</c>, this property is ignored.
    /// </remarks>
    public Type[] EventHandlers { get; set; } = [];

    /// <summary>
    /// Gets an array of request handler implementations.
    /// </summary>
    /// <remarks>
    /// If <see cref="UseAutoRegister"/> is set to <c>true</c>, this property is ignored.
    /// </remarks>
    public Type[] RequestHandlers { get; set; } = [];
    public Type[] RequestPipelineBehaviors { get; set; } = [];

    /// <summary>
    /// Gets an array of stream handler implementations.
    /// </summary>
    /// <remarks>
    /// If <see cref="UseAutoRegister"/> is set to <c>true</c>, this property is ignored.
    /// </remarks>
    public Type[] StreamHandlers { get; set; } = [];
    public Type[] StreamPipelineBehaviors { get; set; } = [];
}

public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterMediatorServices(this IServiceCollection self, Action<MediatorOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new MediatorOptions();

        innerConfigure(options);

        return self
               .RegisterEventHandlers(options)

               .RegisterRequestHandlers(options)
               .RegisterRequestPipelineBehaviors(options)

               .RegisterStreamHandlers(options)
               .RegisterStreamPipelineBehaviors(options)

               .RegisterMediator();
    }

    private static IServiceCollection RegisterEventHandlers(this IServiceCollection self, MediatorOptions options) {
        var serviceType = typeof(IEventHandler<>);
        var eventHandlers = options.UseAutoRegister
            ? options.SupportAssemblies
                     .GetImplementations([serviceType])
                     .ToArray()
            : options.EventHandlers;

        return self
                .AddSingleton<IEventHandlerInvoker, EventHandlerInvoker>()
                .RegisterImplementations(serviceType, eventHandlers);
    }

    private static IServiceCollection RegisterRequestHandlers(this IServiceCollection self, MediatorOptions options) {
        var serviceTypes = new[] { typeof(IRequestHandler<>), typeof(IRequestHandler<,>) };
        var requestHandlers = options.UseAutoRegister
            ? options.SupportAssemblies
                     .GetImplementations(serviceTypes)
                     .ToArray()
            : options.RequestHandlers;

        return self
               .AddSingleton<IRequestHandlerInvoker, RequestHandlerInvoker>()
               .RegisterImplementations(serviceTypes[0], requestHandlers)
               .RegisterImplementations(serviceTypes[1], requestHandlers);
    }

    private static IServiceCollection RegisterRequestPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        var serviceType = typeof(IRequestPipelineBehavior<,>);
        var requestPipelineBehaviors = options.UseAutoRegister
            ? options.SupportAssemblies
                     .GetImplementations([serviceType])
                     .ToArray()
            : options.RequestPipelineBehaviors;

        return self
               .RegisterImplementations(serviceType, requestPipelineBehaviors);
    }

    private static IServiceCollection RegisterStreamHandlers(this IServiceCollection self, MediatorOptions options) {
        var serviceType = typeof(IStreamHandler<,>);
        var streamHandlers = options.UseAutoRegister
            ? options.SupportAssemblies
                     .GetImplementations([serviceType])
                     .ToArray()
            : options.StreamHandlers;

        return self
               .AddSingleton<IStreamHandlerInvoker, StreamHandlerInvoker>()
               .RegisterImplementations(serviceType, streamHandlers);
    }

    private static IServiceCollection RegisterStreamPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        var serviceType = typeof(IStreamPipelineBehavior<,>);
        var streamPipelineBehaviors = options.UseAutoRegister
            ? options.SupportAssemblies
                     .GetImplementations([serviceType])
                     .ToArray()
            : options.StreamPipelineBehaviors;

        return self
            .RegisterImplementations(serviceType, streamPipelineBehaviors);
    }

    private static IEnumerable<Type> GetImplementations(this Assembly[] self, Type[] serviceTypes)
        => serviceTypes.SelectMany(serviceType => self.SelectMany(assembly => assembly.GetImplementationsFor(serviceType)));

    private static IServiceCollection RegisterImplementations(this IServiceCollection self, Type serviceType, IEnumerable<Type> implementations) {
        // For each implementation
        foreach (var implementation in implementations) {
            // For each interface in the implementation
            foreach (var implementationInterface in implementation.GetInterfaces()) {
                // Is the interface assignable to the service type?
                if (!serviceType.IsAssignableFromOpenGenericType(implementationInterface)) {
                    continue;
                }

                // Register the interface as service of the implementation.
                self.AddTransient(serviceType: implementationInterface,
                                  implementationType: implementation);
            }
        }
        return self;
    }

    private static IServiceCollection RegisterMediator(this IServiceCollection self)
        => self.AddSingleton<IMediator>(provider => new MediatorImpl(eventHandlerInvoker: provider.GetRequiredService<IEventHandlerInvoker>(),
                                                                     requestHandlerInvoker: provider.GetRequiredService<IRequestHandlerInvoker>(),
                                                                     streamHandlerInvoker: provider.GetRequiredService<IStreamHandlerInvoker>()));
}
