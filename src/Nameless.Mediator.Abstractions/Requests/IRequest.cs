#pragma warning disable S2326 // Ignore ‘T’ is not used in the interface

namespace Nameless.Mediator.Requests;

/// <summary>
///     Allows for generic type constraints of objects implementing IRequest
///     or IRequest{TResponse}
/// </summary>
public interface IRequestBase;

/// <summary>
///     The base interface for all requests.
/// </summary>
public interface IRequest : IRequestBase;

/// <summary>
///     The base interface for all requests that return a response.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IRequest<out TResponse> : IRequestBase;
