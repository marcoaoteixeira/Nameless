namespace Nameless.CQRS.Requests;

public interface IRequest;

public interface IRequest<out TResponse> : IRequest;