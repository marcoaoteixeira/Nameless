﻿using System.Reflection;
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
    /// <summary>
    ///     Registers the mediator services.
    /// </summary>
    /// <param name="self">
    ///     The current <see cref="IServiceCollection"/>.
    /// </param>
    /// <param name="configure">
    ///     The configuration action.
    /// </param>
    /// <returns>
    ///     The current <see cref="IServiceCollection"/> so other
    ///     actions can be chained.
    /// </returns>
    public static IServiceCollection RegisterMediator(this IServiceCollection self, Action<MediatorOptions>? configure = null) {
        // We are not going to register the configuration
        // action since there are no services that use
        // it after they have been registered.
        var innerConfigure = configure ?? (_ => { });
        var options = new MediatorOptions();

        innerConfigure(options);

        self.RegisterEventHandlers(options);

        self.RegisterRequestHandlers(options);
        self.RegisterRequestPipelineBehaviors(options);

        self.RegisterStreamHandlers(options);
        self.RegisterStreamPipelineBehaviors(options);

        self.TryAddTransient<IEventHandlerInvoker, EventHandlerInvoker>();
        self.TryAddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();
        self.TryAddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();
        self.TryAddTransient<IMediator, MediatorImpl>();

        return self;
    }

    private static void RegisterEventHandlers(this IServiceCollection self, MediatorOptions options) {
        var service = typeof(IEventHandler<>);
        var implementations = options.Assemblies.GetImplementations(service);

        self.RegisterImplementations(
            service,
            implementations,
            options.Assemblies
        );
    }

    private static void RegisterRequestHandlers(this IServiceCollection self, MediatorOptions options) {
        var service = typeof(IRequestHandler<,>);
        var implementations = options.Assemblies.GetImplementations(service);

        self.RegisterImplementations(
            service,
            implementations,
            options.Assemblies
        );
    }

    private static void RegisterRequestPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        self.RegisterImplementations(
            typeof(IRequestPipelineBehavior<,>),
            options.RequestPipelineBehaviors,
            options.Assemblies
        );
    }

    private static void RegisterStreamHandlers(this IServiceCollection self, MediatorOptions options) {
        var service = typeof(IStreamHandler<,>);
        var implementations = options.Assemblies.GetImplementations(service);

        self.RegisterImplementations(
            service,
            implementations,
            options.Assemblies
        );
    }

    private static void RegisterStreamPipelineBehaviors(this IServiceCollection self, MediatorOptions options) {
        self.RegisterImplementations(
            typeof(IStreamPipelineBehavior<,>),
            options.StreamPipelineBehaviors,
            options.Assemblies
        );
    }

    private static void RegisterImplementations(this IServiceCollection self, Type genericDefinition, IEnumerable<Type> implementations, Assembly[] assemblies) {
        var descriptors = new List<ServiceDescriptor>();

        foreach (var implementation in implementations) {
            var items = implementation.IsOpenGeneric()
                ? CreateServiceDescriptorsForGenericDefinition(genericDefinition, implementation, assemblies)
                : CreateServiceDescriptorsForConcrete(genericDefinition, implementation);

            descriptors.AddRange(items);
        }

        // We might have multiple implementations for the same service type.
        // So, in order to be able to resolve all similar services at once
        // we're going to register them using TryAddEnumerable method.
        foreach (var descriptorGroup in descriptors.GroupBy(descriptor => descriptor.ServiceType)) {
            self.TryAddEnumerable(descriptorGroup);
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateServiceDescriptorsForConcrete(Type genericDefinition, Type implementation) {
        // If the implementation is a generic type definition, then skip it.
        // We only want closed types here.
        if (implementation.IsGenericTypeDefinition) {
            yield break;
        }

        // We need to find all closed interfaces or abstract classes for
        // the implementation.
        var types = implementation.GetInterfacesThatCloses(genericDefinition);

        foreach (var type in types) {
            yield return ServiceDescriptor.Transient(type, implementation);
        }
    }

    private static IEnumerable<ServiceDescriptor> CreateServiceDescriptorsForGenericDefinition(Type genericDefinition, Type implementation, Assembly[] assemblies) {
        // If the implementation is not a generic type definition, then skip it.
        // We only want generic type definitions here.
        if (!implementation.IsGenericTypeDefinition) {
            yield break;
        }

        // We need to close the generic definition in order to be able
        // to resolve it later.
        var argumentsThatCloseGroup = GenericTypeHelper.GetArgumentsThatCloses(implementation, assemblies);
        foreach (var argumentsThatClose in argumentsThatCloseGroup) {
            var concrete = implementation.MakeGenericType(argumentsThatClose);
            var descriptors = CreateServiceDescriptorsForConcrete(genericDefinition, concrete);

            foreach (var descriptor in descriptors) {
                yield return descriptor;
            }
        }
    }
}