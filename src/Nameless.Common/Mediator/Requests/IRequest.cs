#pragma warning disable S2326 // Ignore ‘T’ is not used in the interface

namespace Nameless.Mediator.Requests;

/// <summary>
///     Represents a request without a return value.
/// </summary>
public interface IRequest;

/// <summary>
///     Represents a request with a return value.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IRequest<out TResponse> : IRequest;