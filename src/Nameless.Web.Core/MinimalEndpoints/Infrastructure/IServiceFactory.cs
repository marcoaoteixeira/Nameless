namespace Nameless.Web.MinimalEndpoints.Infrastructure;

/// <summary>
///     Service factory interface for creating services instances.
/// </summary>
public interface IServiceFactory {
    /// <summary>
    ///     Creates an instance of the specified service type.
    /// </summary>
    /// <param name="service">
    ///     The type of service to create an instance of.
    /// </param>
    /// <param name="serviceProvider">
    ///     The service provider to use for resolving dependencies.
    ///     If <see langword="null"/>, the default service provider
    ///     will be used.
    /// </param>
    /// <returns>
    ///     The created instance of the specified service type.
    /// </returns>
    object Create(Type service, IServiceProvider? serviceProvider = null);
}