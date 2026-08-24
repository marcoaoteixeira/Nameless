using Nameless.Registration;

namespace Nameless.Reporting;

/// <summary>
///     Registration for Status Reporting feature.
/// </summary>
public class StatusReportingRegistration : AssemblyScanAware<StatusReportingRegistration> {
    private readonly HashSet<Type> _services = [];

    /// <summary>
    ///     Gets or sets the size of the buffer.
    /// </summary>
    public int BufferSize { get; set; } = 10;

    /// <summary>
    ///     Gets the list of services to register.
    /// </summary>
    public IReadOnlyCollection<Type> ForServices => _services;

    /// <summary>
    ///     Registers service <typeparamref name="TService"/> for status
    ///     reporting and monitoring.
    /// </summary>
    /// <typeparam name="TService">
    ///     Type of the service.
    /// </typeparam>
    /// <returns>
    ///     The current instance of <see cref="StatusReportingRegistration"/> so
    ///     other actions can be chained.
    /// </returns>
    public StatusReportingRegistration RegisterForService<TService>() {
        return RegisterForService(typeof(TService));
    }

    /// <summary>
    ///     Registers service <paramref name="service"/> for status
    ///     reporting and monitoring.
    /// </summary>
    /// <param name="service">
    ///     Type of the service.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="StatusReportingRegistration"/> so
    ///     other actions can be chained.
    /// </returns>
    public StatusReportingRegistration RegisterForService(Type service) {
        Throws.When.IsNonConcreteType(service);
        Throws.When.IsOpenGenericType(service);

        _services.Add(service);

        return this;
    }
}
