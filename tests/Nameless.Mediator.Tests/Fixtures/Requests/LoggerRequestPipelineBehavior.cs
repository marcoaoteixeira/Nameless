using Microsoft.Extensions.Logging;
using Nameless.Mediator.Requests;

namespace Nameless.Mediator.Fixtures.Requests;

public class LoggerRequestPipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull {
    private readonly ILogger _logger;

    public LoggerRequestPipelineBehavior(ILogger logger) {
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        _logger.LogInformation("Executing LoggerRequestPipelineBehavior for request ({RequestName}): {@Request}", typeof(TRequest).GetPrettyName(), request);

        return await next(cancellationToken);
    }
}
