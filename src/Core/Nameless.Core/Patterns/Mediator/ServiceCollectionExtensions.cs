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
    public Type[] RequestHandlerPreProcessors { get; set; } = [];
    public Type[] RequestHandlerPostProcessors { get; set; } = [];
    public Type[] RequestBehaviors { get; set; } = [];

    /// <summary>
    /// Gets an array of stream handler implementations.
    /// </summary>
    /// <remarks>
    /// If <see cref="UseAutoRegister"/> is set to <c>true</c>, this property is ignored.
    /// </remarks>
    public Type[] StreamHandlers { get; set; } = [];
    public Type[] StreamBehaviors { get; set; } = [];
}

public static class ServiceCollectionExtensions {
    public static IServiceCollection RegisterMediatorServices(this IServiceCollection self, Action<MediatorOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new MediatorOptions();

        innerConfigure(options);

        self.RegisterServices(options);

        return self;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection self, MediatorOptions options) {
        const string RequestHandlerProxyKey = "56a2e194-5833-43e5-b58e-6ba17bf429d6";
        const string EventHandlerProxyKey = "8b544d44-bcd7-4fe0-8b46-e980f0eb7139";
        const string StreamHandlerProxyKey = "cc1e729c-c09d-4b6d-b8b6-bf5d72ba00b8";

        self.RegisterEventServices(options)
            .AddKeyedScoped<IRequestHandlerProxy, RequestHandlerProxy>(RequestHandlerProxyKey)
            .AddKeyedScoped<IEventHandlerProxy, EventHandlerProxy>(EventHandlerProxyKey)
            .AddKeyedScoped<IStreamHandlerProxy, StreamHandlerProxy>(StreamHandlerProxyKey)
            .AddScoped<IMediator>(provider => new MediatorImpl(requestHandlerProxy: provider.GetRequiredKeyedService<IRequestHandlerProxy>(RequestHandlerProxyKey),
                                                               eventHandlerProxy: provider.GetRequiredKeyedService<IEventHandlerProxy>(EventHandlerProxyKey),
                                                               streamHandlerProxy: provider.GetRequiredKeyedService<IStreamHandlerProxy>(StreamHandlerProxyKey)));

        return self;
    }

    private static IServiceCollection RegisterEventServices(this IServiceCollection self, MediatorOptions options) {
        var eventHandlers = options.UseAutoRegister
            ? options.SupportAssemblies
                     .SelectMany(assembly => assembly.SearchForImplementations(typeof(IEventHandler<>)))
                     .ToArray()
            : options.EventHandlers;

        return self.RegisterHandlers(typeof(IEventHandler<>), eventHandlers);
    }

    private static IServiceCollection RegisterRequestServices(this IServiceCollection self, MediatorOptions options) {
        var eventHandlers = options.UseAutoRegister
            ? options.SupportAssemblies
                     .SelectMany(assembly => assembly.SearchForImplementations(typeof(IRequestHandler<>)))
                     .ToArray()
            : options.RequestHandlers;

        return self.RegisterHandlers(typeof(IEventHandler<>), eventHandlers);
    }

    private static IServiceCollection RegisterStreamServices(this IServiceCollection self, MediatorOptions options) {
        var eventHandlers = options.UseAutoRegister
            ? options.SupportAssemblies
                     .SelectMany(assembly => assembly.SearchForImplementations(typeof(IEventHandler<>)))
                     .ToArray()
            : options.StreamHandlers;

        return self.RegisterHandlers(typeof(IEventHandler<>), eventHandlers);
    }

    private static IServiceCollection RegisterHandlers(this IServiceCollection self, Type handlerServiceType, IEnumerable<Type> handlerImplementations) {
        // For each handler implementation
        foreach (var handlerImplementation in handlerImplementations) {
            // For each interface implemented by the handler
            foreach (var handlerInterface in handlerImplementation.GetInterfaces()) {
                // Is the interface assignable to the handler service type?
                if (!handlerServiceType.IsAssignableFromOpenGenericType(handlerInterface)) {
                    continue;
                }

                // Register the interface as service of the implementation.
                self.AddScoped(serviceType: handlerInterface,
                               implementationType: handlerImplementation);
            }
            // Registers as self
            self.AddScoped(serviceType: handlerImplementation,
                           implementationType: handlerImplementation);
        }

        return self;
    }
}
