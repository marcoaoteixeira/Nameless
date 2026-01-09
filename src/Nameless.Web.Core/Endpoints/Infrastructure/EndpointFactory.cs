using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Default implementation of <see cref="IEndpointFactory"/>.
/// </summary>
public class EndpointFactory : IEndpointFactory {
    private readonly IServiceFactory _serviceFactory;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointFactory"/> class.
    /// </summary>
    /// <param name="serviceFactory">
    ///     The endpoint resolver.
    /// </param>
    public EndpointFactory(IServiceFactory serviceFactory) {
        _serviceFactory = serviceFactory;
    }

    /// <inheritdoc />
    public EndpointCall Create(IEndpointDescriptor descriptor) {
        var endpoint = (IEndpoint)_serviceFactory.Create(descriptor.EndpointType);

        return new EndpointCall(endpoint, descriptor.GetEndpointHandler());
    }
}