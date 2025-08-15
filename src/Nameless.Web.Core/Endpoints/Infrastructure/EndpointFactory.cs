using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

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
        _serviceFactory = Guard.Against.Null(serviceFactory);
    }

    /// <inheritdoc />
    public EndpointCall Create(IEndpointDescriptor descriptor) {
        Guard.Against.Null(descriptor);

        var endpoint = (IEndpoint)_serviceFactory.Create(descriptor.EndpointType);

        return new EndpointCall(endpoint, descriptor.GetEndpointHandler());
    }
}