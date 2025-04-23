using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

public record MediatorOptions {
    public bool UseAutoRegister { get; set; } = true;
    public Assembly[] SupportAssemblies { get; set; } = [];
    public ServiceDescriptor[] Behaviors { get; set; } = [];
    public ServiceDescriptor[] RequestHandlers { get; set; } = [];
    public ServiceDescriptor[] RequestHandlerPreProcessors { get; set; } = [];
    public ServiceDescriptor[] RequestHandlerPostProcessors { get; set; } = [];
    public ServiceDescriptor[] EventHandlers { get; set; } = [];
    public ServiceDescriptor[] StreamHandlers { get; set; } = [];
    public ServiceDescriptor[] StreamHandlerBehaviors { get; set; } = [];
}

public static class ServiceCollectionExtension {
    public static IServiceCollection RegisterMediator(this IServiceCollection self, Action<MediatorOptions>? configure = null) {
        var innerConfigure = configure ?? (_ => { });
        var options = new MediatorOptions();

        innerConfigure(options);

        self.RegisterServices();

        return self;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection self)
        => self.AddTransient<IRequestHandlerProxy, RequestHandlerProxy>()
               .AddTransient<IEventHandlerProxy, EventHandlerProxy>()
               .AddTransient<IStreamHandlerProxy, StreamHandlerProxy>();
}
