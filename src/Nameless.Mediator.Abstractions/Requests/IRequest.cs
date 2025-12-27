#pragma warning disable S2326 // Ignore ‘T’ is not used in the interface

namespace Nameless.Mediator.Requests;

/// <summary>
///     Defines a request that return a response.
/// </summary>
/// <typeparam name="TResponse">
///     Type of the response.
/// </typeparam>
public interface IRequest<out TResponse>;