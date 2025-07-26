using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Nameless.Validation;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Default implementation of the <see cref="IEndpointFactory"/> interface.
/// </summary>
public class EndpointFactory : IEndpointFactory {
    private readonly IServiceResolver _serviceResolver;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EndpointFactory"/>
    ///     class.
    /// </summary>
    /// <param name="serviceResolver">
    ///     The service resolver used to create instances.
    /// </param>
    public EndpointFactory(IServiceResolver serviceResolver) {
        _serviceResolver = Prevent.Argument.Null(serviceResolver);
    }

    /// <inheritdoc />
    public IEndpoint Create(Type endpointType) {



        var endpoint = _serviceResolver.CreateInstance(endpointType);

        return (IEndpoint)endpoint;
    }
}

public static class EndpointProxyFactory {
    private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();
    public static IEndpoint<TRequest> CreateProxy<TRequest>(IEndpoint<TRequest> endpoint, Action<HttpContext> action)
        where TRequest : notnull {
        Prevent.Argument.Null(endpoint);
        Prevent.Argument.Null(action);

        var interceptor = new PreExecutionInterceptor(action);

        // Create a proxy for the endpoint
        return _proxyGenerator.CreateInterfaceProxyWithTarget<IEndpoint<TRequest>>(endpoint, new EndpointInterceptor());
    }
}

public class ValidationInterceptor : IInterceptor {
    private readonly IValidationService _validationService;

    public ValidationInterceptor(IValidationService validationService) {
        _validationService = Prevent.Argument.Null(validationService);
    }

    public void Intercept(IInvocation invocation) {

        // Perform validation logic here
        // For example, check if the request is valid
        if (invocation.Arguments.Length == 0 || invocation.Arguments[0] == null) {
            throw new ArgumentException("Invalid request");
        }
        // Proceed with the original method call
        invocation.Proceed();
    }
}