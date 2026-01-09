using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     The default implementation of <see cref="IServiceFactory"/>.
/// </summary>
public class ServiceFactory : IServiceFactory {
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly ConcurrentDictionary<Type, ObjectFactory> _factories = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServiceFactory"/>
    ///     class.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The service provider to use for resolving dependencies.
    /// </param>
    /// <param name="httpContextAccessor">
    ///     The HTTP context accessor to use for accessing the current
    ///     HTTP context.
    /// </param>
    public ServiceFactory(IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) {
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    /// <remarks>
    ///     Important: The <paramref name="service"/> must be a concrete
    ///     type, not an interface or abstract class.
    /// 
    ///     This method will try to create the instance of the specified
    ///     service using the provided <paramref name="serviceProvider"/>.
    ///     If the provider is <see langword="null"/>, it will use the current
    ///     HTTP context's request services. If that is also
    ///     <see langword="null"/>, it will fall back to the default service
    ///     provider that was initialized in the constructor.
    /// </remarks>
    public object Create(Type service, IServiceProvider? serviceProvider = null) {
        var factory = _factories.GetOrAdd(service, FactoryInitializer);

        var current = serviceProvider
                      ?? _httpContextAccessor.HttpContext?.RequestServices
                      ?? _serviceProvider;

        return factory(current, arguments: null);

        static ObjectFactory FactoryInitializer(Type type) {
            return ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
        }
    }
}