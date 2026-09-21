namespace Nameless.IO.Monitoring;

/// <summary>
///     Tuning knobs for <see cref="IFileMonitor" />.
/// </summary>
public class FileMonitorOptions {
    /// <summary>
    ///     Gets the glob patterns excluded unless <see cref="Excludes" /> is
    ///     changed: Temp/Office owner files (<c>~$*</c>) and <c>*.tmp</c>
    ///     files.
    /// </summary>
    /// <remarks>
    ///     Editors save through temporary files (Word: <c>~WRD0002.tmp</c>,
    ///     Excel: <c>192807EF.tmp</c>) and drop lock files next to documents
    ///     (<c>~$report.docx</c>). Reporting them buries the real change in
    ///     noise, so they are ignored by default. This is a default, not a
    ///     rule: clear or replace <see cref="Excludes" /> to be told about
    ///     them.
    /// </remarks>
    public static readonly string[] DefaultExcludes = ["**/~$*", "**/*.tmp"];

    /// <summary>
    ///     Gets or sets how long a file must stay free of raw events before
    ///     it is probed.
    /// </summary>
    public TimeSpan QuietPeriod { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     Gets or sets the delay before the first re-probe of a locked file.
    /// </summary>
    /// <remarks>
    ///     The delay doubles after each locked probe, up to
    ///     <see cref="MaxProbeInterval" />.
    /// </remarks>
    public TimeSpan ProbeInterval { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     Gets or sets the longest delay between probes of a locked file.
    /// </summary>
    public TimeSpan MaxProbeInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Gets or sets how long a file may stay locked before an
    ///     <see cref="FileLockedTooLongException" /> is reported.
    /// </summary>
    /// <remarks>
    ///     Probing continues after the report, and the change is raised once
    ///     the file is released. The report is repeated only if new raw events
    ///     start another locked episode. It is raised by the first probe after
    ///     the threshold, so back-off can delay it slightly.
    /// </remarks>
    public TimeSpan LockedTooLongAfter { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    ///     Gets or sets how long a deletion is held to see whether the same
    ///     path is replaced, as editors do when saving.
    /// </summary>
    /// <remarks>
    ///     A replaced file is reported as a single change.
    ///     <see cref="TimeSpan.Zero" /> disables this and reports deletions
    ///     immediately.
    /// </remarks>
    public TimeSpan ReplaceGracePeriod { get; set; } = TimeSpan.FromMilliseconds(250);
    
    /// <summary>
    ///     Gets or sets glob patterns, relative to the root, that are never
    ///     reported even when they match the monitored glob.
    /// </summary>
    /// <remarks>
    ///     Starts as <see cref="DefaultExcludes" /> (temporary files). Add
    ///     patterns to ignore more, or call <c>Clear()</c> to receive events
    ///     for temporary files too. Read when the monitor is created.
    /// </remarks>
    public ICollection<string> Excludes { get; set; } = [.. DefaultExcludes];

    /// <summary>
    ///     Gets or sets the size, in bytes, of the watcher's internal buffer.
    /// </summary>
    public int InternalBufferSize { get; set; } = 64 * 1024;
}
