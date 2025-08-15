using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Interception;

/// <summary>
///     Extension methods for <see cref="EndpointDescriptorBuilder"/>.
/// </summary>
public static class EndpointDescriptorBuilderExtensions {
    /// <summary>
    ///     Sets an endpoint interceptor to this endpoint.
    /// </summary>
    /// <typeparam name="TEndpointInterceptor">
    ///     Type of the endpoint interceptor.
    /// </typeparam>
    /// <param name="self">
    ///     The current <see cref="EndpointDescriptorBuilder"/>.
    /// </param>
    /// <returns>
    ///     The current <see cref="EndpointDescriptorBuilder"/> so other
    ///     actions can be chained.
    /// </returns>
    public static EndpointDescriptorBuilder WithInterceptor<TEndpointInterceptor>(this EndpointDescriptorBuilder self)
        where TEndpointInterceptor : EndpointInterceptorBase {
        Guard.Against.Null(self);

        var metadata = new EndpointInterceptorMetadata(typeof(TEndpointInterceptor));

        self.WithAdditionalMetadata(metadata);

        return self;
    }
}

/// <summary>
///     Represents an endpoint interceptor metadata.
/// </summary>
public record EndpointInterceptorMetadata {
    /// <summary>
    ///     Gets the type of the interceptor.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointInterceptorMetadata"/> class.
    /// </summary>
    /// <param name="endpointInterceptorType">
    ///     Type of the endpoint interceptor.
    /// </param>
    public EndpointInterceptorMetadata(Type endpointInterceptorType) {
        Guard.Against.Null(endpointInterceptorType);
        Guard.Against.NotAssignableFrom<EndpointInterceptorBase>(endpointInterceptorType);

        Type = endpointInterceptorType;
    }
}