using System.Diagnostics;
using Nameless.Mediator.Requests;
using Nameless.Microservices.Application.Monitoring;

namespace Nameless.Microservices.Application.Behaviors;

/// <summary>
/// Performance pipeline behavior. 
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public sealed class PerformancePipelineBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse> where TRequest : IRequestBase {
    private const string ACTIVITY_SOURCE_NAME = "Request Handlers Performance";

    private readonly IActivitySourceProvider _activitySourceProvider;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="PerformancePipelineBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="activitySourceProvider">The activity source provider.</param>
    public PerformancePipelineBehavior(IActivitySourceProvider activitySourceProvider, TimeProvider timeProvider) {
        _activitySourceProvider = Prevent.Argument.Null(activitySourceProvider);
        _timeProvider = Prevent.Argument.Null(timeProvider);
    }

    /// <inheritdoc />
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        var activityName = $"Executing request handler for request '{typeof(TRequest).GetPrettyName()}' with response '{typeof(TResponse).GetPrettyName()}'";

        using var activity = _activitySourceProvider
            .GetActivitySource(ACTIVITY_SOURCE_NAME)
            .StartActivity(ActivityKind.Internal, startTime: _timeProvider.GetUtcNow(), name: activityName);

        return next(cancellationToken);
    }
}
