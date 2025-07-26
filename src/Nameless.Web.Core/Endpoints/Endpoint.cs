using Microsoft.AspNetCore.Http;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints;

/// <summary>
///     Defines a minimal endpoint that can be used in the application.
/// </summary>
public interface IEndpoint {
    /// <summary>
    ///     Describes the endpoint.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEndpointDescriptor"/> that describes the endpoint.
    /// </returns>
    IEndpointDescriptor Describe();
}

public abstract class EndpointBase<TRequest> : IEndpoint
    where TRequest : notnull {
    /// <inheritdoc />
    public virtual IEndpointDescriptor Describe() {
        return EndpointDescriptorBuilder.Create(GetType())
                                        .Get($"/{GetType().Name.ToSnakeCase('-')}", nameof(ExecuteAsync))
                                        .Build();
    }

    /// <summary>
    ///     Executes the endpoint with the given request and cancellation token.
    /// </summary>
    /// <param name="request">
    ///     The request to execute the endpoint with.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token to use for the execution of the endpoint.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, where the result
    ///     is an <see cref="IResult"/>.
    /// </returns>
    public abstract Task<IResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}