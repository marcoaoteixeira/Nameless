using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Endpoints.Definitions.Metadata;

namespace Nameless.Web.Endpoints.Interception;

public abstract class EndpointInterceptorBase : IInterceptor {
    private readonly Lazy<ILogger<EndpointInterceptorBase>> _logger;

    protected IHttpContextAccessor HttpContextAccessor { get; }

    protected ILogger<EndpointInterceptorBase> Logger => _logger.Value;

    protected EndpointInterceptorBase(IHttpContextAccessor httpContextAccessor) {
        HttpContextAccessor = Guard.Against.Null(httpContextAccessor);

        _logger = new Lazy<ILogger<EndpointInterceptorBase>>(CreateLogger);
    }

    public virtual bool CanIntercept(IEndpointDescriptor descriptor) {
        Guard.Against.Null(descriptor);

        return typeof(IEndpoint).IsAssignableFrom(descriptor.EndpointType);
    }

    public abstract Task<IResult> InterceptAsync(HttpContext httpContext, object?[] arguments, CancellationToken cancellationToken);

    void IInterceptor.Intercept(IInvocation invocation) {
        var httpContext = HttpContextAccessor.HttpContext;
        if (httpContext is null) {
            throw new InvalidOperationException("HTTP context is not available.");
        }

        var endpointDescriptor = httpContext.GetEndpointDescriptorMetadata();
        if (endpointDescriptor is null) {
            throw new InvalidOperationException("Endpoint descriptor metadata is not available.");
        }

        if (!string.Equals(invocation.Method.Name, endpointDescriptor.Descriptor.ActionName, StringComparison.Ordinal)) {
            // not the method that we want to intercept.
            invocation.Proceed();

            return;
        }

        try {
            var intercept = InterceptAsync(httpContext, invocation.Arguments, httpContext.RequestAborted);
            var result = intercept.WaitAsync(httpContext.RequestAborted)
                                  .ConfigureAwait(continueOnCapturedContext: false)
                                  .GetAwaiter()
                                  .GetResult();

            if (result is ContinueInterceptionResult) {
                invocation.Proceed();

                return;
            }

            invocation.ReturnValue = Task.FromResult(result);
        }
        catch (Exception ex) {
            Logger.InterceptionFailure(
                endpointType: endpointDescriptor.Descriptor.EndpointType,
                interceptorType: GetType(),
                exception: ex
            );

            throw;
        }
    }

    private ILogger<EndpointInterceptorBase> CreateLogger() {
        return HttpContextAccessor.HttpContext
                                  ?.RequestServices
                                  .GetService<ILogger<EndpointInterceptorBase>>()
               ?? NullLogger<EndpointInterceptorBase>.Instance;
    }
}
