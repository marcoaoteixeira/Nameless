using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Web.Endpoints.Definitions;

namespace Nameless.Web.Endpoints.Infrastructure;

public interface IEndpointInterceptor : IInterceptor {
    bool CanIntercept(IEndpointDescriptor descriptor);

    Task<IResult> InterceptAsync(object[] arguments, CancellationToken cancellationToken);
}

public abstract class EndpointInterceptor : IEndpointInterceptor {
    private readonly Lazy<ILogger<EndpointInterceptor>> _logger;

    protected IHttpContextAccessor HttpContextAccessor { get; }

    protected ILogger<EndpointInterceptor> Logger => _logger.Value;

    protected EndpointInterceptor(IHttpContextAccessor httpContextAccessor) {
        HttpContextAccessor = Prevent.Argument.Null(httpContextAccessor);

        _logger = new Lazy<ILogger<EndpointInterceptor>>(CreateLogger);
    }

    public abstract bool CanIntercept(IEndpointDescriptor descriptor);

    public abstract Task<IResult> InterceptAsync(object[] arguments, CancellationToken cancellationToken);

    void IInterceptor.Intercept(IInvocation invocation) {
        var httpContext = HttpContextAccessor.HttpContext;
        var endpointDescriptor = httpContext?.GetEndpointDescriptorMetadata();

        if (endpointDescriptor is null) {
            throw new InvalidOperationException("Whoops, doesn't seem to be a valid endpoint.");
        }

        if (!string.Equals(invocation.Method.Name, endpointDescriptor.Descriptor.Action, StringComparison.CurrentCulture)) {
            // not the method that we want to intercept.
            invocation.Proceed();

            return;
        }

        // Tries to get the current HTTP context cancellation token.
        var cancellationToken = httpContext?.RequestAborted ?? CancellationToken.None;

        try {
            var intercept = InterceptAsync(invocation.Arguments, cancellationToken);
            var result = intercept.WaitAsync(cancellationToken)
                                  .ConfigureAwait(continueOnCapturedContext: false)
                                  .GetAwaiter()
                                  .GetResult();

            if (result is not EmptyHttpResult) {
                invocation.ReturnValue = Task.FromResult(result);

                return;
            }

            invocation.Proceed();
        }
        catch (Exception ex) {
            Logger.LogError(ex, "Error white intercepting endpoint.");

            throw;
        }
    }

    private ILogger<EndpointInterceptor> CreateLogger() {
        return HttpContextAccessor.HttpContext
                                  ?.RequestServices
                                  .GetService<ILogger<EndpointInterceptor>>()
               ?? NullLogger<EndpointInterceptor>.Instance;
    }
}
