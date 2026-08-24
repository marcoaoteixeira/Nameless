namespace Nameless.Reporting;

/// <summary>
///     Carries the reason a status channel faulted. Only ever constructed and
///     passed to <see cref="IObserver{T}.OnError"/> - never thrown - so
///     <see cref="Exception.StackTrace"/> stays <see langword="null"/> and no
///     internal detail leaks to a remote (WPF/SignalR) consumer.
/// </summary>
public sealed class FaultException(string reason, string? code = null) : Exception(reason) {
    /// <summary>
    ///     Gets the optional error code associated with the fault.
    /// </summary>
    public string? Code { get; } = code;
}
