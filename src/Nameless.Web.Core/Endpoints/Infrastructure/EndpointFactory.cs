using Castle.DynamicProxy;
using Microsoft.AspNetCore.Http;
using Nameless.Validation;
using Nameless.Web.Endpoints.Definitions;

using HttpResults = Microsoft.AspNetCore.Http.Results;

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
    public IEndpoint Create(IEndpointDescriptor descriptor) {
        // we might need to create a proxy for the endpoint
        // if it needs validation or pre-execution actions.

        // let's go easy now and just check if it needs validation.
        var endpoint = (IEndpoint)_serviceResolver.CreateInstance(descriptor.EndpointType);

        var proxyFactory = (IEndpointProxyFactory)_serviceResolver.CreateInstance(typeof(EndpointProxyFactory));

        var proxy = proxyFactory.CreateProxy(endpoint, descriptor);

        return (IEndpoint)proxy;
    }
}

/// <summary>
///     Defines a factory for creating endpoint proxies.
/// </summary>
public interface IEndpointProxyFactory {
    /// <summary>
    ///     Creates a proxy for the specified endpoint.
    /// </summary>
    /// <param name="current">
    ///     The current endpoint instance.
    /// </param>
    /// <param name="descriptor">
    ///     The endpoint descriptor.
    /// </param>
    /// <returns>
    ///     The proxy instance of the specified endpoint type,
    /// </returns>
    IEndpoint CreateProxy(IEndpoint current, IEndpointDescriptor descriptor);
}

public class EndpointProxyFactory : IEndpointProxyFactory {
    private readonly IProxyGenerator _proxyGenerator;
    private readonly IEndpointInterceptor[] _interceptors = [];

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="EndpointProxyFactory"/> class.
    /// </summary>
    /// <param name="proxyGenerator">
    ///     The proxy generator used to create proxies for endpoints.
    /// </param>
    public EndpointProxyFactory(IProxyGenerator proxyGenerator, IEnumerable<IEndpointInterceptor> interceptors) {
        _proxyGenerator = Prevent.Argument.Null(proxyGenerator);
        _interceptors = Prevent.Argument.Null(interceptors).ToArray();
    }

    /// <inheritdoc />
    public IEndpoint CreateProxy(IEndpoint current, IEndpointDescriptor descriptor) {
        Prevent.Argument.Null(descriptor);

        // maybe get the interceptors from the descriptor?
        var interceptors = _interceptors.Where(interceptor => interceptor.CanIntercept(descriptor.EndpointType))
                                        .Cast<IInterceptor>()
                                        .ToArray();

        // Create a proxy for the endpoint
        var proxy = _proxyGenerator.CreateInterfaceProxyWithTarget(
            target: current,
            additionalInterfacesToProxy: [typeof(IEndpoint)],
            options: new ProxyGenerationOptions {
                BaseTypeForInterfaceProxy = descriptor.EndpointType
            },
            interceptors: interceptors,
            interfaceToProxy: typeof(IEndpoint)
        );

        return (IEndpoint)proxy;
    }
}

public interface IEndpointInterceptor : IInterceptor {
    /// <summary>
    ///     Checks if the interceptor can intercept the specified endpoint
    ///     type.
    /// </summary>
    /// <param name="endpointType">
    ///     Type of the endpoint.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if the interceptor can intercept;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    bool CanIntercept(Type endpointType);
}

public class ValidateEndpointInterceptor : IEndpointInterceptor {
    private readonly IValidationService _validationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="ValidateEndpointInterceptor"/> class.
    /// </summary>
    /// <param name="validationService">
    ///     The validation service used to validate the endpoint
    ///     requests objects.
    /// </param>
    public ValidateEndpointInterceptor(IValidationService validationService, IHttpContextAccessor httpContextAccessor) {
        _validationService = Prevent.Argument.Null(validationService);
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
    }

    /// <inheritdoc />
    public bool CanIntercept(Type endpointType) {
        return typeof(IEndpoint).IsAssignableFrom(endpointType);
    }

    public void Intercept(IInvocation invocation) {
        var cancellationToken = _httpContextAccessor
                                .HttpContext?
                                .RequestAborted ?? CancellationToken.None;

        invocation.ReturnValue = InterceptAsync(invocation, cancellationToken);
    }

    private async Task InterceptAsync(IInvocation invocation, CancellationToken cancellationToken) {
        var arguments = invocation.Arguments.Where(ValidateAttribute.IsPresent);

        foreach (var arg in arguments) {
            var result = await _validationService.ValidateAsync(arg, cancellationToken);

            if (result.Succeeded) {
                continue;
            }

            invocation.ReturnValue = HttpResults.ValidationProblem(result.ToDictionary());

            return;
        }
    }
}