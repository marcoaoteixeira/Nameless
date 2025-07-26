using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     The default implementation of <see cref="IServiceResolver"/>.
/// </summary>
public class ServiceResolver : IServiceResolver {
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ConcurrentDictionary<Type, ObjectFactory> _factories = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceResolver"/>
    ///     class.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The service provider to use for resolving dependencies.
    /// </param>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor to use for accessing the current
    ///     HTTP context.
    /// </param>
    public ServiceResolver(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) {
        _serviceProvider = Prevent.Argument.Null(serviceProvider);
        _httpContextAccessor = Prevent.Argument.Null(httpContextAccessor);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     This method will try to create the instance of the specified
    ///     service using the provided <paramref name="provider"/>. If
    ///     the provider is <see langword="null"/>, it will use the current
    ///     HTTP context's request services. If that is also
    ///     <see langword="null"/>, it will fall back to the default service
    ///     provider.
    /// </remarks>
    public object CreateInstance(Type service, IServiceProvider? provider = null) {
        var factory = _factories.GetOrAdd(service, FactoryInitializer);

        var serviceProvider = provider ??
                              _httpContextAccessor.HttpContext?.RequestServices ??
                              _serviceProvider;

        return factory(serviceProvider, arguments: null);

        static ObjectFactory FactoryInitializer(Type type) {
            return ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
        }
    }
}