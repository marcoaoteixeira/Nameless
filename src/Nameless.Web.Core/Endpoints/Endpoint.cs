using Microsoft.AspNetCore.Http;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints;

/// <summary>
///     Represents a minimal endpoint in the application.
/// </summary>
public interface IEndpoint {
    /// <summary>
    ///     Creates the endpoint descriptor for this endpoint.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEndpointDescriptor"/> that describes the endpoint.
    /// </returns>
    IEndpointDescriptor Describe();
}

/// <summary>
///     A base class for defining a minimal endpoint with a request type.
/// </summary>
/// <typeparam name="TRequest">
///     Type of the request that this endpoint handles.
/// </typeparam>
public interface IEndpoint<in TRequest> : IEndpoint
    where TRequest : notnull {

    Task<IResult> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}
