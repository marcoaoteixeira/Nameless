using System.Diagnostics;
using Nameless.Barebones.Infrastructure.Monitoring;
using Nameless.Mediator.Requests;

namespace Nameless.Barebones.Infrastructure.Behaviors;

/// <summary>
/// Performance pipeline behavior. 
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class PerformancePipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse> where TRequest : IRequestBase {
    private readonly IActivitySourceManager _activitySourceManager;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="PerformancePipelineBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="activitySourceManager">The activity source provider.</param>
    public PerformancePipelineBehavior(IActivitySourceManager activitySourceManager, TimeProvider timeProvider) {
        _activitySourceManager = Prevent.Argument.Null(activitySourceManager);
        _timeProvider = Prevent.Argument.Null(timeProvider);
    }

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var requestName = typeof(TRequest).GetPrettyName();
        var responseName = typeof(TResponse).GetPrettyName();
        var sourceName = $"IRequestHandler<{requestName}, {responseName}>";
        var activityName = $"Executing request handler '{sourceName}'";

        using var activity = _activitySourceManager.GetActivitySource(sourceName)
                                                   .StartActivity(
                                                       ActivityKind.Internal,
                                                       startTime: _timeProvider.GetUtcNow(),
                                                       name: activityName
                                                    );

        // We need to wait for the activity to be registered before proceeding
        return await next(cancellationToken);
    }
}
