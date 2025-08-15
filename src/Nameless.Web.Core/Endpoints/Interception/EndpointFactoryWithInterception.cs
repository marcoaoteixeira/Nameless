using System.Diagnostics.CodeAnalysis;
using Castle.DynamicProxy;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Endpoints.Infrastructure;

namespace Nameless.Web.Endpoints.Interception;
public class EndpointFactoryWithInterception : IEndpointFactory {
    private readonly IEndpointWrapperGenerator _wrapperGenerator;
    private readonly IProxyGenerator _proxyGenerator;
    private readonly IServiceFactory _serviceFactory;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointFactory"/> class.
    /// </summary>
    public EndpointFactoryWithInterception(
        IEndpointWrapperGenerator wrapperGenerator,
        IProxyGenerator proxyGenerator,
        IServiceFactory serviceFactory) {
        _wrapperGenerator = Guard.Against.Null(wrapperGenerator);
        _proxyGenerator = Guard.Against.Null(proxyGenerator);
        _serviceFactory = Guard.Against.Null(serviceFactory);
    }

    /// <inheritdoc />
    public EndpointCall Create(IEndpointDescriptor descriptor) {
        Guard.Against.Null(descriptor);

        var endpoint = (IEndpoint)_serviceFactory.Create(descriptor.EndpointType);

        if (TryCreateWrapper(descriptor, endpoint, out var proxy)) {
            var handler = proxy.GetType()
                               .GetMethod(descriptor.ActionName)
                          ?? throw new InvalidOperationException("Could not find endpoint action handler in endpoint proxy.");

            return new EndpointCall(proxy, handler);
        }

        return new EndpointCall(endpoint, descriptor.GetEndpointHandler());
    }

    private bool TryCreateWrapper(IEndpointDescriptor descriptor, IEndpoint currentInstance, [NotNullWhen(returnValue: true)] out IEndpoint? endpoint) {
        endpoint = null;

        var metadataCollection = descriptor.GetAdditionalMetadata<EndpointInterceptorMetadata>()
                                           .ToArray();
        if (metadataCollection.Length == 0) {
            return false;
        }

        var interceptors = metadataCollection.Select(metadata => (EndpointInterceptorBase)_serviceFactory.Create(metadata.Type))
                                             .Where(interceptor => interceptor.CanIntercept(descriptor))
                                             .Cast<IInterceptor>()
                                             .ToArray();

        // if there are no interceptors, let's just return
        // the current instance of the endpoint.
        if (interceptors.Length == 0) {
            return false;
        }

        var wrapper = _wrapperGenerator.Create(descriptor);
        var wrapperInstance = Activator.CreateInstance(wrapper)
                              ?? throw new InvalidOperationException("Could not create endpoint wrapper instance.");

        _wrapperGenerator.SetWrapperTarget(wrapperInstance, currentInstance);

        var proxyWithInterceptors = _proxyGenerator.CreateClassProxyWithTarget(
            classToProxy: wrapper,
            additionalInterfacesToProxy: [typeof(IEndpoint)],
            target: wrapperInstance,
            options: ProxyGenerationOptions.Default,
            constructorArguments: [],
            interceptors: interceptors
        );

        endpoint = (IEndpoint)proxyWithInterceptors;

        return true;
    }
}
