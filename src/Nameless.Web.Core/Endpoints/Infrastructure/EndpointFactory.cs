using Castle.DynamicProxy;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

public class EndpointFactory : IEndpointFactory {
    private readonly IServiceResolver _serviceResolver;
    private readonly IEndpointWrapperGenerator _endpointWrapperGenerator;
    private readonly IProxyGenerator _proxyGenerator;
    private readonly IEndpointInterceptor[] _interceptors;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointFactory"/> class.
    /// </summary>
    public EndpointFactory(
        IServiceResolver serviceResolver,
        IEndpointWrapperGenerator endpointWrapperGenerator,
        IProxyGenerator proxyGenerator,
        IEnumerable<IEndpointInterceptor> interceptors) {
        _serviceResolver = Prevent.Argument.Null(serviceResolver);
        _endpointWrapperGenerator = Prevent.Argument.Null(endpointWrapperGenerator);
        _proxyGenerator = Prevent.Argument.Null(proxyGenerator);
        _interceptors = Prevent.Argument.Null(interceptors).ToArray();
    }

    /// <inheritdoc />
    public EndpointInfo Create(IEndpointDescriptor descriptor) {
        Prevent.Argument.Null(descriptor);

        var endpoint = (IEndpoint)_serviceResolver.CreateInstance(descriptor.EndpointType);

        if (descriptor.UseInterceptors) {
            endpoint = CreateWrapper(descriptor, endpoint);
        }

        var handler = endpoint.GetType()
                              .GetMethod(descriptor.GetAction().Name)!;

        return new EndpointInfo(endpoint, handler);
    }

    private IEndpoint CreateWrapper(IEndpointDescriptor descriptor, IEndpoint endpoint) {
        var interceptors = _interceptors.Where(interceptor => interceptor.CanIntercept(descriptor))
                                        .Cast<IInterceptor>()
                                        .ToArray();

        // if there are no interceptors, let's just return
        // the current instance of the endpoint.
        if (interceptors.Length == 0) {
            return endpoint;
        }

        var wrapper = _endpointWrapperGenerator.Create(descriptor);
        var wrapperInstance = Activator.CreateInstance(wrapper)
            ?? throw new InvalidOperationException("Could not create endpoint wrapper instance.");

        _endpointWrapperGenerator.SetWrapperTarget(wrapperInstance, endpoint);

        var proxy = _proxyGenerator.CreateClassProxyWithTarget(
            classToProxy: wrapper,
            additionalInterfacesToProxy: [typeof(IEndpoint)],
            target: wrapperInstance,
            options: ProxyGenerationOptions.Default,
            constructorArguments: [],
            interceptors: interceptors
        );

        return (IEndpoint)proxy;
    }
}