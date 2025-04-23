namespace Nameless.Mediator.Requests;

/// <summary>
/// The base interface for all requests.
/// </summary>
public interface IRequest;

/// <summary>
/// The base interface for all requests that return a response.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IRequest<out TResponse> : IRequest;