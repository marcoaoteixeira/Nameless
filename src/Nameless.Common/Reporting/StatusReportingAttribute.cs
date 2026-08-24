namespace Nameless.Reporting;

/// <summary>
///     Marker to auto-register the Status Reporter and Monitor.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StatusReportingAttribute : Attribute;