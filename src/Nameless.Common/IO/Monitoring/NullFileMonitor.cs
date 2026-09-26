using System.Diagnostics.CodeAnalysis;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.IO.Monitoring;

/// <summary>
///     Null-Object pattern implementation of <see cref="IFileMonitor"/>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.Internal)]
public sealed class NullFileMonitor : IFileMonitor {
    /// <summary>
    ///     Gets the single instance of <see cref="NullFileMonitor"/>.
    /// </summary>
    public static IFileMonitor Instance { get; } = new NullFileMonitor();

    static NullFileMonitor() { }

    private NullFileMonitor() { }

    /// <inheritdoc/>
    public string Root => string.Empty;

    /// <inheritdoc/>
    public string Glob => string.Empty;

    /// <inheritdoc/>
    public void OnCreated(Action<FileCreatedEvent> action) { }

    /// <inheritdoc/>
    public void OnRenamed(Action<FileRenamedEvent> action) { }

    /// <inheritdoc/>
    public void OnDeleted(Action<FileDeletedEvent> action) { }

    /// <inheritdoc/>
    public void OnChanged(Action<FileChangedEvent> action) { }

    /// <inheritdoc/>
    public void OnError(Action<FileMonitorErrorEvent> action) { }

    /// <inheritdoc/>
    public void Start() { }

    /// <inheritdoc/>
    public void Dispose() { }
}
