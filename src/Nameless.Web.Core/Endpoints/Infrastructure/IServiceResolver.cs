namespace Nameless.Web.Endpoints.Infrastructure;

/// <summary>
///     Service resolver interface for creating instances of services.
/// </summary>
public interface IServiceResolver {
    /// <summary>
    ///     Creates an instance of the specified service type.
    /// </summary>
    /// <param name="service">
    ///     The type of service to create an instance of.
    /// </param>
    /// <param name="provider">
    ///     The service provider to use for resolving dependencies.
    ///     If null, the default service provider will be used.
    /// </param>
    /// <returns>
    ///     The created instance of the specified service type.
    /// </returns>
    object CreateInstance(Type service, IServiceProvider? provider = null);
}